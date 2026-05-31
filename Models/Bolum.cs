using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Bolum")]
    public class Bolum
    {
        // Birincil anahtar: Her bölümü benzersiz ayırır
        [Key]

        public int BolumID { get; set; }

        // Bölüm adı: Yazılım Mühendisliği, Bilgisayar Mühendisliği vb.
        public string BolumAdi { get; set; }

        // Bir bölümün birden fazla dersi olabilir
        public ICollection<Ders> Dersler { get; set; }

        // Bir bölümde birden fazla personel olabilir
        public ICollection<Personel> Personeller { get; set; }
    }
}
