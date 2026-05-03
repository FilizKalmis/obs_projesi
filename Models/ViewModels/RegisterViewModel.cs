using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    public string KullaniciAdi { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Sifre { get; set; }

    [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor")]
    [DataType(DataType.Password)]
    public string SifreTekrar { get; set; }

    [Required(ErrorMessage = "Personel seçimi zorunludur")]
    public int PersonelID { get; set; } // Hangi akademisyen kayıt oluyor?
}