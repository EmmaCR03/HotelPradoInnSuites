using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cliente;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas
{
    [Table("Reservas")]
    public class ReservasTabla
    {
        [Key]
        public int IdReserva { get; set; }

        public string  IdCliente { get; set; }  

        public string NombreCliente { get; set; }
        public int cantidadPersonas { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string EstadoReserva { get; set; }
        public Decimal MontoTotal { get; set; }

        [ForeignKey("IdHabitacion")]
        public virtual HabitacionesTabla Habitacion { get; set; }

        [ForeignKey("IdCliente")]
        public virtual ApplicationUser Cliente { get; set; }  
    }
}