using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("SinavSalonu")]
    public class SinavSalonu
    {
        [Key]
        public int SinavSalonuID { get; set; } // Diyagrama göre PK

        public int SinavID { get; set; }
        public int DerslikID { get; set; }

        [ForeignKey("SinavID")]
        public virtual Sinav Sinav { get; set; }

        [ForeignKey("DerslikID")]
        public virtual Derslik Derslik { get; set; }

        // Bu salon atamasına yapılacak gözetmen atamaları için ilişki
        public virtual ICollection<GozetmenAtama> GozetmenAtamalari { get; set; }
    }
}