using System;

namespace HotelPrado.UI.Models
{
    /// <summary>Una fila del historial: una estancia (check-in) de un cliente. Si TieneReservas=false, es un cliente sin reservas.</summary>
    public class HistorialClienteViewModel
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
        public string CedulaCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string EmailCliente { get; set; }
        /// <summary>Número de estancia (1 = más reciente, 2 = anterior, etc.). 0 si sin reservas.</summary>
        public int NumeroEstancia { get; set; }
        public int IdReserva { get; set; }
        /// <summary>Fecha y hora de entrada de esta estancia.</summary>
        public DateTime? FechaEntrada { get; set; }
        /// <summary>Fecha y hora de salida de esta estancia.</summary>
        public DateTime? FechaSalida { get; set; }
        public int NumeroHabitacion { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal MontoTotal { get; set; }
        public string EstadoReserva { get; set; }
        public bool TieneReservas { get; set; }
    }
}
