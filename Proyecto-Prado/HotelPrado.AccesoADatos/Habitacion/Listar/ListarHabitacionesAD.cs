using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.Listar
{
   public class ListarHabitacionesAD : IListarHabitacionesAD
    {
        Contexto _contexto;

        public ListarHabitacionesAD()
        {
            _contexto = new Contexto();
        }

        public List<HabitacionesDTO> Listar()
        {
            var laListaDeHabitaciones = (from laHabitacion in _contexto.HabitacionesTabla
                                         join tipoHab in _contexto.TipoHabitacionTabla
                                         on laHabitacion.IdTipoHabitacion equals tipoHab.IdTipoHabitacion into tipoHabJoin
                                         from tipo in tipoHabJoin.DefaultIfEmpty()  // LEFT JOIN
                                         select new HabitacionesDTO
                                         {
                                             IdHabitacion = laHabitacion.IdHabitacion,
                                             NumeroHabitacion = laHabitacion.NumeroHabitacion,
                                             PrecioPorNoche = laHabitacion.PrecioPorNoche,
                                             Estado = laHabitacion.Estado,
                                             IdTipoHabitacion = laHabitacion.IdTipoHabitacion,
                                         }).ToList();


            return laListaDeHabitaciones;
        }


    }
}
