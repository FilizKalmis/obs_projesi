using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Sinav")]
    public class Sinav
    {
        [Key]
        public int SinavID { get; set; }

        public int DersID { get; set; }

        public DateTime Tarih { get; set; }

        public int OturumID { get; set; }

        public Ders? Ders { get; set; }

        public Oturum? Oturum { get; set; }

        public ICollection<SinavSalonu>? SinavSalonlari { get; set; }
    }
}