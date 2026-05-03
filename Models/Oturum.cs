using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OBS_Projesi.Models
{
    [Table("Oturum")]
    public class Oturum
    {
        // Birincil anahtar
        [Key]
        public int OturumID { get; set; }

        // Oturum adı/açıklaması: Sabah-1, Öğle-1 gibi
        public string Tanim { get; set; }

        // Oturumun başlangıç saati
        public TimeSpan BaslangicSaat { get; set; }

        // Oturumun bitiş saati
        public TimeSpan BitisSaat { get; set; }

        // Bir oturumda birden fazla sınav olabilir
        public ICollection<Sinav> Sinavlar { get; set; }
    }
}