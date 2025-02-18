using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cliente;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas
{
    [Table("Reservas")]
    public class ReservasTabla
    {
        [Key]
        public int IdReserva { get; set; }
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string EstadoReserva { get; set; }
        public Decimal MontoTotal { get; set; }

        [ForeignKey("IdHabitacion")]
        public virtual HabitacionesTabla Habitacion { get; set; }
        [ForeignKey("IdCliente")]
        public virtual ClienteTabla Cliente { get; set; }
    }
}
