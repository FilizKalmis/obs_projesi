using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OBS_Projesi.Models
{
    [Table("Ders")]
    public class Ders
    {
        [Key]
        public int DersID { get; set; }

        [Required(ErrorMessage = "Ders kodu zorunludur.")]
        [StringLength(20)]
        [Display(Name = "Ders Kodu")]
        public string DersKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ders Adı")]
        public string DersAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ders türü zorunludur.")]
        [StringLength(30)]
        [Display(Name = "Ders Türü")]
        public string DersTuru { get; set; } = string.Empty;

        [Required(ErrorMessage = "Öğrenci sayısı zorunludur.")]
        [Range(1, 500, ErrorMessage = "Öğrenci sayısı 1-500 arasında olmalıdır.")]
        [Display(Name = "Öğrenci Sayısı")]
        public int OgrenciSayisi { get; set; }

        [Required(ErrorMessage = "Yarıyıl bilgisi zorunludur.")]
        [Range(1, 8, ErrorMessage = "Yarıyıl 1-8 arasında olmalıdır.")]
        [Display(Name = "Yarıyıl")]
        public int Yariyil { get; set; }

        [Required(ErrorMessage = "Bölüm seçimi zorunludur.")]
        [Display(Name = "Bölüm")]
        public int BolumID { get; set; }

        [ValidateNever]
        [ForeignKey("BolumID")]
        public Bolum Bolum { get; set; } = null!;

        [ValidateNever]
        public ICollection<Sinav> Sinavlar { get; set; } = new List<Sinav>();
    }
}