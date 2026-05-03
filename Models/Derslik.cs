using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Derslik")]
    public class Derslik
    {
        // Birincil anahtar
        [Key]

        public int DerslikID { get; set; }

        [Required(ErrorMessage = "Derslik adı zorunludur.")]
        [StringLength(50)] // XSS riskine karşı girdi uzunluğunu sınırlıyoruz
        [Display(Name = "Derslik Adı")]

        // Derslik adı: Amfi-1, Z-04, Lab-1 gibi
        public string Ad { get; set; }

        // Dersliğin öğrenci kapasitesi
        [Range(1, 500, ErrorMessage = "Kapasite 1-500 arasında olmalıdır.")]
        public int Kapasite { get; set; }

        // Derslik tipi: Amfi, Sınıf, Lab gibi
        [Required]
        public string Tip { get; set; }

        // Derslik aktif mi? Kullanım dışı salonları pasif yapmak için
        public bool Aktif { get; set; }

        // Aynı katta salon seçimi için kullanılır: Zemin, 1, 2 gibi
        public string Kat { get; set; }

        // Bir derslik farklı sınavlarda kullanılabilir
        public ICollection<SinavSalonu> SinavSalonlari { get; set; }
    }
}