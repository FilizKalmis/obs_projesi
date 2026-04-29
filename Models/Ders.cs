using System.Collections.Generic;

namespace OBS_Projesi.Models
{
    public class Ders
    {
        // Birincil anahtar
        public int DersID { get; set; }

        // Ders kodu: Örn. YZM 2126
        public string DersKodu { get; set; }

        // Ders türü: Zorunlu / Seçmeli
        public string DersTuru { get; set; }

        // Ders adı: Örn. Veritabanı Sistemlerine Giriş
        public string DersAdi { get; set; }

        // Dersi alan öğrenci sayısı
        public int OgrenciSayisi { get; set; }

        // Dersin ait olduğu yarıyıl: 1, 2, 3...
        // Aynı yarıyıldaki zorunlu dersler aynı oturuma konulmamalı
        public int Yariyil { get; set; }

        // Foreign Key: Bu ders hangi bölüme ait?
        public int BolumID { get; set; }

        // Navigation Property: Dersin bağlı olduğu bölüm
        public Bolum Bolum { get; set; }

        // Bir dersin birden fazla sınavı olabilir
        public ICollection<Sinav> Sinavlar { get; set; }
    }
}