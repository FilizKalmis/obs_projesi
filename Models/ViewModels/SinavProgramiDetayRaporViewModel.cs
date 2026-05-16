namespace OBS_Projesi.Models.ViewModels
{
    public class SinavProgramiDetayRaporViewModel
    {
        public DateTime? Tarih { get; set; }
        public string Oturum { get; set; } = "-";
        public string DersAdi { get; set; } = "-";
        public string Salon { get; set; } = "-";
        public string Gozetmen { get; set; } = "-";
    }
}
