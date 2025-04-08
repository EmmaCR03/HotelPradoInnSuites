using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Reservas
{
    public class ReservasDTO
    {
        [Key]
        public int IdReserva { get; set; }
        public string IdCliente { get; set; }
        public int cantidadPersonas { get; set; }
        public string NombreCliente { get; set; }
        public int IdHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string EstadoReserva { get; set; }
        public Decimal MontoTotal { get; set; }
        public string NumeroEmpresa { get; set; }
        public string CorreoEmpresa { get; set; }


    }
}