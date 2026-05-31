using System.ComponentModel.DataAnnotations;

namespace OBS_Projesi.Models.ViewModels
{
    public class SinavOlusturViewModel
    {
        [Required(ErrorMessage = "Ders seçimi zorunludur.")]
        [Display(Name = "Ders")]
        public int DersID { get; set; }

        [Required(ErrorMessage = "Sınav tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Sınav Tarihi")]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Oturum seçimi zorunludur.")]
        [Display(Name = "Oturum")]
        public int OturumID { get; set; }
    }
}