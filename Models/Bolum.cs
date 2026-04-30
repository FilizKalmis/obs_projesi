using System.Collections.Generic;

namespace OBS_Projesi.Models
{
    public class Bolum
    {
        // Birincil anahtar: Her bölümü benzersiz ayırır
        public int BolumID { get; set; }

        // Bölüm adı: Yazılım Mühendisliği, Bilgisayar Mühendisliği vb.
        public string BolumAdi { get; set; }

        // Bir bölümün birden fazla dersi olabilir
        public ICollection<Ders> Dersler { get; set; }

        // Bir bölümde birden fazla personel olabilir
        public ICollection<Personel> Personeller { get; set; }
    }
}
