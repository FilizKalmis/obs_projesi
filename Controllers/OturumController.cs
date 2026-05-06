using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class OturumController : Controller
    {
        private readonly AppDbContext _context;

        public OturumController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var oturumlar = await _context.Oturumlar
                .OrderBy(o => o.BaslangicSaat)
                .ToListAsync();

            return View(oturumlar);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle([Bind("Tanim,BaslangicSaat,BitisSaat")] Oturum oturum)
        {
            if (!ModelState.IsValid)
            {
                return View(oturum);
            }

            _context.Oturumlar.Add(oturum);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var oturum = await _context.Oturumlar.FindAsync(id);

            if (oturum == null)
            {
                return NotFound();
            }

            return View(oturum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, [Bind("OturumID,Tanim,BaslangicSaat,BitisSaat")] Oturum oturum)
        {
            if (id != oturum.OturumID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(oturum);
            }

            try
            {
                _context.Update(oturum);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool oturumVarMi = await _context.Oturumlar
                    .AnyAsync(o => o.OturumID == oturum.OturumID);

                if (!oturumVarMi)
                {
                    return NotFound();
                }

                throw;
            }
        }
    }
}