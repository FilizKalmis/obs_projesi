using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("SinavLog")]
    public class SinavLog
    {
        [Key]
        public int LogID { get; set; }

        // Hangi sınav değişti?
        [Required]
        public int SinavID { get; set; }

        // İşlemi yapan personel (Yönetici) kim?
        // Not: Dokümandaki "Admin" ifadesi Personel tablosundaki bir yetkiliye karşılık gelir.
        [Required]
        public int PersonelID { get; set; }

        [Required]
        [StringLength(50)]
        public string IslemTuru { get; set; } // Örn: "Oturum Güncelleme"

        [StringLength(255)]
        public string EskiDeger { get; set; } // Örn: "Oturum 1 (09:00)"

        [StringLength(255)]
        public string YeniDeger { get; set; } // Örn: "Oturum 2 (11:00)"

        [Required]
        public DateTime IslemTarihi { get; set; } // İşlemin yapıldığı an

        // İlişkiler (Foreign Keys)
        [ForeignKey("SinavID")]
        public virtual Sinav Sinav { get; set; }

        [ForeignKey("PersonelID")]
        public virtual Personel DegistirenPersonel { get; set; }
    }
}