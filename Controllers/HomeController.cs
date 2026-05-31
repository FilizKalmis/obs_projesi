using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ToplamDerslik = await _context.Derslikler.CountAsync();
            ViewBag.AktifDerslik = await _context.Derslikler.CountAsync(x => x.Aktif);
            ViewBag.ToplamDers = await _context.Dersler.CountAsync();
            ViewBag.ToplamSinav = await _context.Sinavlar.CountAsync();
            ViewBag.ToplamPersonel = await _context.Personeller.CountAsync();

            ViewBag.AktifToplamKapasite = await _context.Derslikler
                .Where(x => x.Aktif)
                .SumAsync(x => (int?)x.Kapasite) ?? 0;

            var yaklasanSinavlar = await _context.Sinavlar
                .Include(x => x.Ders)
                .Include(x => x.Oturum)
                .Where(x => x.Tarih >= DateTime.Today)
                .OrderBy(x => x.Tarih)
                .Take(5)
                .ToListAsync();

            return View(yaklasanSinavlar);
        }
    }
}