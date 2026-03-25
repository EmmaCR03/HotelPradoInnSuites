using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelPrado.AccesoADatos.Habitacion.HabDisponibles
{
    public class HabDisponiblesAD : IHabDisponiblesAD
    {
        Contexto _contexto;

        public HabDisponiblesAD()
        {
            _contexto = new Contexto();
        }

        public List<HabitacionesDTO> ListarDisponibles(DateTime check_in, DateTime check_out, int capacidad)
        {
            int totalNoches = (int)(check_out - check_in).TotalDays; // Calcula el total de noches

            var habitacionesDisponibles =
                from laHabitacion in _contexto.HabitacionesTabla
                where laHabitacion.CapacidadMin <= capacidad
                      && laHabitacion.CapacidadMax >= capacidad // Capacidad suficiente
                      && !_contexto.ReservasTabla.Any(reserva =>
                           reserva.IdHabitacion == laHabitacion.IdHabitacion &&
                           (reserva.EstadoReserva == "Confirmada" || reserva.EstadoReserva == "Solicitada") &&
                           reserva.FechaInicio.HasValue &&
                           reserva.FechaFinal.HasValue &&
                           (
                               (check_in >= reserva.FechaInicio.Value && check_in < reserva.FechaFinal.Value) ||
                               (check_out > reserva.FechaInicio.Value && check_out <= reserva.FechaFinal.Value) ||
                               (check_in <= reserva.FechaInicio.Value && check_out >= reserva.FechaFinal.Value)
                           )
                       )
                select new HabitacionesDTO
                {
                    IdHabitacion = laHabitacion.IdHabitacion,
                    NumeroHabitacion = laHabitacion.NumeroHabitacion,
                    PrecioPorNoche1P = laHabitacion.PrecioPorNoche1P,
                    PrecioPorNoche2P = laHabitacion.PrecioPorNoche2P,
                    PrecioPorNoche3P = laHabitacion.PrecioPorNoche3P,
                    PrecioPorNoche4P = laHabitacion.PrecioPorNoche4P,
                    PrecioGeneral = laHabitacion.PrecioGeneral,
                    PrecioGobierno = laHabitacion.PrecioGobierno,
                    PrecioCorporativo = laHabitacion.PrecioCorporativo,
                    // Tarifa general es la que ven los clientes; si no está definida usamos precio por capacidad
                    PrecioFinal = laHabitacion.PrecioGeneral > 0 ? laHabitacion.PrecioGeneral :
                                 (capacidad == 1 ? laHabitacion.PrecioPorNoche1P :
                                  capacidad == 2 ? laHabitacion.PrecioPorNoche2P :
                                  capacidad == 3 ? laHabitacion.PrecioPorNoche3P :
                                  laHabitacion.PrecioPorNoche4P),
                    TotalNoches = totalNoches,
                    TotalPrecio = (laHabitacion.PrecioGeneral > 0 ? laHabitacion.PrecioGeneral :
                                  (capacidad == 1 ? laHabitacion.PrecioPorNoche1P :
                                   capacidad == 2 ? laHabitacion.PrecioPorNoche2P :
                                   capacidad == 3 ? laHabitacion.PrecioPorNoche3P :
                                   laHabitacion.PrecioPorNoche4P)) * totalNoches,
                    Estado = laHabitacion.Estado,
                    Capacidad = capacidad,
                    UrlImagenes = laHabitacion.UrlImagenes,
                    CapacidadMin = laHabitacion.CapacidadMin,
                    CapacidadMax = laHabitacion.CapacidadMax,
                };

            return habitacionesDisponibles.ToList();
        }
    }
}