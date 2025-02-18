using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var habitacionesDisponibles = (from laHabitacion in _contexto.HabitacionesTabla
                                           join tipoHab in _contexto.TipoHabitacionTabla
                                           on laHabitacion.IdTipoHabitacion equals tipoHab.IdTipoHabitacion into tipoHabJoin
                                           from tipo in tipoHabJoin.DefaultIfEmpty()  // LEFT JOIN
                                           where laHabitacion.CapacidadMin <= capacidad
                                                 && laHabitacion.CapacidadMax >= capacidad // Capacidad suficiente
                                                 && !_contexto.ReservasTabla.Any(reserva =>
                                                      reserva.IdHabitacion == laHabitacion.IdHabitacion &&
                                                      (
                                                          (check_in >= reserva.FechaInicio && check_in < reserva.FechaFinal) ||
                                                          (check_out > reserva.FechaInicio && check_out <= reserva.FechaFinal) ||
                                                          (check_in <= reserva.FechaInicio && check_out >= reserva.FechaFinal)
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
                                               PrecioFinal = capacidad == 1 ? laHabitacion.PrecioPorNoche1P :
                                                            capacidad == 2 ? laHabitacion.PrecioPorNoche2P :
                                                            capacidad == 3 ? laHabitacion.PrecioPorNoche3P :
                                                            laHabitacion.PrecioPorNoche4P,
                                               TotalNoches = totalNoches, // Se añade el total de noches
                                               TotalPrecio = (capacidad == 1 ? laHabitacion.PrecioPorNoche1P :
                                                             capacidad == 2 ? laHabitacion.PrecioPorNoche2P :
                                                             capacidad == 3 ? laHabitacion.PrecioPorNoche3P :
                                                             laHabitacion.PrecioPorNoche4P) * totalNoches, // Precio total
                                               Estado = laHabitacion.Estado,
                                               Capacidad = capacidad,
                                               IdTipoHabitacion = laHabitacion.IdTipoHabitacion
                                           });

            return habitacionesDisponibles.ToList();
        }
    }
}