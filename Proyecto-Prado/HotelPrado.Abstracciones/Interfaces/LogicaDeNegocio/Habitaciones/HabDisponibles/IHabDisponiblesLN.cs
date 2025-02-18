using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.HabDisponibles
{
    public interface IHabDisponiblesLN
    {
        List<HabitacionesDTO> ListarDisponibles(DateTime check_in, DateTime check_out, int capacidad);
    }
}
