using HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeHabitacion.Listar;
using HotelPrado.Abstracciones.Modelos.TipoDeHabitacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.TipoDeHabitacion.Listar
{
    public class ListarTipoDeHabitacionAD : IListarTipoDeHabitacionAD
    {
        Contexto _contexto;

        public ListarTipoDeHabitacionAD()
        {
            _contexto = new Contexto();
        }

        public List<TipoHabitacionDTO> Listar()
        {
            List<TipoHabitacionDTO> laListaDeTipoDeHabitacion = (from TipoDeHabitacion in _contexto.TipoHabitacionTabla
                                                                 select new TipoHabitacionDTO
                                                                     {
                                                                     IdTipoHabitacion = TipoDeHabitacion.IdTipoHabitacion,
                                                                     Descripcion = TipoDeHabitacion.Descripcion,
                                                                     CapacidadDePersonas = TipoDeHabitacion.CapacidadDePersonas,
                                                                     NumeroHabitaciones = TipoDeHabitacion.NumeroHabitaciones
                                                                 }).ToList();
            return laListaDeTipoDeHabitacion;

        }
    }
}
