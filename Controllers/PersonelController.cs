using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OBS_Projesi.Data;
using OBS_Projesi.Models;
using System.Linq;

namespace OBS_Projesi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PersonelController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection ile DbContext'i alıyoruz (Yeni dev branch'indeki yapına uygun)
        public PersonelController(AppDbContext context)
        {
            _context = context;
        }

        // Personel Listesi
        public IActionResult Index()
        {
            // İleride buraya Include(p => p.Bolum) ekleyeceğiz
            var personeller = _context.Personeller.ToList();
            return View(personeller);
        }

        // GET: Personel Ekleme Formu
        public IActionResult Ekle()
        {
            // Formda Bolum seçtirmek için ViewBag ile verileri View'a taşıyoruz
            ViewBag.Bolumler = new SelectList(_context.Bolumler, "BolumID", "BolumAdi");
            return View();
        }

        // POST: Personel Ekleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken] // GÜVENLİK: CSRF saldırılarını engeller
        public IActionResult Ekle(Personel personel)
        {
            if (ModelState.IsValid)
            {
                _context.Personeller.Add(personel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa bölümleri tekrar yükle ve formu geri döndür
            ViewBag.Bolumler = new SelectList(_context.Bolumler, "BolumID", "BolumAdi", personel.BolumID);
            return View(personel);
        }
    }
}
