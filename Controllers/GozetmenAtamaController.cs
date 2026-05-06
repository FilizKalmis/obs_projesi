using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class GozetmenAtamaController : Controller
    {
        private readonly AppDbContext _context;

        public GozetmenAtamaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var atamalar = await _context.GozetmenAtamalari
                .Include(g => g.Personel)
                    .ThenInclude(p => p.Bolum)
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Derslik)
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                        .ThenInclude(s => s.Ders)
                            .ThenInclude(d => d.Bolum)
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                        .ThenInclude(s => s.Oturum)
                .OrderBy(g => g.SinavSalonu.Sinav.Tarih)
                .ThenBy(g => g.SinavSalonu.Sinav.Oturum.BaslangicSaat)
                .ThenBy(g => g.Personel.Ad)
                .ToListAsync();

            return View(atamalar);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle()
        {
            await SelectListleriHazirla();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle([Bind("SinavSalonuID,PersonelID")] GozetmenAtama atama)
        {
            await AtamaKurallariniKontrolEt(atama);

            if (!ModelState.IsValid)
            {
                await SelectListleriHazirla(atama.SinavSalonuID, atama.PersonelID);
                return View(atama);
            }

            _context.GozetmenAtamalari.Add(atama);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözetmen ataması başarıyla yapıldı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var atama = await _context.GozetmenAtamalari
                .Include(g => g.Personel)
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                .FirstOrDefaultAsync(g => g.AtamaID == id);

            if (atama == null)
            {
                TempData["ErrorMessage"] = "Düzenlenecek gözetmen ataması bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await SelectListleriHazirla(atama.SinavSalonuID, atama.PersonelID);
            return View(atama);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("AtamaID,SinavSalonuID,PersonelID")] GozetmenAtama atama)
        {
            if (id != atama.AtamaID)
            {
                return NotFound();
            }

            await AtamaKurallariniKontrolEt(atama, atama.AtamaID);

            if (!ModelState.IsValid)
            {
                await SelectListleriHazirla(atama.SinavSalonuID, atama.PersonelID);
                return View(atama);
            }

            var mevcutAtama = await _context.GozetmenAtamalari.FindAsync(id);

            if (mevcutAtama == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek gözetmen ataması bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            mevcutAtama.SinavSalonuID = atama.SinavSalonuID;
            mevcutAtama.PersonelID = atama.PersonelID;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözetmen ataması başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var atama = await _context.GozetmenAtamalari.FindAsync(id);

            if (atama == null)
            {
                TempData["ErrorMessage"] = "Silinecek gözetmen ataması bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            _context.GozetmenAtamalari.Remove(atama);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözetmen ataması kaldırıldı.";
            return RedirectToAction(nameof(Index));
        }

        private async Task SelectListleriHazirla(int? seciliSinavSalonuID = null, int? seciliPersonelID = null)
        {
            var sinavSalonlari = await _context.SinavSalonlari
                .Include(ss => ss.Derslik)
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Ders)
                        .ThenInclude(d => d.Bolum)
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Oturum)
                .OrderBy(ss => ss.Sinav.Tarih)
                .ThenBy(ss => ss.Sinav.Oturum.BaslangicSaat)
                .ThenBy(ss => ss.Derslik.Ad)
                .ToListAsync();

            var sinavSalonuSelect = sinavSalonlari.Select(ss => new
            {
                ss.SinavSalonuID,
                Bilgi = $"{ss.Sinav.Tarih:dd.MM.yyyy} | {ss.Sinav.Oturum.Tanim} | {ss.Sinav.Ders.DersKodu} - {ss.Sinav.Ders.DersAdi} | Salon: {ss.Derslik.Ad}"
            }).ToList();

            ViewBag.SinavSalonlari = new SelectList(
                sinavSalonuSelect,
                "SinavSalonuID",
                "Bilgi",
                seciliSinavSalonuID
            );

            int? seciliSinavBolumID = null;

            if (seciliSinavSalonuID.HasValue)
            {
                seciliSinavBolumID = sinavSalonlari
                    .FirstOrDefault(ss => ss.SinavSalonuID == seciliSinavSalonuID.Value)
                    ?.Sinav
                    ?.Ders
                    ?.BolumID;
            }

            var gorevSayilari = await _context.GozetmenAtamalari
                .GroupBy(g => g.PersonelID)
                .Select(g => new
                {
                    PersonelID = g.Key,
                    GorevSayisi = g.Count()
                })
                .ToDictionaryAsync(x => x.PersonelID, x => x.GorevSayisi);

            var personeller = await _context.Personeller
                .Include(p => p.Bolum)
                .ToListAsync();

            var personelSelect = personeller
                .Select(p =>
                {
                    int gorevSayisi = gorevSayilari.ContainsKey(p.PersonelID)
                        ? gorevSayilari[p.PersonelID]
                        : 0;

                    bool ayniBolum = seciliSinavBolumID.HasValue && p.BolumID == seciliSinavBolumID.Value;

                    return new
                    {
                        p.PersonelID,
                        p.BolumID,
                        GorevSayisi = gorevSayisi,
                        AyniBolum = ayniBolum,
                        Bilgi = $"{p.Unvan} {p.Ad} {p.Soyad} | {p.Bolum.BolumAdi} | Görev: {gorevSayisi}"
                    };
                })
                .OrderByDescending(p => p.AyniBolum)
                .ThenBy(p => p.GorevSayisi)
                .ThenBy(p => p.BolumID)
                .ThenBy(p => p.Bilgi)
                .ToList();

            ViewBag.Personeller = new SelectList(
                personelSelect,
                "PersonelID",
                "Bilgi",
                seciliPersonelID
            );

            ViewBag.PersonelYukleri = personelSelect
                .Select(p => new
                {
                    Personel = p.Bilgi,
                    p.GorevSayisi
                })
                .ToList();
        }

        private async Task AtamaKurallariniKontrolEt(GozetmenAtama atama, int? mevcutAtamaID = null)
        {
            var secilenSinavSalonu = await _context.SinavSalonlari
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Oturum)
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Ders)
                .Include(ss => ss.Derslik)
                .FirstOrDefaultAsync(ss => ss.SinavSalonuID == atama.SinavSalonuID);

            if (secilenSinavSalonu == null)
            {
                ModelState.AddModelError("SinavSalonuID", "Geçerli bir sınav salonu seçmelisiniz.");
                return;
            }

            bool personelVarMi = await _context.Personeller.AnyAsync(p => p.PersonelID == atama.PersonelID);

            if (!personelVarMi)
            {
                ModelState.AddModelError("PersonelID", "Geçerli bir gözetmen seçmelisiniz.");
                return;
            }

            var tarih = secilenSinavSalonu.Sinav.Tarih.Date;
            var oturumID = secilenSinavSalonu.Sinav.OturumID;

            bool ayniSalonaAyniPersonelAtanmis = await _context.GozetmenAtamalari
                .AnyAsync(g =>
                    g.SinavSalonuID == atama.SinavSalonuID &&
                    g.PersonelID == atama.PersonelID &&
                    (!mevcutAtamaID.HasValue || g.AtamaID != mevcutAtamaID.Value)
                );

            if (ayniSalonaAyniPersonelAtanmis)
            {
                ModelState.AddModelError("", "Bu personel zaten bu sınav salonuna atanmış.");
            }

            bool ayniAndaBaskaSalondaGorevVar = await _context.GozetmenAtamalari
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                .AnyAsync(g =>
                    g.PersonelID == atama.PersonelID &&
                    g.SinavSalonu.Sinav.Tarih.Date == tarih &&
                    g.SinavSalonu.Sinav.OturumID == oturumID &&
                    (!mevcutAtamaID.HasValue || g.AtamaID != mevcutAtamaID.Value)
                );

            if (ayniAndaBaskaSalondaGorevVar)
            {
                ModelState.AddModelError("", "Bu personel aynı tarih ve oturumda başka bir salonda görevli.");
            }

            bool personelMazeretli = await _context.PersonelMazeretleri
                .AnyAsync(m =>
                    m.PersonelID == atama.PersonelID &&
                    m.Tarih.Date == tarih &&
                    m.OturumID == oturumID &&
                    m.UygunMu == false
                );

            if (personelMazeretli)
            {
                ModelState.AddModelError("", "Bu personel seçilen tarih ve oturumda izinli veya uygun değil.");
            }

            bool ucOturumSiniriAsiliyorMu = await UcOturumSiniriAsiliyorMu(
                atama.PersonelID,
                tarih,
                oturumID,
                mevcutAtamaID
            );

            if (ucOturumSiniriAsiliyorMu)
            {
                ModelState.AddModelError("", "Bu personel aynı gün arka arkaya en fazla 3 oturumda görev alabilir. Bu atama 3 oturum sınırını aşıyor.");
            }
        }

        private async Task<bool> UcOturumSiniriAsiliyorMu(
            int personelID,
            DateTime tarih,
            int yeniOturumID,
            int? mevcutAtamaID = null)
        {
            var oturumlar = await _context.Oturumlar
                .OrderBy(o => o.BaslangicSaat)
                .Select(o => new
                {
                    o.OturumID,
                    o.BaslangicSaat
                })
                .ToListAsync();

            var oturumSiraMap = oturumlar
                .Select((o, index) => new
                {
                    o.OturumID,
                    Sira = index + 1
                })
                .ToDictionary(x => x.OturumID, x => x.Sira);

            if (!oturumSiraMap.ContainsKey(yeniOturumID))
            {
                return false;
            }

            var mevcutOturumIDleri = await _context.GozetmenAtamalari
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                .Where(g =>
                    g.PersonelID == personelID &&
                    g.SinavSalonu.Sinav.Tarih.Date == tarih &&
                    (!mevcutAtamaID.HasValue || g.AtamaID != mevcutAtamaID.Value)
                )
                .Select(g => g.SinavSalonu.Sinav.OturumID)
                .ToListAsync();

            mevcutOturumIDleri.Add(yeniOturumID);

            var siralar = mevcutOturumIDleri
                .Where(id => oturumSiraMap.ContainsKey(id))
                .Select(id => oturumSiraMap[id])
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            int enUzunArdisik = 0;
            int mevcutArdisik = 0;
            int? oncekiSira = null;

            foreach (var sira in siralar)
            {
                if (oncekiSira.HasValue && sira == oncekiSira.Value + 1)
                {
                    mevcutArdisik++;
                }
                else
                {
                    mevcutArdisik = 1;
                }

                enUzunArdisik = Math.Max(enUzunArdisik, mevcutArdisik);
                oncekiSira = sira;
            }

            return enUzunArdisik > 3;
        }
    }
}