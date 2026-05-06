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
        public DbSet<Derslik> Derslikler { get; set; }
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<GozetmenAtama> GozetmenAtamalari { get; set; }
        public DbSet<PersonelMazeret> PersonelMazeretleri { get; set; }
        public DbSet<SinavLog> SinavLoglar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EF Core'a Sinav tablosunda trigger olduðunu söylüyoruz.
            // Aksi halde SQL Server "OUTPUT clause + trigger" hatasý veriyor.
            modelBuilder.Entity<Sinav>()
                .ToTable("Sinav", tb => tb.HasTrigger("trg_SinavGuncellemeLog"));

            // EF Core'a SinavSalonu tablosunda trigger olduðunu söylüyoruz.
            // Aksi halde SQL Server "OUTPUT clause + trigger" hatasý veriyor.
            modelBuilder.Entity<SinavSalonu>()
                .ToTable("SinavSalonu", tb => tb.HasTrigger("trg_SalonCakismaEngelle"));

            // SinavLog ile Sinav arasýndaki silme döngüsünü kýrýyoruz.
            modelBuilder.Entity<SinavLog>()
                .HasOne(l => l.Sinav)
                .WithMany()
                .HasForeignKey(l => l.SinavID)
                .OnDelete(DeleteBehavior.NoAction);

            // SinavLog ile Personel arasýndaki silme döngüsünü kýrýyoruz.
            modelBuilder.Entity<SinavLog>()
                .HasOne(l => l.DegistirenPersonel)
                .WithMany()
                .HasForeignKey(l => l.PersonelID)
                .OnDelete(DeleteBehavior.NoAction);

            // GozetmenAtama ile SinavSalonu arasýndaki silme döngüsünü kýrýyoruz.
            modelBuilder.Entity<GozetmenAtama>()
                .HasOne(ga => ga.SinavSalonu)
                .WithMany(ss => ss.GozetmenAtamalari)
                .HasForeignKey(ga => ga.SinavSalonuID)
                .OnDelete(DeleteBehavior.NoAction);

            // GozetmenAtama ile Personel arasýndaki silme döngüsünü kýrýyoruz.
            modelBuilder.Entity<GozetmenAtama>()
                .HasOne(ga => ga.Personel)
                .WithMany()
                .HasForeignKey(ga => ga.PersonelID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}