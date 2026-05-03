using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // PasswordHasher için
using Microsoft.AspNetCore.Mvc;
using OBS_Projesi.Data;
using OBS_Projesi.Models;
using OBS_Projesi.Models.ViewModels;
using System.Security.Claims;

namespace OBS_Projesi.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // 1. Kullanıcıyı sadece kullanıcı adıyla bul
            var user = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == model.KullaniciAdi);

            if (user != null)
            {
                // 2. Hashlenmiş şifreyi doğrula (Siber Güvenlik Katmanı)
                var hasher = new PasswordHasher<Kullanici>();
                var result = hasher.VerifyHashedPassword(user, user.Sifre, model.Sifre);

                if (result == PasswordVerificationResult.Success)
                {
                    // Şifre doğruysa Claim'leri oluştur ve giriş yap
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.KullaniciAdi),
                new Claim(ClaimTypes.Role, user.Rol),
                new Claim("PersonelID", user.PersonelID.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre!");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Personeller = _context.Personeller.ToList(); // Dropdown dolması için şart
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kullanıcı adı daha önce alınmış mı?
                if (_context.Kullanicilar.Any(u => u.KullaniciAdi == model.KullaniciAdi))
                {
                    ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }

                // 2. Şifreyi Hash'leme 
                var hasher = new PasswordHasher<Kullanici>();
                string hashedPass = hasher.HashPassword(null, model.Sifre);

                // 3. Yeni Kullanıcı Nesnesi
                var yeniKullanici = new Kullanici
                {
                    KullaniciAdi = model.KullaniciAdi,
                    Sifre = hashedPass,
                    Rol = "Viewer", // Yeni kayıt olanlar varsayılan olarak kısıtlı yetkiyle başlar
                    PersonelID = model.PersonelID
                };

                _context.Kullanicilar.Add(yeniKullanici);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            ViewBag.Personeller = _context.Personeller.ToList();
            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
