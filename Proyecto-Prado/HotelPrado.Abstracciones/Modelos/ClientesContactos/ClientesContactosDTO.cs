using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.ClientesContactos
{
    public class ClientesContactosDTO
    {
        [Key]
        public int IdContacto { get; set; }
        public int IdCliente { get; set; }
        public string TipoContacto { get; set; }
        public string ValorContacto { get; set; }
        public int EsPrincipal { get; set; }
        public DateTime FechaRegistro { get; set; }


    }
}
