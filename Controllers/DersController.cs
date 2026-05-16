using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class DersController : Controller
    {
        private readonly AppDbContext _context;

        public DersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dersler = await _context.Dersler
                .Include(d => d.Bolum)
                .OrderBy(d => d.DersKodu)
                .ToListAsync();

            return View(dersler);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle()
        {
            await BolumListesiniYukle();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle([Bind("DersKodu,DersTuru,DersAdi,OgrenciSayisi,Yariyil,BolumID")] Ders ders)
        {
            if (!ModelState.IsValid)
            {
                await BolumListesiniYukle(ders.BolumID);
                return View(ders);
            }

            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ders başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var ders = await _context.Dersler.FindAsync(id);

            if (ders == null)
            {
                TempData["ErrorMessage"] = "Düzenlenecek ders bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await BolumListesiniYukle(ders.BolumID);
            return View(ders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("DersID,DersKodu,DersTuru,DersAdi,OgrenciSayisi,Yariyil,BolumID")] Ders ders)
        {
            if (id != ders.DersID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await BolumListesiniYukle(ders.BolumID);
                return View(ders);
            }

            var mevcutDers = await _context.Dersler.FindAsync(id);

            if (mevcutDers == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek ders bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            mevcutDers.DersKodu = ders.DersKodu;
            mevcutDers.DersAdi = ders.DersAdi;
            mevcutDers.DersTuru = ders.DersTuru;
            mevcutDers.OgrenciSayisi = ders.OgrenciSayisi;
            mevcutDers.Yariyil = ders.Yariyil;
            mevcutDers.BolumID = ders.BolumID;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ders başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool dersVarMi = await _context.Dersler.AnyAsync(d => d.DersID == ders.DersID);

                if (!dersVarMi)
                {
                    return NotFound();
                }

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var ders = await _context.Dersler
                .Include(d => d.Sinavlar)
                .FirstOrDefaultAsync(d => d.DersID == id);

            if (ders == null)
            {
                TempData["ErrorMessage"] = "Silinecek ders bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool derseAitSinavVarMi = await _context.Sinavlar
                .AnyAsync(s => s.DersID == id);

            if (derseAitSinavVarMi)
            {
                TempData["ErrorMessage"] = "Bu derse ait sınav kaydı bulunduğu için ders silinemez. Önce ilgili sınavları silmeniz gerekir.";
                return RedirectToAction(nameof(Index));
            }

            _context.Dersler.Remove(ders);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ders başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task BolumListesiniYukle(int? seciliBolumID = null)
        {
            var bolumler = await _context.Bolumler
                .OrderBy(b => b.BolumAdi)
                .ToListAsync();

            ViewBag.Bolumler = new SelectList(bolumler, "BolumID", "BolumAdi", seciliBolumID);
        }
    }
}