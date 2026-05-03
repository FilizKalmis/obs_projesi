using System.Collections.Generic;

namespace OBS_Projesi.Models
{
    public class Derslik
    {
        // Birincil anahtar
        public int DerslikID { get; set; }

        // Derslik adı: Amfi-1, Z-04, Lab-1 gibi
        public string Ad { get; set; }

        // Dersliğin öğrenci kapasitesi
        public int Kapasite { get; set; }

        // Derslik tipi: Amfi, Sınıf, Lab gibi
        public string Tip { get; set; }

        // Derslik aktif mi? Kullanım dışı salonları pasif yapmak için
        public bool Aktif { get; set; }

        // Aynı katta salon seçimi için kullanılır: Zemin, 1, 2 gibi
        public string Kat { get; set; }

        // Bir derslik farklı sınavlarda kullanılabilir
        public ICollection<SinavSalonu> SinavSalonlari { get; set; }
    }
}