using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("Sinav")]
    public class Sinav
    {
        [Key]
        public int SinavID { get; set; }

        [Required(ErrorMessage = "Ders seçimi zorunludur.")]
        [Display(Name = "Ders")]
        public int DersID { get; set; }

        [Required(ErrorMessage = "Sınav tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Sınav Tarihi")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Oturum seçimi zorunludur.")]
        [Display(Name = "Oturum")]
        public int OturumID { get; set; }

        [ValidateNever]
        public Ders Ders { get; set; } = null!;

        [ValidateNever]
        public Oturum Oturum { get; set; } = null!;

        [ValidateNever]
        public ICollection<SinavSalonu> SinavSalonlari { get; set; } = new List<SinavSalonu>();
    }
}