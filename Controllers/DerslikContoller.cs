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
            var derslikler = await _context.Derslikler.ToListAsync();
            return View(derslikler);
        }

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
            derslik.Aktif = true;

            _context.Derslikler.Add(derslik);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Derslik derslik)
        {
            if (id != derslik.DerslikID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(derslik);
            }

            _context.Derslikler.Update(derslik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik başarıyla güncellendi.";
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
                return NotFound();
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
                return NotFound();
            }

            derslik.Aktif = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Derslik aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}