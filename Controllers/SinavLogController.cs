using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;

namespace OBS_Projesi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SinavLogController : Controller
    {
        private readonly AppDbContext _context;

        public SinavLogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var loglar = await _context.SinavLoglar
                .Include(l => l.Sinav)
                    .ThenInclude(s => s.Ders)
                .Include(l => l.Sinav)
                    .ThenInclude(s => s.Oturum)
                .Include(l => l.DegistirenPersonel)
                .OrderByDescending(l => l.IslemTarihi)
                .ToListAsync();

            return View(loglar);
        }
    }
}
