namespace TrabajoProyecto.Models
{
    public class Socio 
    {
        public int SocioId { get; set; }
        public int ClubId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAsociado { get; set; }
        public int Dni {  get; set; }
        public int CantidadAsistencias { get; set; }

    }
}
