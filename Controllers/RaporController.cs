using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Data;
using OBS_Projesi.Models.ViewModels;
using System.Data;
using System.Data.Common;

namespace OBS_Projesi.Controllers
{
    [Authorize]
    public class RaporController : Controller
    {
        private readonly AppDbContext _context;

        public RaporController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SinavProgrami()
        {
            var liste = await SinavProgramiDetaylariniGetir();
            return View(liste);
        }

        public async Task<IActionResult> DerslikDurumu()
        {
            var liste = await DerslikDurumlariniGetir();
            return View(liste);
        }

        public async Task<IActionResult> GozetmenGorevYuku()
        {
            var liste = await GozetmenGorevYukleriniGetir();
            return View(liste);
        }

        private async Task<List<SinavProgramiDetayRaporViewModel>> SinavProgramiDetaylariniGetir()
        {
            var liste = new List<SinavProgramiDetayRaporViewModel>();
            var connection = _context.Database.GetDbConnection();

            var baglantiBizActik = connection.State != ConnectionState.Open;

            if (baglantiBizActik)
            {
                await connection.OpenAsync();
            }

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                    SELECT 
                        Tarih,
                        Oturum,
                        DersAdi,
                        Salon,
                        Gozetmen
                    FROM vw_SinavProgramiDetay
                    ORDER BY Tarih, Oturum, DersAdi, Salon";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    liste.Add(new SinavProgramiDetayRaporViewModel
                    {
                        Tarih = ReadNullableDateTime(reader, "Tarih"),
                        Oturum = ReadString(reader, "Oturum"),
                        DersAdi = ReadString(reader, "DersAdi"),
                        Salon = ReadString(reader, "Salon"),
                        Gozetmen = ReadString(reader, "Gozetmen")
                    });
                }
            }
            finally
            {
                if (baglantiBizActik)
                {
                    await connection.CloseAsync();
                }
            }

            return liste;
        }

        private async Task<List<DerslikDurumuRaporViewModel>> DerslikDurumlariniGetir()
        {
            var liste = new List<DerslikDurumuRaporViewModel>();
            var connection = _context.Database.GetDbConnection();

            var baglantiBizActik = connection.State != ConnectionState.Open;

            if (baglantiBizActik)
            {
                await connection.OpenAsync();
            }

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                    SELECT 
                        Ad,
                        Kapasite,
                        ToplamSinavSayisi
                    FROM vw_DerslikDurumu
                    ORDER BY ToplamSinavSayisi DESC, Ad";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    liste.Add(new DerslikDurumuRaporViewModel
                    {
                        Ad = ReadString(reader, "Ad"),
                        Kapasite = ReadInt(reader, "Kapasite"),
                        ToplamSinavSayisi = ReadInt(reader, "ToplamSinavSayisi")
                    });
                }
            }
            finally
            {
                if (baglantiBizActik)
                {
                    await connection.CloseAsync();
                }
            }

            return liste;
        }

        private async Task<List<GozetmenGorevYukuRaporViewModel>> GozetmenGorevYukleriniGetir()
        {
            var liste = new List<GozetmenGorevYukuRaporViewModel>();
            var connection = _context.Database.GetDbConnection();

            var baglantiBizActik = connection.State != ConnectionState.Open;

            if (baglantiBizActik)
            {
                await connection.OpenAsync();
            }

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                    SELECT 
                        Unvan,
                        Ad,
                        Soyad,
                        GorevSayisi
                    FROM vw_GozetmenGorevYuku
                    ORDER BY GorevSayisi DESC, Ad, Soyad";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    liste.Add(new GozetmenGorevYukuRaporViewModel
                    {
                        Unvan = ReadString(reader, "Unvan"),
                        Ad = ReadString(reader, "Ad"),
                        Soyad = ReadString(reader, "Soyad"),
                        GorevSayisi = ReadInt(reader, "GorevSayisi")
                    });
                }
            }
            finally
            {
                if (baglantiBizActik)
                {
                    await connection.CloseAsync();
                }
            }

            return liste;
        }

        private static string ReadString(DbDataReader reader, string columnName)
        {
            return reader[columnName] == DBNull.Value
                ? "-"
                : reader[columnName]?.ToString() ?? "-";
        }

        private static int ReadInt(DbDataReader reader, string columnName)
        {
            return reader[columnName] == DBNull.Value
                ? 0
                : Convert.ToInt32(reader[columnName]);
        }

        private static DateTime? ReadNullableDateTime(DbDataReader reader, string columnName)
        {
            return reader[columnName] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader[columnName]);
        }
    }
}