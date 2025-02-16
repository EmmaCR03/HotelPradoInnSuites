using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.ObtenerPorId
{
    public class ObtenerHabitacionesPorIdAD : IObtenerHabitacionesPorIdAD
    {
        Contexto _contexto;

        public ObtenerHabitacionesPorIdAD()
        {
            _contexto = new Contexto();
        }

        public HabitacionesTabla Obtener(int IdHabitacion)
        {
            return _contexto.HabitacionesTabla
                .FirstOrDefault(d => d.IdHabitacion == IdHabitacion);
        }

    }
}
