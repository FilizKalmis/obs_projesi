namespace OBS_Projesi.Models.ViewModels
{
    public class SpUdfPanelViewModel
    {
        public int? SinavID { get; set; }
        public int? SinavSalonuID { get; set; }

        public int? Yariyil { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Today;
        public int? OturumID { get; set; }

        public bool? KapasiteYeterliMi { get; set; }
        public bool? YariyilCakismaVarMi { get; set; }

        public string? SonucMesaji { get; set; }
    }
}
