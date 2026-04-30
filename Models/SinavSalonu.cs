namespace OBS_Projesi.Models
{
    public class SinavSalonu
    {
        // Birincil anahtar
        public int AtamaID { get; set; }

        // Foreign Key: Hangi sınav?
        public int SinavID { get; set; }

        // Foreign Key: Hangi derslik/salon?
        public int DerslikID { get; set; }

        // Navigation Property: Sınav bilgisi
        public Sinav Sinav { get; set; }

        // Navigation Property: Derslik bilgisi
        public Derslik Derslik { get; set; }
    }
}