using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("Derslik")]
    public class Derslik
    {
        [Key]
        public int DerslikID { get; set; }

        [Required(ErrorMessage = "Derslik adı zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Derslik Adı")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kapasite zorunludur.")]
        [Range(1, 500, ErrorMessage = "Kapasite 1-500 arasında olmalıdır.")]
        public int Kapasite { get; set; }

        [Required(ErrorMessage = "Derslik tipi zorunludur.")]
        [Display(Name = "Derslik Tipi")]
        public string Tip { get; set; } = string.Empty;

        public bool Aktif { get; set; }

        [Required(ErrorMessage = "Kat bilgisi zorunludur.")]
        [StringLength(20)]
        [Display(Name = "Kat")]
        public string Kat { get; set; } = string.Empty;

        [ValidateNever]
        public ICollection<SinavSalonu> SinavSalonlari { get; set; } = new List<SinavSalonu>();
    }
}