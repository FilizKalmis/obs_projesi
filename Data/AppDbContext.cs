using Microsoft.EntityFrameworkCore;
using OBS_Projesi.Models;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DbSet<Derslik> Derslikler { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<GozetmenAtama> GozetmenAtamalari { get; set; }
        public DbSet<PersonelMazeret> PersonelMazeretleri { get; set; }
        public DbSet<SinavLog> SinavLoglar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SinavLog ile Sinav arasýndaki silme döngüsünü kýrýyoruz
            modelBuilder.Entity<SinavLog>()
                .HasOne(l => l.Sinav)
                .WithMany()
                .HasForeignKey(l => l.SinavID)
                .OnDelete(DeleteBehavior.NoAction); // Döngüyü engellemek için NoAction yaptýk

            // SinavLog ile Personel arasýndaki silme döngüsünü de garantiye alalým
            modelBuilder.Entity<SinavLog>()
                .HasOne(l => l.DegistirenPersonel)
                .WithMany()
                .HasForeignKey(l => l.PersonelID)
                .OnDelete(DeleteBehavior.NoAction);

            // GozetmenAtama silme döngüsünü kýrýyoruz
            modelBuilder.Entity<GozetmenAtama>()
                .HasOne(ga => ga.SinavSalonu)
                .WithMany(ss => ss.GozetmenAtamalari) // Ýliþki tanýmý
                .HasForeignKey(ga => ga.SinavSalonuID)
                .OnDelete(DeleteBehavior.NoAction); // Döngü hatasýný bu satýr çözer[cite: 1]

            modelBuilder.Entity<GozetmenAtama>()
                .HasOne(ga => ga.Personel)
                .WithMany()
                .HasForeignKey(ga => ga.PersonelID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}