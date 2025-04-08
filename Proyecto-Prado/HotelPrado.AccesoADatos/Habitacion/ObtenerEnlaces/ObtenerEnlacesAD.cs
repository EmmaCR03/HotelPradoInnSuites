using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.ObtenerEnlaces;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Habitacion.ObtenerEnlaces
{
    public class ObtenerEnlacesAD : IObtenerEnlacesAD
    {
        private readonly Contexto _contexto;

        public ObtenerEnlacesAD()
        {
            _contexto = new Contexto();
        }

        public List<HabitacionesTabla> ObtenerHabitaciones()
        {
            return _contexto.HabitacionesTabla.ToList();
        }
    }
}


