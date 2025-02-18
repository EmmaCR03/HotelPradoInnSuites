using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.HabDisponibles
{
   public class ListarHabitacionesAD : IListarHabitacionesAD
    {
        Contexto _contexto;

        public ListarHabitacionesAD()
        {
            _contexto = new Contexto();
        }

        public List<HabitacionesDTO> Listar(int? capacidad = null, string estado = null)
        {
            var laListaDeHabitaciones = (from laHabitacion in _contexto.HabitacionesTabla
                                         join tipoHab in _contexto.TipoHabitacionTabla
                                         on laHabitacion.IdTipoHabitacion equals tipoHab.IdTipoHabitacion into tipoHabJoin
                                         from tipo in tipoHabJoin.DefaultIfEmpty()  // LEFT JOIN
                                         select new HabitacionesDTO
                                         {
                                             IdHabitacion = laHabitacion.IdHabitacion,
                                             NumeroHabitacion = laHabitacion.NumeroHabitacion,
                                             PrecioPorNoche1P = laHabitacion.PrecioPorNoche1P,
                                             PrecioPorNoche2P = laHabitacion.PrecioPorNoche2P,
                                             PrecioPorNoche3P = laHabitacion.PrecioPorNoche3P,
                                             PrecioPorNoche4P = laHabitacion.PrecioPorNoche4P,
                                             Estado = laHabitacion.Estado,
                                             Capacidad = laHabitacion.Capacidad,
                                             IdTipoHabitacion =laHabitacion.IdTipoHabitacion
                                         });

            if (capacidad.HasValue)
            {
                laListaDeHabitaciones = laListaDeHabitaciones.Where(h => h.Capacidad == capacidad.Value);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                laListaDeHabitaciones = laListaDeHabitaciones.Where(h => h.Estado == estado);
            }


            return laListaDeHabitaciones.ToList();
        }


    }
}
