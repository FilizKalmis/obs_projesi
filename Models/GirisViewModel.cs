using System.ComponentModel.DataAnnotations;

namespace OBS_Projesi.Models
{
    public class GirisViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Giriş türü seçilmelidir.")]
        [Display(Name = "Giriş Türü")]
        public string Rol { get; set; } // "Admin" veya "Gozetmen"
    }
}