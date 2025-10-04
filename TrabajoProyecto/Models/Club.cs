namespace TrabajoProyecto.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public string Nombre { get; set; }
        public int CantidadSocios { get; set; }
        public int CantidadTitulos { get; set; }
        public DateTime FechaFundacion { get; set; }
        public string UbicacionEstadio { get; set; }
        public string NombreEstadio { get; set; }

    }
}
