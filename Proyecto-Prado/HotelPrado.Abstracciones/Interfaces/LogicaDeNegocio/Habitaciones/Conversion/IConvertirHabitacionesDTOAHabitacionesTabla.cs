using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Conversion
{
    public interface IConvertirHabitacionesDTOAHabitacionesTabla
    {
        HabitacionesTabla Convertir(HabitacionesDTO laHabitacion);
    }
}
