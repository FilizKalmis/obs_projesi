using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OBS_Projesi.Data;
using OBS_Projesi.Models;
using System.Linq;


namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class HesapController : Controller
    {
        private readonly AppDbContext _context;

        public HesapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Giris() => View();

        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF Koruması
        public IActionResult Giris(GirisViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. ADIM: Girilen KullaniciAdi ile Personel tablosunda böyle biri var mı?
                var personel = _context.Personeller.FirstOrDefault(p => p.Ad.ToLower() == model.KullaniciAdi.ToLower());

                if (personel != null)
                {
                    // Şimdilik basit şifre kontrolü (123)
                    if (model.Sifre == "123")
                    {
                        // 2. ADIM: Dokümandaki Role-Based Security kurgusu
                        if (model.Rol == "Admin")
                        {
                            // Burada arka planda App_Admin yetkisi atanacak
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Burada arka planda App_Viewer yetkisi atanacak
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            }
            return View(model);
        }
    }
}