using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;
using OBS_Projesi.Models.ViewModels;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class SinavController : Controller
    {
        private readonly AppDbContext _context;

        public SinavController(AppDbContext context)
        {
            _context = context;
        }

        // Sınav Planlama menüsüne tıklanınca burası açılır.
        // Direkt sınav oluşturma ekranı değil, sınav programı listesi gelir.
        public async Task<IActionResult> Index()
        {
            var sinavlar = await _context.Sinavlar
                .Include(s => s.Ders)
                    .ThenInclude(d => d.Bolum)
                .Include(s => s.Oturum)
                .Include(s => s.SinavSalonlari)
                    .ThenInclude(ss => ss.Derslik)
                .OrderBy(s => s.Tarih)
                .ThenBy(s => s.Oturum.BaslangicSaat)
                .ToListAsync();

            return View(sinavlar);
        }

        // Eski Program action'ı kullanılmışsa bozulmasın diye Index'e yönlendiriyoruz.
        public IActionResult Program()
        {
            return RedirectToAction(nameof(Index));
        }

        // Yeni sınav oluşturma ekranı
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Olustur()
        {
            await FormListeleriniYukle();

            var model = new SinavOlusturViewModel
            {
                Tarih = DateTime.Today
            };

            return View(model);
        }

        // Yeni sınav oluşturma + otomatik salon atama
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Olustur(SinavOlusturViewModel model)
        {
            await FormListeleriniYukle();

            if (model.DersID <= 0)
            {
                ModelState.AddModelError("DersID", "Ders seçimi zorunludur.");
            }

            if (model.OturumID <= 0)
            {
                ModelState.AddModelError("OturumID", "Oturum seçimi zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ders = await _context.Dersler
                .Include(d => d.Bolum)
                .FirstOrDefaultAsync(d => d.DersID == model.DersID);

            if (ders == null)
            {
                ModelState.AddModelError("", "Seçilen ders bulunamadı.");
                return View(model);
            }

            var oturum = await _context.Oturumlar
                .FirstOrDefaultAsync(o => o.OturumID == model.OturumID);

            if (oturum == null)
            {
                ModelState.AddModelError("", "Seçilen oturum bulunamadı.");
                return View(model);
            }

            var gunBaslangic = model.Tarih.Date;
            var gunBitis = gunBaslangic.AddDays(1);

            // Aynı ders aynı gün ve aynı oturumda tekrar oluşturulmasın.
            var ayniDersAyniOturumVarMi = await _context.Sinavlar
                .AnyAsync(s =>
                    s.DersID == model.DersID &&
                    s.Tarih >= gunBaslangic &&
                    s.Tarih < gunBitis &&
                    s.OturumID == model.OturumID
                );

            if (ayniDersAyniOturumVarMi)
            {
                ModelState.AddModelError("", "Bu ders için aynı gün ve aynı oturumda zaten sınav oluşturulmuş.");
                return View(model);
            }

            // Aynı bölüm ve aynı yarıyıldaki dersler aynı gün aynı oturuma konulmasın.
            var yariyilCakismaVarMi = await _context.Sinavlar
                .Include(s => s.Ders)
                .AnyAsync(s =>
                    s.Tarih >= gunBaslangic &&
                    s.Tarih < gunBitis &&
                    s.OturumID == model.OturumID &&
                    s.Ders.Yariyil == ders.Yariyil &&
                    s.Ders.BolumID == ders.BolumID
                );

            if (yariyilCakismaVarMi)
            {
                ModelState.AddModelError("", "Bu bölüm ve yarıyıla ait başka bir ders aynı gün ve aynı oturumda zaten planlanmış.");
                return View(model);
            }

            // Aynı bölüm ve aynı yarıyıl için bir güne 2'den fazla sınav uyarısı
            var gunlukYariyilSinavSayisi = await _context.Sinavlar
                .Include(s => s.Ders)
                .CountAsync(s =>
                    s.Tarih >= gunBaslangic &&
                    s.Tarih < gunBitis &&
                    s.Ders.Yariyil == ders.Yariyil &&
                    s.Ders.BolumID == ders.BolumID
                );

            if (gunlukYariyilSinavSayisi >= 2)
            {
                TempData["WarningMessage"] = $"{ders.Bolum?.BolumAdi} bölümü {ders.Yariyil}. yarıyıl için bu güne 2'den fazla sınav planlanıyor. Kontrol etmeniz önerilir.";
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sinav = new Sinav
                {
                    DersID = model.DersID,
                    Tarih = model.Tarih.Date,
                    OturumID = model.OturumID
                };

                _context.Sinavlar.Add(sinav);
                await _context.SaveChangesAsync();

                // Aynı tarih ve oturumda kullanılan derslikleri bul.
                var kullanilanDerslikIDleri = await _context.SinavSalonlari
                    .Include(ss => ss.Sinav)
                    .Where(ss =>
                        ss.Sinav.Tarih >= gunBaslangic &&
                        ss.Sinav.Tarih < gunBitis &&
                        ss.Sinav.OturumID == model.OturumID
                    )
                    .Select(ss => ss.DerslikID)
                    .ToListAsync();

                // Boş ve aktif derslikleri al.
                var bosDerslikler = await _context.Derslikler
                    .Where(d =>
                        d.Aktif &&
                        !kullanilanDerslikIDleri.Contains(d.DerslikID)
                    )
                    .ToListAsync();

                if (!bosDerslikler.Any())
                {
                    throw new Exception("Uygun boş derslik bulunamadı. Sınav oluşturulmadı.");
                }

                var hedefOgrenciSayisi = ders.OgrenciSayisi;
                var secilenDerslikler = new List<Derslik>();

                // Önce aynı katta yeterli kapasite var mı kontrol et.
                var ayniKattaUygunSecenek = bosDerslikler
                    .GroupBy(d => d.Kat)
                    .Select(grup =>
                    {
                        var siraliDerslikler = grup
                            .OrderByDescending(d => d.Kapasite)
                            .ToList();

                        var secilenler = new List<Derslik>();
                        var kalan = hedefOgrenciSayisi;

                        foreach (var derslik in siraliDerslikler)
                        {
                            if (kalan <= 0)
                            {
                                break;
                            }

                            secilenler.Add(derslik);
                            kalan -= derslik.Kapasite;
                        }

                        return new
                        {
                            Kat = grup.Key,
                            Derslikler = secilenler,
                            ToplamKapasite = secilenler.Sum(d => d.Kapasite),
                            Kalan = kalan
                        };
                    })
                    .Where(x => x.Kalan <= 0)
                    .OrderBy(x => x.Derslikler.Count)
                    .ThenBy(x => x.ToplamKapasite)
                    .FirstOrDefault();

                if (ayniKattaUygunSecenek != null)
                {
                    secilenDerslikler = ayniKattaUygunSecenek.Derslikler;
                }
                else
                {
                    // Aynı katta yeterli kapasite yoksa genel kapasite sıralamasına geç.
                    var genelSiraliDerslikler = bosDerslikler
                        .OrderByDescending(d => d.Kapasite)
                        .ToList();

                    var kalan = hedefOgrenciSayisi;

                    foreach (var derslik in genelSiraliDerslikler)
                    {
                        if (kalan <= 0)
                        {
                            break;
                        }

                        secilenDerslikler.Add(derslik);
                        kalan -= derslik.Kapasite;
                    }
                }

                var toplamSecilenKapasite = secilenDerslikler.Sum(d => d.Kapasite);

                if (toplamSecilenKapasite < hedefOgrenciSayisi)
                {
                    throw new Exception("Yeterli boş derslik bulunamadı. Sınav oluşturulmadı.");
                }

                var atananSalonlar = new List<string>();

                foreach (var derslik in secilenDerslikler)
                {
                    var sinavSalonu = new SinavSalonu
                    {
                        SinavID = sinav.SinavID,
                        DerslikID = derslik.DerslikID
                    };

                    _context.SinavSalonlari.Add(sinavSalonu);

                    atananSalonlar.Add($"{derslik.Ad} ({derslik.Kapasite} kişi / {derslik.Kat})");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Sınav oluşturuldu. Atanan salonlar: {string.Join(", ", atananSalonlar)}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("", $"Sınav oluşturulurken hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        // Düzenleme ekranı
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var sinav = await _context.Sinavlar
                .Include(s => s.Ders)
                .Include(s => s.Oturum)
                .FirstOrDefaultAsync(s => s.SinavID == id);

            if (sinav == null)
            {
                return NotFound();
            }

            await FormListeleriniYukle();

            return View(sinav);
        }

        // Sınav düzenleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Sinav sinav)
        {
            if (id != sinav.SinavID)
            {
                return NotFound();
            }

            ModelState.Remove("Ders");
            ModelState.Remove("Oturum");
            ModelState.Remove("SinavSalonlari");

            await FormListeleriniYukle();

            if (!ModelState.IsValid)
            {
                return View(sinav);
            }

            var mevcutSinav = await _context.Sinavlar
                .Include(s => s.Ders)
                .FirstOrDefaultAsync(s => s.SinavID == id);

            if (mevcutSinav == null)
            {
                return NotFound();
            }

            var yeniDers = await _context.Dersler
                .FirstOrDefaultAsync(d => d.DersID == sinav.DersID);

            if (yeniDers == null)
            {
                ModelState.AddModelError("", "Seçilen ders bulunamadı.");
                return View(sinav);
            }

            var gunBaslangic = sinav.Tarih.Date;
            var gunBitis = gunBaslangic.AddDays(1);

            // Düzenleme sırasında aynı bölüm + aynı yarıyıl + aynı tarih + aynı oturum çakışması kontrolü
            var cakismaVarMi = await _context.Sinavlar
                .Include(s => s.Ders)
                .AnyAsync(s =>
                    s.SinavID != id &&
                    s.Tarih >= gunBaslangic &&
                    s.Tarih < gunBitis &&
                    s.OturumID == sinav.OturumID &&
                    s.Ders.Yariyil == yeniDers.Yariyil &&
                    s.Ders.BolumID == yeniDers.BolumID
                );

            if (cakismaVarMi)
            {
                ModelState.AddModelError("", "Bu bölüm ve yarıyıla ait başka bir ders aynı gün ve aynı oturumda zaten planlanmış.");
                return View(sinav);
            }

            var gunlukSinavSayisi = await _context.Sinavlar
                .Include(s => s.Ders)
                .CountAsync(s =>
                    s.SinavID != id &&
                    s.Tarih >= gunBaslangic &&
                    s.Tarih < gunBitis &&
                    s.Ders.Yariyil == yeniDers.Yariyil &&
                    s.Ders.BolumID == yeniDers.BolumID
                );

            if (gunlukSinavSayisi >= 2)
            {
                TempData["WarningMessage"] = $"{yeniDers.Yariyil}. yarıyıl için bu güne 2'den fazla sınav planlanıyor. Kontrol etmeniz önerilir.";
            }

            mevcutSinav.DersID = sinav.DersID;
            mevcutSinav.Tarih = sinav.Tarih.Date;
            mevcutSinav.OturumID = sinav.OturumID;

            try
            {
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sınav bilgileri başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Sınav güncellenirken hata oluştu: {ex.Message}");
                return View(sinav);
            }
        }

        // Sınav silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sinav = await _context.Sinavlar
                    .FirstOrDefaultAsync(s => s.SinavID == id);

                if (sinav == null)
                {
                    TempData["ErrorMessage"] = "Silinecek sınav bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                // 1. Önce bu sınava bağlı SinavSalonu ID'lerini bul.
                var sinavSalonuIdleri = await _context.SinavSalonlari
                    .Where(ss => ss.SinavID == id)
                    .Select(ss => ss.SinavSalonuID)
                    .ToListAsync();

                // 2. Önce bağlı GözetmenAtama kayıtlarını sil.
                var gozetmenAtamalari = await _context.GozetmenAtamalari
                    .Where(ga => sinavSalonuIdleri.Contains(ga.SinavSalonuID))
                    .ToListAsync();

                if (gozetmenAtamalari.Any())
                {
                    _context.GozetmenAtamalari.RemoveRange(gozetmenAtamalari);
                    await _context.SaveChangesAsync();
                }

                // 3. Sonra bağlı SinavSalonu kayıtlarını sil.
                var sinavSalonlari = await _context.SinavSalonlari
                    .Where(ss => ss.SinavID == id)
                    .ToListAsync();

                if (sinavSalonlari.Any())
                {
                    _context.SinavSalonlari.RemoveRange(sinavSalonlari);
                    await _context.SaveChangesAsync();
                }

                // 4. En son Sinav kaydını sil.
                _context.Sinavlar.Remove(sinav);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Sınav, bağlı gözetmen atamaları ve salon kayıtlarıyla birlikte başarıyla silindi.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] = $"Sınav silinirken hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Eski Ekle action'ı kullanılmışsa bozulmasın diye Olustur'a yönlendiriyoruz.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return RedirectToAction(nameof(Olustur));
        }

        private async Task FormListeleriniYukle()
        {
            var dersler = await _context.Dersler
                .Include(d => d.Bolum)
                .OrderBy(d => d.DersKodu)
                .ToListAsync();

            ViewBag.Dersler = dersler
                .Select(d => new SelectListItem
                {
                    Value = d.DersID.ToString(),
                    Text = $"{d.DersKodu} - {d.DersAdi} ({d.OgrenciSayisi} kişi / {d.Yariyil}. yarıyıl)"
                })
                .ToList();

            var oturumlar = await _context.Oturumlar
                .OrderBy(o => o.BaslangicSaat)
                .ToListAsync();

            ViewBag.Oturumlar = oturumlar
                .Select(o => new SelectListItem
                {
                    Value = o.OturumID.ToString(),
                    Text = $"{o.Tanim} ({o.BaslangicSaat:hh\\:mm} - {o.BitisSaat:hh\\:mm})"
                })
                .ToList();
        }
    }
}