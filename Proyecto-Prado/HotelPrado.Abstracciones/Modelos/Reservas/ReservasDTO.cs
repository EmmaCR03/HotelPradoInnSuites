using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Reservas
{
    public class ReservasDTO
    {
        [Key]
        public int IdReserva { get; set; }
        [Display(Name = "ID Cliente")]
        public string IdCliente { get; set; }
        [Display(Name = "Cantidad de personas")]
        public int cantidadPersonas { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }
        [Display(Name = "ID Habitación")]
        public int IdHabitacion { get; set; }
        [Display(Name = "Número de habitación")]
        public int NumeroHabitacion { get; set; }
        [Display(Name = "Fecha de inicio")]
        public DateTime? FechaInicio { get; set; }
        [Display(Name = "Fecha final")]
        public DateTime? FechaFinal { get; set; }
        [Display(Name = "Estado de la reserva")]
        public string EstadoReserva { get; set; }
        [Display(Name = "Monto total")]
        public Decimal MontoTotal { get; set; }
        [Display(Name = "Teléfono de la empresa")]
        public string NumeroEmpresa { get; set; }
        [Display(Name = "Correo electrónico de la empresa")]
        public string CorreoEmpresa { get; set; }
    }
}