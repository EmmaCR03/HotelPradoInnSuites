using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Reservas
{
    public class ReservasDTO
    {
        [Key]
        public int IdReserva {  get; set; }
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public string EstadoReserva { get; set; }
        public Decimal MontoTotal { get; set; }
    }
}