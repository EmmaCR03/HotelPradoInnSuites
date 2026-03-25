using System.Collections.Generic;

namespace HotelPrado.UI.Models
{
    /// <summary>
    /// ViewModel para la vista de calendario de habitaciones
    /// Contiene los datos necesarios para mostrar las reservas en un calendario
    /// </summary>
    public class CalendarioHabitacionesViewModel
    {
        public List<EventoCalendario> Eventos { get; set; }
        public List<HabitacionInfo> Habitaciones { get; set; }
    }

    /// <summary>
    /// Representa un evento en el calendario (reserva)
    /// </summary>
    public class EventoCalendario
    {
        public int IdReserva { get; set; }
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public string Titulo { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string HoraCheckIn { get; set; }
        public string HoraCheckOut { get; set; }
        public string Estado { get; set; }
        public string Color { get; set; }
        public string Cliente { get; set; }
        public int CantidadPersonas { get; set; }
    }

    /// <summary>
    /// Información básica de una habitación
    /// </summary>
    public class HabitacionInfo
    {
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public string Estado { get; set; }
        public int CapacidadMax { get; set; }
    }
}

