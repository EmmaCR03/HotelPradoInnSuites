using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.ObtenerEnlaces
{
    public interface IObtenerEnlacesHAD
    {
        List<HabitacionesTabla> ObtenerHabitaciones();
    }
}
