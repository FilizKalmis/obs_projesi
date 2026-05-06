using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Kullanicilar
                .FirstOrDefault(u => u.KullaniciAdi == model.KullaniciAdi);

            if (user != null)
            {
                var hasher = new PasswordHasher<Kullanici>();
                var result = hasher.VerifyHashedPassword(user, user.Sifre, model.Sifre);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.KullaniciAdi),
                        new Claim(ClaimTypes.Role, user.Rol)
                    };

                    if (user.PersonelID.HasValue)
                    {
                        claims.Add(new Claim("PersonelID", user.PersonelID.Value.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Hatalı kullanıcı adı veya şifre!");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Kullanicilar.Any(u => u.KullaniciAdi == model.KullaniciAdi))
            {
                ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = model.KullaniciAdi,
                Rol = "Viewer",
                PersonelID = null
            };

            var hasher = new PasswordHasher<Kullanici>();
            yeniKullanici.Sifre = hasher.HashPassword(yeniKullanici, model.Sifre);

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}