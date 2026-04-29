using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Models;

namespace OBS_Projesi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bolum> Bolumler { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<Oturum> Oturumlar { get; set; }
        public DbSet<Sinav> Sinavlar { get; set; }
        public DbSet<SinavSalonu> SinavSalonlari { get; set; }
    }
}