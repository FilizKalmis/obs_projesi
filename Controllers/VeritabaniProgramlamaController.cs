using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models.ViewModels;
using System.Data;
using System.Data.Common;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class VeritabaniProgramlamaController : Controller
    {
        private readonly AppDbContext _context;

        public VeritabaniProgramlamaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await FormListeleriniYukle();

            var model = new SpUdfPanelViewModel
            {
                Tarih = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AkilliSalonAta(int sinavID)
        {
            if (sinavID <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir sınav seçiniz.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await ExecuteNonQueryAsync(
                    "EXEC sp_AkilliSalonAta @SinavID",
                    new Dictionary<string, object>
                    {
                        { "@SinavID", sinavID }
                    }
                );

                TempData["SuccessMessage"] = "sp_AkilliSalonAta Stored Procedure başarıyla çağrıldı. Seçilen sınav için salon atama işlemi çalıştırıldı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"sp_AkilliSalonAta çalıştırılırken hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GozetmenHavuzundanAta(int sinavSalonuID)
        {
            if (sinavSalonuID <= 0)
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir sınav salonu seçiniz.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await ExecuteNonQueryAsync(
                    "EXEC sp_GozetmenHavuzundanAta @SinavSalonuID",
                    new Dictionary<string, object>
                    {
                        { "@SinavSalonuID", sinavSalonuID }
                    }
                );

                TempData["SuccessMessage"] = "sp_GozetmenHavuzundanAta Stored Procedure başarıyla çağrıldı. Seçilen sınav salonuna havuzdan gözetmen atama işlemi çalıştırıldı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"sp_GozetmenHavuzundanAta çalıştırılırken hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KapasiteKontrol(SpUdfPanelViewModel model)
        {
            await FormListeleriniYukle();

            if (model.SinavID == null || model.SinavID <= 0)
            {
                ModelState.AddModelError("", "Kapasite kontrolü için sınav seçiniz.");
                return View("Index", model);
            }

            try
            {
                var sonuc = await ExecuteScalarAsync(
                    "SELECT dbo.fn_KapasiteKontrol(@SinavID)",
                    new Dictionary<string, object>
                    {
                        { "@SinavID", model.SinavID.Value }
                    }
                );

                model.KapasiteYeterliMi = Convert.ToBoolean(sonuc);
                model.SonucMesaji = model.KapasiteYeterliMi == true
                    ? "fn_KapasiteKontrol sonucu: Kapasite yeterli."
                    : "fn_KapasiteKontrol sonucu: Kapasite yetersiz.";

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"fn_KapasiteKontrol çalıştırılırken hata oluştu: {ex.Message}");
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YariyilCakismaKontrol(SpUdfPanelViewModel model)
        {
            await FormListeleriniYukle();

            if (model.Yariyil == null || model.Yariyil <= 0)
            {
                ModelState.AddModelError("", "Yarıyıl bilgisi zorunludur.");
                return View("Index", model);
            }

            if (model.OturumID == null || model.OturumID <= 0)
            {
                ModelState.AddModelError("", "Oturum seçimi zorunludur.");
                return View("Index", model);
            }

            try
            {
                var sonuc = await ExecuteScalarAsync(
                    "SELECT dbo.fn_YariyilCakismaKontrol(@Yariyil, @Tarih, @OturumID)",
                    new Dictionary<string, object>
                    {
                        { "@Yariyil", model.Yariyil.Value },
                        { "@Tarih", model.Tarih.Date },
                        { "@OturumID", model.OturumID.Value }
                    }
                );

                model.YariyilCakismaVarMi = Convert.ToBoolean(sonuc);
                model.SonucMesaji = model.YariyilCakismaVarMi == true
                    ? "fn_YariyilCakismaKontrol sonucu: Bu yarıyıl için seçilen tarih ve oturumda çakışma var."
                    : "fn_YariyilCakismaKontrol sonucu: Bu yarıyıl için seçilen tarih ve oturumda çakışma yok.";

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"fn_YariyilCakismaKontrol çalıştırılırken hata oluştu: {ex.Message}");
                return View("Index", model);
            }
        }

        private async Task ExecuteNonQueryAsync(string commandText, Dictionary<string, object> parameters)
        {
            var connection = _context.Database.GetDbConnection();
            var baglantiBizActik = connection.State != ConnectionState.Open;

            if (baglantiBizActik)
            {
                await connection.OpenAsync();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = commandText;

                ParametreleriEkle(command, parameters);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (baglantiBizActik)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<object?> ExecuteScalarAsync(string commandText, Dictionary<string, object> parameters)
        {
            var connection = _context.Database.GetDbConnection();
            var baglantiBizActik = connection.State != ConnectionState.Open;

            if (baglantiBizActik)
            {
                await connection.OpenAsync();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = commandText;

                ParametreleriEkle(command, parameters);

                return await command.ExecuteScalarAsync();
            }
            finally
            {
                if (baglantiBizActik)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private static void ParametreleriEkle(DbCommand command, Dictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = item.Key;
                parameter.Value = item.Value ?? DBNull.Value;

                command.Parameters.Add(parameter);
            }
        }

        private async Task FormListeleriniYukle()
        {
            var sinavlar = await _context.Sinavlar
                .Include(s => s.Ders)
                .Include(s => s.Oturum)
                .OrderByDescending(s => s.Tarih)
                .ThenBy(s => s.Oturum.BaslangicSaat)
                .ToListAsync();

            ViewBag.Sinavlar = sinavlar
                .Select(s => new SelectListItem
                {
                    Value = s.SinavID.ToString(),
                    Text = $"{s.SinavID} - {s.Ders?.DersKodu} {s.Ders?.DersAdi} / {s.Tarih:dd.MM.yyyy} / {s.Oturum?.Tanim}"
                })
                .ToList();

            var sinavSalonlari = await _context.SinavSalonlari
                .Include(ss => ss.Sinav)
                    .ThenInclude(s => s.Ders)
                .Include(ss => ss.Derslik)
                .OrderByDescending(ss => ss.Sinav.Tarih)
                .ToListAsync();

            ViewBag.SinavSalonlari = sinavSalonlari
                .Select(ss => new SelectListItem
                {
                    Value = ss.SinavSalonuID.ToString(),
                    Text = $"{ss.SinavSalonuID} - {ss.Sinav?.Ders?.DersKodu} / {ss.Sinav?.Tarih:dd.MM.yyyy} / {ss.Derslik?.Ad}"
                })
                .ToList();

            var oturumlar = await _context.Oturumlar
                .OrderBy(o => o.BaslangicSaat)
                .ToListAsync();

            ViewBag.Oturumlar = oturumlar
                .Select(o => new SelectListItem
                {
                    Value = o.OturumID.ToString(),
                    Text = $"{o.Tanim} ({o.BaslangicSaat:hh\\:mm} - {o.BitisSaat:hh\\:mm})"
                })
                .ToList();
        }
    }
}