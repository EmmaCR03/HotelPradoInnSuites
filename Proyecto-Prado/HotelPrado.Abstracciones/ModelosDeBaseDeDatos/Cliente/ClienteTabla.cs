using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cliente
{
    [Table("Cliente")]

    public class ApplicationUser
    {
        [Key]
        public string IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string PrimerApellidoCliente { get; set; }
        public string SegundoApellidoCLiente { get; set; }
        public string EmailCliente { get; set; }
        public int TelefonoCliente { get; set; }
        public string DireccionCliente { get; set; }
    }
}
