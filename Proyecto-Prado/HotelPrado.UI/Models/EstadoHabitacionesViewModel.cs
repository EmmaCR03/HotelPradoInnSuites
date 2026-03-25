using System;
using System.Collections.Generic;

namespace HotelPrado.UI.Models
{
    /// <summary>
    /// ViewModel para la vista de estado de habitaciones en tiempo real
    /// Contiene el estado actual de todas las habitaciones y departamentos
    /// </summary>
    public class EstadoHabitacionesViewModel
    {
        public List<EstadoHabitacion> Habitaciones { get; set; }
        public List<EstadoDepartamento> Departamentos { get; set; }
        public EstadisticasResumen Estadisticas { get; set; }
    }

    /// <summary>
    /// Estado detallado de una habitación
    /// </summary>
    public class EstadoHabitacion
    {
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public string Estado { get; set; } // Disponible, Ocupada, Mantenimiento, Limpieza
        public string EstadoReserva { get; set; } // Confirmada, Pendiente, Cancelada
        public string ClienteActual { get; set; }
        public DateTime? FechaCheckIn { get; set; }
        public DateTime? FechaCheckOut { get; set; }
        public int? CantidadPersonas { get; set; }
        public int IdReserva { get; set; }
        public int CapacidadMax { get; set; }
        public string ColorEstado { get; set; }
        public string IconoEstado { get; set; }
        public int DiasRestantes { get; set; }
        public string DescripcionLimpieza { get; set; }
        public DateTime? FechaInicioLimpieza { get; set; }
        public string TiempoOcupada { get; set; } // Tiempo transcurrido desde check-in
        public string TiempoEnLimpieza { get; set; } // Tiempo transcurrido desde inicio de limpieza
        public string TiempoRestanteOcupacion { get; set; } // Tiempo hasta check-out
        /// <summary>Texto para leyenda: Libre, Salen Hoy, Check-out pronto, Ocupada, No Disponible, Reservada, Check-in pronto, Limpieza.</summary>
        public string EstadoLeyenda { get; set; }
    }

    /// <summary>
    /// Estado detallado de un departamento
    /// </summary>
    public class EstadoDepartamento
    {
        public int IdDepartamento { get; set; }
        public string NumeroDepartamento { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; } // Disponible, Ocupado, Mantenimiento, Limpieza
        public string ClienteActual { get; set; }
        public string ColorEstado { get; set; }
        public string IconoEstado { get; set; }
        public string DescripcionMantenimiento { get; set; }
        public string EstadoLeyenda { get; set; }
    }

    /// <summary>
    /// Estadísticas resumen del estado de las habitaciones y departamentos
    /// </summary>
    public class EstadisticasResumen
    {
        public int TotalHabitaciones { get; set; }
        public int HabitacionesDisponibles { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int HabitacionesMantenimiento { get; set; }
        public int HabitacionesLimpieza { get; set; }
        public int TotalDepartamentos { get; set; }
        public int DepartamentosDisponibles { get; set; }
        public int DepartamentosOcupados { get; set; }
        public int DepartamentosMantenimiento { get; set; }
        public int ReservasHoy { get; set; }
        public int CheckOutsHoy { get; set; }
    }
}

