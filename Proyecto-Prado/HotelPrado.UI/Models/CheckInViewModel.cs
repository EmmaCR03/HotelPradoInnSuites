using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.UI.Models
{
    public class CheckInViewModel
    {
        [Display(Name = "ID Check-in")]
        public int IdCheckIn { get; set; }
        [Display(Name = "Código folio")]
        public int? CodigoFolio { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }
        [Display(Name = "Cédula del cliente")]
        public string CedulaCliente { get; set; }
        [Display(Name = "Fecha check-in")]
        public DateTime? FechaCheckIn { get; set; }
        [Display(Name = "Fecha check-out")]
        public DateTime? FechaCheckOut { get; set; }
        [Display(Name = "Número de habitación")]
        public string NumeroHabitacion { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }

    public class CheckInDetalleViewModel
    {
        [Display(Name = "ID Check-in")]
        public int? IdCheckIn { get; set; }
        [Display(Name = "Código folio")]
        public int? CodigoFolio { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }
        [Display(Name = "Cédula del cliente")]
        public string CedulaCliente { get; set; }
        [Display(Name = "Teléfono del cliente")]
        public string TelefonoCliente { get; set; }
        [Display(Name = "Dirección del cliente")]
        public string DireccionCliente { get; set; }
        [Display(Name = "Fecha check-in")]
        public DateTime? FechaCheckIn { get; set; }
        [Display(Name = "Fecha check-out")]
        public DateTime? FechaCheckOut { get; set; }
        [Display(Name = "Número de adultos")]
        public int? NumeroAdultos { get; set; }
        [Display(Name = "Número de niños")]
        public int? NumeroNinos { get; set; }
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }
        [Display(Name = "Total")]
        public decimal? Total { get; set; }
        [Display(Name = "Depósito")]
        public decimal? Deposito { get; set; }
        [Display(Name = "Número de habitación")]
        public string NumeroHabitacion { get; set; }
        [Display(Name = "Nombre de empresa")]
        public string NombreEmpresa { get; set; }
        [Display(Name = "Correo electrónico del cliente")]
        public string EmailCliente { get; set; }
    }

    public class CheckInCrearViewModel
    {
        [Display(Name = "ID Reserva")]
        public int? IdReserva { get; set; }
        [Display(Name = "ID Cliente")]
        public int? IdCliente { get; set; }
        [Display(Name = "ID Habitación")]
        public int? IdHabitacion { get; set; }
        [Display(Name = "ID Empresa")]
        public int? IdEmpresa { get; set; }
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }
        [Display(Name = "Cédula del cliente")]
        public string CedulaCliente { get; set; }
        [Display(Name = "Teléfono del cliente")]
        public string TelefonoCliente { get; set; }
        [Display(Name = "Dirección del cliente")]
        public string DireccionCliente { get; set; }
        [Display(Name = "Correo electrónico del cliente")]
        public string EmailCliente { get; set; }
        [Display(Name = "Fecha check-in")]
        public DateTime FechaCheckIn { get; set; }
        [Display(Name = "Fecha check-out")]
        public DateTime FechaCheckOut { get; set; }
        [Display(Name = "Número de adultos")]
        public int NumeroAdultos { get; set; }
        [Display(Name = "Número de niños")]
        public int NumeroNinos { get; set; }
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }
        [Display(Name = "Total")]
        public decimal? Total { get; set; }
        [Display(Name = "Depósito")]
        public decimal? Deposito { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
    }
}

