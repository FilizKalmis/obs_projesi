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
                .ToListAsync();

            return View(dersler);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            ViewBag.Bolumler = new SelectList(_context.Bolumler, "BolumID", "BolumAdi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle([Bind("DersKodu,DersTuru,DersAdi,OgrenciSayisi,Yariyil,BolumID")] Ders ders)
        {
            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
