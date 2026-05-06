using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("Oturum")]
    public class Oturum
    {
        [Key]
        public int OturumID { get; set; }

        [Required(ErrorMessage = "Oturum tanımı zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Oturum Tanımı")]
        public string Tanim { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan BaslangicSaat { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan BitisSaat { get; set; }

        [ValidateNever]
        public ICollection<Sinav> Sinavlar { get; set; } = new List<Sinav>();
    }
}