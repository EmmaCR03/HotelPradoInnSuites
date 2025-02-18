using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.HabDisponibles
{
    public interface IHabDisponiblesAD
    {
        List<HabitacionesDTO> ListarDisponibles(DateTime check_in, DateTime check_out, int capacidad);
    }
}
