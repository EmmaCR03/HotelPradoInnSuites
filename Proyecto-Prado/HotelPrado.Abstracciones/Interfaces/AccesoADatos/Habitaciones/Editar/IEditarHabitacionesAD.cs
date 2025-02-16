using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Editar
{
    public interface IEditarHabitacionesAD
    {

        Task<int> Editar(HabitacionesTabla laHabitacionActualizar);
    }
}
