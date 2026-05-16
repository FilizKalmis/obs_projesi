using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("SinavSalonu")]
    public class SinavSalonu
    {
        [Key]
        public int SinavSalonuID { get; set; }

        public int SinavID { get; set; }

        public int DerslikID { get; set; }

        [ForeignKey("SinavID")]
        public virtual Sinav? Sinav { get; set; }

        [ForeignKey("DerslikID")]
        public virtual Derslik? Derslik { get; set; }

        public virtual ICollection<GozetmenAtama> GozetmenAtamalari { get; set; }
            = new List<GozetmenAtama>();
    }
}