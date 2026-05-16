using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models;

namespace OBS_Projesi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var personeller = await _context.Personeller
                .Include(p => p.Bolum)
                .OrderBy(p => p.Bolum.BolumAdi)
                .ThenBy(p => p.Soyad)
                .ThenBy(p => p.Ad)
                .ToListAsync();

            return View(personeller);
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            await BolumListesiniYukle();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle([Bind("Unvan,Ad,Soyad,BolumID")] Personel personel)
        {
            ModelState.Remove("Bolum");

            if (!ModelState.IsValid)
            {
                await BolumListesiniYukle(personel.BolumID);
                return View(personel);
            }

            _context.Personeller.Add(personel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personel başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle(int id)
        {
            var personel = await _context.Personeller.FindAsync(id);

            if (personel == null)
            {
                TempData["ErrorMessage"] = "Düzenlenecek personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await BolumListesiniYukle(personel.BolumID);
            return View(personel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("PersonelID,Unvan,Ad,Soyad,BolumID")] Personel personel)
        {
            if (id != personel.PersonelID)
            {
                return NotFound();
            }

            ModelState.Remove("Bolum");

            if (!ModelState.IsValid)
            {
                await BolumListesiniYukle(personel.BolumID);
                return View(personel);
            }

            var mevcutPersonel = await _context.Personeller.FindAsync(id);

            if (mevcutPersonel == null)
            {
                TempData["ErrorMessage"] = "Güncellenecek personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            mevcutPersonel.Unvan = personel.Unvan;
            mevcutPersonel.Ad = personel.Ad;
            mevcutPersonel.Soyad = personel.Soyad;
            mevcutPersonel.BolumID = personel.BolumID;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Mazeretler(int id)
        {
            var personel = await _context.Personeller
                .Include(p => p.Bolum)
                .FirstOrDefaultAsync(p => p.PersonelID == id);

            if (personel == null)
            {
                TempData["ErrorMessage"] = "Personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var mazeretler = await _context.PersonelMazeretleri
                .Include(m => m.Oturum)
                .Where(m => m.PersonelID == id)
                .OrderBy(m => m.Tarih)
                .ThenBy(m => m.Oturum.BaslangicSaat)
                .ToListAsync();

            ViewBag.Personel = personel;
            ViewBag.Oturumlar = new SelectList(
                await _context.Oturumlar.OrderBy(o => o.BaslangicSaat).ToListAsync(),
                "OturumID",
                "Tanim"
            );

            return View(mazeretler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MazeretEkle(
            int personelId,
            DateTime tarih,
            int oturumId,
            string mazeretTuru)
        {
            var personelVarMi = await _context.Personeller.AnyAsync(p => p.PersonelID == personelId);

            if (!personelVarMi)
            {
                TempData["ErrorMessage"] = "Personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool ayniMazeretVarMi = await _context.PersonelMazeretleri.AnyAsync(m =>
                m.PersonelID == personelId &&
                m.Tarih.Date == tarih.Date &&
                m.OturumID == oturumId);

            if (ayniMazeretVarMi)
            {
                TempData["ErrorMessage"] = "Bu personel için aynı tarih ve oturumda zaten mazeret kaydı var.";
                return RedirectToAction(nameof(Mazeretler), new { id = personelId });
            }

            var mazeret = new PersonelMazeret
            {
                PersonelID = personelId,
                Tarih = tarih.Date,
                OturumID = oturumId,
                MazeretTuru = string.IsNullOrWhiteSpace(mazeretTuru) ? "Mazeret" : mazeretTuru.Trim(),
                UygunMu = false
            };

            _context.PersonelMazeretleri.Add(mazeret);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personel mazereti başarıyla eklendi.";
            return RedirectToAction(nameof(Mazeretler), new { id = personelId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MazeretSil(int id)
        {
            var mazeret = await _context.PersonelMazeretleri.FindAsync(id);

            if (mazeret == null)
            {
                TempData["ErrorMessage"] = "Silinecek mazeret bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            int personelId = mazeret.PersonelID;

            _context.PersonelMazeretleri.Remove(mazeret);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mazeret başarıyla silindi.";
            return RedirectToAction(nameof(Mazeretler), new { id = personelId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var personel = await _context.Personeller.FindAsync(id);

            if (personel == null)
            {
                TempData["ErrorMessage"] = "Silinecek personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            bool gorevAtamasiVarMi = await _context.GozetmenAtamalari
                .AnyAsync(g => g.PersonelID == id);

            bool kullaniciyaBagliMi = await _context.Kullanicilar
                .AnyAsync(k => k.PersonelID == id);

            if (gorevAtamasiVarMi || kullaniciyaBagliMi)
            {
                TempData["ErrorMessage"] = "Bu personele bağlı gözetmen görevi veya kullanıcı hesabı olduğu için personel silinemez.";
                return RedirectToAction(nameof(Index));
            }

            var mazeretler = await _context.PersonelMazeretleri
                .Where(m => m.PersonelID == id)
                .ToListAsync();

            _context.PersonelMazeretleri.RemoveRange(mazeretler);
            _context.Personeller.Remove(personel);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Personel başarıyla silindi.";
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