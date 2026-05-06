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

        // Gözetmen atamalarını listeler
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle()
        {
            await SelectListleriHazirla();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(GozetmenAtama atama)
        {
            await AtamaKurallariniKontrolEt(atama);

            if (!ModelState.IsValid)
            {
                await SelectListleriHazirla();
                return View(atama);
            }

            _context.GozetmenAtamalari.Add(atama);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözetmen ataması başarıyla yapıldı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            // FindAsync(id) doğrudan AtamaID üzerinden arama yapar
            var atama = await _context.GozetmenAtamalari.FindAsync(id);

            if (atama == null)
            {
                return NotFound();
            }

            _context.GozetmenAtamalari.Remove(atama);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözetmen ataması kaldırıldı.";
            return RedirectToAction(nameof(Index));
        }

        private async Task SelectListleriHazirla()
        {
            var sinavSalonlari = await _context.SinavSalonlari
                .Include(ss => ss.Derslik)
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Ders)
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Oturum)
                .OrderBy(ss => ss.Sinav.Tarih)
                .ThenBy(ss => ss.Sinav.Oturum.BaslangicSaat)
                .ToListAsync();

            var sinavSalonuSelect = sinavSalonlari.Select(ss => new
            {
                ss.SinavSalonuID,
                Bilgi = $"{ss.Sinav.Tarih:dd.MM.yyyy} | {ss.Sinav.Oturum.Tanim} | {ss.Sinav.Ders.DersKodu} - {ss.Sinav.Ders.DersAdi} | Salon: {ss.Derslik.Ad}"
            }).ToList();

            ViewBag.SinavSalonlari = new SelectList(sinavSalonuSelect, "SinavSalonuID", "Bilgi");

            var personeller = await _context.Personeller
                .Include(p => p.Bolum)
                .OrderBy(p => p.Bolum.BolumAdi)
                .ThenBy(p => p.Ad)
                .ToListAsync();

            var personelSelect = personeller.Select(p => new
            {
                p.PersonelID,
                Bilgi = $"{p.Unvan} {p.Ad} {p.Soyad} | {p.Bolum.BolumAdi}"
            }).ToList();

            ViewBag.Personeller = new SelectList(personelSelect, "PersonelID", "Bilgi");

            // Personel yükü takibi için (Opsiyonel)
            ViewBag.PersonelYukleri = await _context.GozetmenAtamalari
                .Include(g => g.Personel)
                .GroupBy(g => new
                {
                    g.PersonelID,
                    g.Personel.Unvan,
                    g.Personel.Ad,
                    g.Personel.Soyad
                })
                .Select(g => new
                {
                    Personel = $"{g.Key.Unvan} {g.Key.Ad} {g.Key.Soyad}",
                    GorevSayisi = g.Count()
                })
                .OrderBy(x => x.GorevSayisi)
                .ToListAsync();
        }

        private async Task AtamaKurallariniKontrolEt(GozetmenAtama atama)
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

            // Çakışma ve Mazeret Kontrolü İçin Değişkenler
            var tarih = secilenSinavSalonu.Sinav.Tarih.Date;
            var oturumID = secilenSinavSalonu.Sinav.OturumID;

            // 1. Kural: Aynı personeli aynı salona iki kez atama
            bool ayniSalonaAyniPersonelAtanmis = await _context.GozetmenAtamalari
                .AnyAsync(g =>
                    g.SinavSalonuID == atama.SinavSalonuID &&
                    g.PersonelID == atama.PersonelID
                );

            if (ayniSalonaAyniPersonelAtanmis)
            {
                ModelState.AddModelError("", "Bu personel zaten bu sınav salonuna atanmış.");
            }

            // 2. Kural: Aynı personeli aynı tarih ve oturumda başka salona atama (Çakışma)
            bool ayniAndaBaskaSalondaGorevVar = await _context.GozetmenAtamalari
                .Include(g => g.SinavSalonu)
                    .ThenInclude(ss => ss.Sinav)
                .AnyAsync(g =>
                    g.PersonelID == atama.PersonelID &&
                    g.SinavSalonu.Sinav.Tarih.Date == tarih &&
                    g.SinavSalonu.Sinav.OturumID == oturumID
                );

            if (ayniAndaBaskaSalondaGorevVar)
            {
                ModelState.AddModelError("", "Bu personel aynı tarih ve oturumda başka bir salonda görevli.");
            }

            // 3. Kural: Personel mazeret kontrolü (UygunMu alanı üzerinden)
            bool personelMazeretli = await _context.PersonelMazeretleri
                .AnyAsync(m =>
                    m.PersonelID == atama.PersonelID &&
                    m.Tarih.Date == tarih &&
                    m.OturumID == oturumID &&
                    m.UygunMu == false // 'Uygun' yerine senin tablondaki 'UygunMu' kullanıldı
                );

            if (personelMazeretli)
            {
                ModelState.AddModelError("", "Bu personel seçilen tarih ve oturumda izinli veya uygun değil.");
            }
        }
    }
}
