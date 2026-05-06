using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class DerslikController : Controller
    {
        private readonly AppDbContext _context;

        public DerslikController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var derslikler = await _context.Derslikler
                .OrderByDescending(d => d.Aktif)
                .ThenBy(d => d.Kat)
                .ThenBy(d => d.Ad)
                .ToListAsync();

            return View(derslikler);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            var model = new Derslik
            {
                Aktif = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle([Bind("Ad,Kapasite,Tip,Kat,Aktif")] Derslik derslik)
        {
            if (!ModelState.IsValid)
            {
                return View(derslik);
            }

            derslik.Aktif = true;

            _context.Derslikler.Add(derslik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var derslik = await _context.Derslikler.FindAsync(id);

            if (derslik == null)
            {
                TempData["ErrorMessage"] = "Düzenlenecek derslik bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(derslik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("DerslikID,Ad,Kapasite,Tip,Kat,Aktif")] Derslik derslik)
        {
            if (id != derslik.DerslikID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(derslik);
            }

            var mevcutDerslik = await _context.Derslikler.FindAsync(id);

            if (mevcutDerslik == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek derslik bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            mevcutDerslik.Ad = derslik.Ad;
            mevcutDerslik.Kapasite = derslik.Kapasite;
            mevcutDerslik.Tip = derslik.Tip;
            mevcutDerslik.Kat = derslik.Kat;
            mevcutDerslik.Aktif = derslik.Aktif;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var derslik = await _context.Derslikler
                .Include(d => d.SinavSalonlari)
                .FirstOrDefaultAsync(d => d.DerslikID == id);

            if (derslik == null)
            {
                TempData["ErrorMessage"] = "Silinecek derslik bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool derslikSinavdaKullaniliyorMu = await _context.SinavSalonlari
                .AnyAsync(ss => ss.DerslikID == id);

            if (derslikSinavdaKullaniliyorMu)
            {
                TempData["ErrorMessage"] = "Bu derslik sınavlarda kullanıldığı için silinemez. Bunun yerine pasif hale getirildi.";
                derslik.Aktif = false;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            _context.Derslikler.Remove(derslik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PasifYap(int id)
        {
            var derslik = await _context.Derslikler.FindAsync(id);

            if (derslik == null)
            {
                TempData["ErrorMessage"] = "Derslik bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            derslik.Aktif = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik pasif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AktifYap(int id)
        {
            var derslik = await _context.Derslikler.FindAsync(id);

            if (derslik == null)
            {
                TempData["ErrorMessage"] = "Derslik bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            derslik.Aktif = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}