using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("PersonelMazeret")]
    public class PersonelMazeret
    {
        [Key]
        public int MazeretID { get; set; }

        public int PersonelID { get; set; }
        public int OturumID { get; set; } // Diyagramda Oturum ile ilişkilendirilmiş

        public DateTime Tarih { get; set; }
        public string MazeretTuru { get; set; }
        public bool UygunMu { get; set; } // Diyagramdaki özellik

        [ForeignKey("PersonelID")]
        public virtual Personel Personel { get; set; }

        [ForeignKey("OturumID")]
        public virtual Oturum Oturum { get; set; }
    }
}