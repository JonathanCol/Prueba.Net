namespace RegistroUsuarios.Domain
{
    public class Persona
    {
        public string DocumentoIdentidad { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public List<string> Telefonos { get; set; } = new List<string>();
        public List<string> Correos { get; set; } = new List<string>();
        public List<string> Direcciones { get; set; } = new List<string>();
    }
}
