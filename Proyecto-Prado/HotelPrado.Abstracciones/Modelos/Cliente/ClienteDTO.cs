using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Cliente
{
    public class ClienteDTO
    {
        [Key]
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string PrimerApellidoCliente { get; set; }
        public string SegundoApellidoCLiente { get; set; }
        public string EmailCliente { get; set; }
        public int TelefonoCliente { get; set; }
        public string DireccionCliente { get; set; }

    }
}
