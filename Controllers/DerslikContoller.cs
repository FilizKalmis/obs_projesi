namespace OBS_Projesi.Controllers;
using global::OBS_Projesi.Models;
using Microsoft.AspNetCore.Mvc;


    public class DerslikController : Controller
    {
        // Derslik Listesi (App_Viewer yetkisi için uygun)
        public IActionResult Index()
        {
            // Şimdilik boş bir liste döndürüyoruz
            var derslikler = new List<Derslik>();
            return View(derslikler);
        }

        // Derslik Ekleme Sayfası (GET)
        public IActionResult Ekle()
        {
            return View();
        }

        // Derslik Ekleme (POST) - GÜVENLİK KRİTİK!
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarını engeller
        public IActionResult Ekle(Derslik derslik)
        {
            if (ModelState.IsValid)
            {
                // Veritabanı kayıt işlemleri burada yapılacak
                return RedirectToAction(nameof(Index));
            }
            return View(derslik);
        }
    }
