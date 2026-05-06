using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

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

        public async Task<IActionResult> Index()
        {
            var sinavlar = await _context.Sinavlar
                .Include(s => s.Ders)
                    .ThenInclude(d => d.Bolum)
                .Include(s => s.Oturum)
                .OrderBy(s => s.Tarih)
                .ThenBy(s => s.Oturum.BaslangicSaat)
                .ToListAsync();

            return View(sinavlar);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle()
        {
            await SelectListleriHazirla();

            var model = new Sinav
            {
                Tarih = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Sinav sinav)
        {
            await PlanlamaKurallariniKontrolEt(sinav);

            if (!ModelState.IsValid)
            {
                await SelectListleriHazirla();
                return View(sinav);
            }

            _context.Sinavlar.Add(sinav);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sınav başarıyla planlandı.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var sinav = await _context.Sinavlar.FindAsync(id);

            if (sinav == null)
            {
                return NotFound();
            }

            await SelectListleriHazirla();
            return View(sinav);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Sinav sinav)
        {
            if (id != sinav.SinavID)
            {
                return BadRequest();
            }

            await PlanlamaKurallariniKontrolEt(sinav, id);

            if (!ModelState.IsValid)
            {
                await SelectListleriHazirla();
                return View(sinav);
            }

            _context.Sinavlar.Update(sinav);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sınav başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var sinav = await _context.Sinavlar
                .Include(s => s.SinavSalonlari)
                .FirstOrDefaultAsync(s => s.SinavID == id);

            if (sinav == null)
            {
                return NotFound();
            }

            if (sinav.SinavSalonlari.Any())
            {
                TempData["ErrorMessage"] = "Bu sınava salon ataması yapıldığı için önce salon atamalarını kaldırmalısınız.";
                return RedirectToAction(nameof(Index));
            }

            _context.Sinavlar.Remove(sinav);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sınav başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task SelectListleriHazirla()
        {
            var dersler = await _context.Dersler
                .Include(d => d.Bolum)
                .OrderBy(d => d.Yariyil)
                .ThenBy(d => d.DersKodu)
                .ToListAsync();

            var dersSelect = dersler.Select(d => new
            {
                d.DersID,
                DersBilgisi = $"{d.DersKodu} - {d.DersAdi} | {d.Yariyil}. Yarıyıl | {d.Bolum?.BolumAdi}"
            }).ToList();

            ViewBag.Dersler = new SelectList(dersSelect, "DersID", "DersBilgisi");

            var oturumlar = await _context.Oturumlar
                .OrderBy(o => o.BaslangicSaat)
                .ToListAsync();

            var oturumSelect = oturumlar.Select(o => new
            {
                o.OturumID,
                OturumBilgisi = $"{o.Tanim} ({o.BaslangicSaat:hh\\:mm} - {o.BitisSaat:hh\\:mm})"
            }).ToList();

            ViewBag.Oturumlar = new SelectList(oturumSelect, "OturumID", "OturumBilgisi");
        }

        private async Task PlanlamaKurallariniKontrolEt(Sinav sinav, int? mevcutSinavID = null)
        {
            var secilenDers = await _context.Dersler
                .FirstOrDefaultAsync(d => d.DersID == sinav.DersID);

            if (secilenDers == null)
            {
                ModelState.AddModelError("DersID", "Geçerli bir ders seçmelisiniz.");
                return;
            }

            var tarih = sinav.Tarih.Date;

            bool ayniYariyilAyniOturumCakisma = await _context.Sinavlar
                .Include(s => s.Ders)
                .AnyAsync(s =>
                    (!mevcutSinavID.HasValue || s.SinavID != mevcutSinavID.Value) &&
                    s.Tarih.Date == tarih &&
                    s.OturumID == sinav.OturumID &&
                    s.Ders.Yariyil == secilenDers.Yariyil
                );

            if (ayniYariyilAyniOturumCakisma)
            {
                ModelState.AddModelError("", "Aynı yarıyıldaki başka bir ders bu tarih ve oturumda zaten planlanmış. Çakışma oluşur.");
            }

            int ayniGunAyniYariyilSinavSayisi = await _context.Sinavlar
                .Include(s => s.Ders)
                .CountAsync(s =>
                    (!mevcutSinavID.HasValue || s.SinavID != mevcutSinavID.Value) &&
                    s.Tarih.Date == tarih &&
                    s.Ders.Yariyil == secilenDers.Yariyil
                );

            if (ayniGunAyniYariyilSinavSayisi >= 2)
            {
                ModelState.AddModelError("", "Bu yarıyıl için aynı güne 2’den fazla sınav planlanamaz.");
            }

            bool ayniDersAyniTarihOturum = await _context.Sinavlar
                .AnyAsync(s =>
                    (!mevcutSinavID.HasValue || s.SinavID != mevcutSinavID.Value) &&
                    s.DersID == sinav.DersID &&
                    s.Tarih.Date == tarih &&
                    s.OturumID == sinav.OturumID
                );

            if (ayniDersAyniTarihOturum)
            {
                ModelState.AddModelError("", "Bu ders için aynı tarih ve oturumda zaten sınav kaydı var.");
            }
        }
    }
}