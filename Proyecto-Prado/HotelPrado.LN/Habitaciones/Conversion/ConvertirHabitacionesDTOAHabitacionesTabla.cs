using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Conversion;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.Conversion
{
    public class ConvertirHabitacionesDTOAHabitacionesTabla : IConvertirHabitacionesDTOAHabitacionesTabla
    {
        public HabitacionesTabla Convertir(HabitacionesDTO laHabitacion)
        {
            return new HabitacionesTabla
            {
                IdHabitacion = laHabitacion.IdHabitacion,
                NumeroHabitacion = laHabitacion.NumeroHabitacion,
                PrecioPorNoche1P = laHabitacion.PrecioPorNoche1P,
                PrecioPorNoche2P = laHabitacion.PrecioPorNoche2P,
                PrecioPorNoche3P = laHabitacion.PrecioPorNoche3P,
                PrecioPorNoche4P = laHabitacion.PrecioPorNoche4P,
                IdTipoHabitacion = laHabitacion.IdTipoHabitacion,
                CapacidadMax = laHabitacion.CapacidadMax,
                CapacidadMin = laHabitacion.CapacidadMin,
                Estado = laHabitacion.Estado
            };
        }
    }
}
