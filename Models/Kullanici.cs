using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("Kullanici")]
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Sifre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = string.Empty; // Admin veya Viewer

        // Kullanıcı her zaman personele bağlı olmak zorunda değil.
        public int? PersonelID { get; set; }

        [ValidateNever]
        [ForeignKey("PersonelID")]
        public virtual Personel? Personel { get; set; }
    }
}