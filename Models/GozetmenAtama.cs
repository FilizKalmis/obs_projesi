using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("GozetmenAtama")]
    public class GozetmenAtama
    {
        [Key]
        public int AtamaID { get; set; }

        [Required(ErrorMessage = "Sınav salonu seçimi zorunludur.")]
        [Display(Name = "Sınav Salonu")]
        public int SinavSalonuID { get; set; }

        [Required(ErrorMessage = "Personel seçimi zorunludur.")]
        [Display(Name = "Gözetmen")]
        public int PersonelID { get; set; }

        [ForeignKey("SinavSalonuID")]
        [ValidateNever]
        public SinavSalonu SinavSalonu { get; set; } = null!;

        [ForeignKey("PersonelID")]
        [ValidateNever]
        public Personel Personel { get; set; } = null!;
    }
}