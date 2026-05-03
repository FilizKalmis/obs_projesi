using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Kullanici")]
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        [Required]
        [StringLength(255)] // Hashlenmiş şifreler için uzun tutuyoruz
        public string Sifre { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } // "Admin" veya "Viewer"

        // Hangi personele ait?
        public int PersonelID { get; set; }

        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }
    }
}