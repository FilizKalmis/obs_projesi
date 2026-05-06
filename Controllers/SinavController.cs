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

        public async Task<IActionResult> Program()
        {
            var sinavlar = await _context.Sinavlar
                .Include(s => s.Ders)
                .Include(s => s.Oturum)
                .Include(s => s.SinavSalonlari)
                    .ThenInclude(ss => ss.Derslik)
                .OrderBy(s => s.Tarih)
                .ToListAsync();

            return View(sinavlar);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Olustur()
        {
            ViewBag.Dersler = new SelectList(_context.Dersler, "DersID", "DersAdi");
            ViewBag.Oturumlar = new SelectList(_context.Oturumlar, "OturumID", "Tanim");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Olustur([Bind("DersID,Tarih,OturumID")] Sinav sinav)
        {
            var ders = await _context.Dersler.FirstOrDefaultAsync(d => d.DersID == sinav.DersID);

            if (ders == null)
            {
                ModelState.AddModelError("", "Ders bulunamadı.");
                ViewBag.Dersler = new SelectList(_context.Dersler, "DersID", "DersAdi", sinav.DersID);
                ViewBag.Oturumlar = new SelectList(_context.Oturumlar, "OturumID", "Tanim", sinav.OturumID);
                return View(sinav);
            }

            bool cakismaVar = await _context.Sinavlar
                .Include(s => s.Ders)
                .AnyAsync(s =>
                    s.Tarih == sinav.Tarih &&
                    s.OturumID == sinav.OturumID &&
                    s.Ders != null &&
                    s.Ders.Yariyil == ders.Yariyil &&
                    s.Ders.DersTuru == "Zorunlu" &&
                    ders.DersTuru == "Zorunlu");

            if (cakismaVar)
            {
                ModelState.AddModelError("", "Aynı yarıyıldaki zorunlu ders aynı oturuma atanamaz.");
                ViewBag.Dersler = new SelectList(_context.Dersler, "DersID", "DersAdi", sinav.DersID);
                ViewBag.Oturumlar = new SelectList(_context.Oturumlar, "OturumID", "Tanim", sinav.OturumID);
                return View(sinav);
            }

            var kullanilanSalonlar = await _context.SinavSalonlari
                .Include(x => x.Sinav)
                .Where(x =>
                    x.Sinav != null &&
                    x.Sinav.Tarih == sinav.Tarih &&
                    x.Sinav.OturumID == sinav.OturumID)
                .Select(x => x.DerslikID)
                .ToListAsync();

            var uygunDerslikler = await _context.Derslikler
                .Where(d => d.Aktif && !kullanilanSalonlar.Contains(d.DerslikID))
                .OrderByDescending(d => d.Kapasite)
                .ToListAsync();

            int kalan = ders.OgrenciSayisi;
            List<Derslik> secilenDerslikler = new();

            foreach (var derslik in uygunDerslikler)
            {
                if (kalan <= 0)
                    break;

                secilenDerslikler.Add(derslik);
                kalan -= derslik.Kapasite;
            }

            if (kalan > 0)
            {
                ModelState.AddModelError("", "Yeterli kapasitede boş derslik bulunamadı.");
                ViewBag.Dersler = new SelectList(_context.Dersler, "DersID", "DersAdi", sinav.DersID);
                ViewBag.Oturumlar = new SelectList(_context.Oturumlar, "OturumID", "Tanim", sinav.OturumID);
                return View(sinav);
            }

            _context.Sinavlar.Add(sinav);
            await _context.SaveChangesAsync();

            foreach (var derslik in secilenDerslikler)
            {
                _context.SinavSalonlari.Add(new SinavSalonu
                {
                    SinavID = sinav.SinavID,
                    DerslikID = derslik.DerslikID
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Program));
        }
    }
}