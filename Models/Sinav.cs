using System;
using System.Collections.Generic;

namespace OBS_Projesi.Models
{
    public class Sinav
    {
        // Birincil anahtar
        public int SinavID { get; set; }

        // Foreign Key: Bu sınav hangi derse ait?
        public int DersID { get; set; }

        // Sınavın yapılacağı tarih
        public DateTime Tarih { get; set; }

        // Foreign Key: Bu sınav hangi oturumda yapılacak?
        public int OturumID { get; set; }

        // Navigation Property: Sınavın bağlı olduğu ders
        public Ders Ders { get; set; }

        // Navigation Property: Sınavın yapılacağı oturum
        public Oturum Oturum { get; set; }

        // Bir sınav birden fazla salonda yapılabilir
        public ICollection<SinavSalonu> SinavSalonlari { get; set; }
    }
}