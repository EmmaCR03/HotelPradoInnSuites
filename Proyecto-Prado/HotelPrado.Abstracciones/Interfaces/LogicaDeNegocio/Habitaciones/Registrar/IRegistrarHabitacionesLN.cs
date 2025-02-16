using HotelPrado.Abstracciones.Modelos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Registrar
{
    public interface IRegistrarHabitacionesLN
    {
        Task<int> Guardar(HabitacionesDTO modelo);
    }
}
