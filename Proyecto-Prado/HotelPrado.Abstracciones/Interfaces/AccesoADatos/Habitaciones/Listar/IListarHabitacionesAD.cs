using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar
{
    public interface IListarHabitacionesAD
    {
        List<HabitacionesDTO> Listar();
    }
}
