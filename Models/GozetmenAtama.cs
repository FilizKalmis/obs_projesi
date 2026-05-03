using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("GozetmenAtama")]
    public class GozetmenAtama
    {
        [Key]
        public int AtamaID { get; set; } // Diyagrama göre PK

        public int SinavSalonuID { get; set; } // Hangi sınav ve salon birleşimi?
        public int PersonelID { get; set; }    // Hangi hoca?

        [ForeignKey("SinavSalonuID")]
        public virtual SinavSalonu SinavSalonu { get; set; }

        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }
    }
}