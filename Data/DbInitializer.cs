using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Models;

namespace OBS_Projesi.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();

            const string adminKullaniciAdi = "admin";
            const string adminSifre = "Admin123!";
            const string adminRol = "Admin";

            var admin = await context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == adminKullaniciAdi);

            var hasher = new PasswordHasher<Kullanici>();

            if (admin == null)
            {
                admin = new Kullanici
                {
                    KullaniciAdi = adminKullaniciAdi,
                    Rol = adminRol,
                    PersonelID = null
                };

                admin.Sifre = hasher.HashPassword(admin, adminSifre);

                context.Kullanicilar.Add(admin);
                await context.SaveChangesAsync();

                return;
            }

            admin.Rol = adminRol;

            var sifreGecerliMi = false;

            try
            {
                var result = hasher.VerifyHashedPassword(admin, admin.Sifre, adminSifre);
                sifreGecerliMi = result == PasswordVerificationResult.Success ||
                                 result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch
            {
                sifreGecerliMi = false;
            }

            if (!sifreGecerliMi)
            {
                admin.Sifre = hasher.HashPassword(admin, adminSifre);
            }

            await context.SaveChangesAsync();
        }
    }
}