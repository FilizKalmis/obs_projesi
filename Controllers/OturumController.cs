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

        [HttpGet]
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
            SaatAraliginiKontrolEt(oturum);

            if (!ModelState.IsValid)
            {
                return View(oturum);
            }

            bool ayniSaatAraligindaOturumVarMi = await _context.Oturumlar
                .AnyAsync(o =>
                    o.BaslangicSaat == oturum.BaslangicSaat &&
                    o.BitisSaat == oturum.BitisSaat);

            if (ayniSaatAraligindaOturumVarMi)
            {
                ModelState.AddModelError("", "Bu saat aralığında zaten bir oturum tanımlı.");
                return View(oturum);
            }

            _context.Oturumlar.Add(oturum);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Oturum başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var oturum = await _context.Oturumlar.FindAsync(id);

            if (oturum == null)
            {
                TempData["ErrorMessage"] = "Düzenlenecek oturum bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(oturum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("OturumID,Tanim,BaslangicSaat,BitisSaat")] Oturum oturum)
        {
            if (id != oturum.OturumID)
            {
                return NotFound();
            }

            SaatAraliginiKontrolEt(oturum);

            if (!ModelState.IsValid)
            {
                return View(oturum);
            }

            var mevcutOturum = await _context.Oturumlar.FindAsync(id);

            if (mevcutOturum == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek oturum bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool ayniSaatAraligindaBaskaOturumVarMi = await _context.Oturumlar
                .AnyAsync(o =>
                    o.OturumID != id &&
                    o.BaslangicSaat == oturum.BaslangicSaat &&
                    o.BitisSaat == oturum.BitisSaat);

            if (ayniSaatAraligindaBaskaOturumVarMi)
            {
                ModelState.AddModelError("", "Bu saat aralığında başka bir oturum zaten tanımlı.");
                return View(oturum);
            }

            mevcutOturum.Tanim = oturum.Tanim;
            mevcutOturum.BaslangicSaat = oturum.BaslangicSaat;
            mevcutOturum.BitisSaat = oturum.BitisSaat;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Oturum başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var oturum = await _context.Oturumlar
                .Include(o => o.Sinavlar)
                .FirstOrDefaultAsync(o => o.OturumID == id);

            if (oturum == null)
            {
                TempData["ErrorMessage"] = "Silinecek oturum bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool oturumSinavdaKullaniliyorMu = await _context.Sinavlar
                .AnyAsync(s => s.OturumID == id);

            if (oturumSinavdaKullaniliyorMu)
            {
                TempData["ErrorMessage"] = "Bu oturuma bağlı sınav bulunduğu için oturum silinemez. Önce ilgili sınavları silmeniz veya başka oturuma taşımanız gerekir.";
                return RedirectToAction(nameof(Index));
            }

            _context.Oturumlar.Remove(oturum);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Oturum başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private void SaatAraliginiKontrolEt(Oturum oturum)
        {
            if (oturum.BitisSaat <= oturum.BaslangicSaat)
            {
                ModelState.AddModelError("", "Bitiş saati başlangıç saatinden sonra olmalıdır.");
            }
        }
    }
}