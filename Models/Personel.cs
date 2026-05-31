using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Personel")]
    public class Personel
    {
        [Key]
        public int PersonelID { get; set; }

        [Required(ErrorMessage = "Unvan seçimi zorunludur.")]
        [StringLength(20)]
        public string Unvan { get; set; } // Prof. Dr., Doç. Dr., Arş. Gör. vb.

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)] // XSS koruması için
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string Soyad { get; set; }

        // Bölüm ile ilişki (Foreign Key)
        [Required(ErrorMessage = "Bağlı olunan bölüm seçilmelidir.")]
        public int BolumID { get; set; }

        [ForeignKey("BolumID")]
        public virtual Bolum Bolum { get; set; } // obs_projesi-dev'deki Bolum.cs ile ilişki
    }
}
