using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.AccesoADatos.Habitacion.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.ObtenerPorId
{
    public class ObtenerHabitacionesPorIdLN : IObtenerHabitacionesPorIdLN
    {
        IObtenerHabitacionesPorIdAD _obtenerporId;

        public ObtenerHabitacionesPorIdLN()
        {
            _obtenerporId = new ObtenerHabitacionesPorIdAD();
        }
        public HabitacionesDTO Obtener(int IdHabitacion)
        {
            HabitacionesTabla habitacionEnBaseDeDatos = _obtenerporId.Obtener(IdHabitacion);
            HabitacionesDTO laHabitacionAMostrar = ConvertirADepartamentoAMostrar(habitacionEnBaseDeDatos);
            return laHabitacionAMostrar;
        }
        private HabitacionesDTO ConvertirADepartamentoAMostrar(HabitacionesTabla habitacionEnBaseDeDatos)
        {
            return new HabitacionesDTO
            {
                IdHabitacion = habitacionEnBaseDeDatos.IdHabitacion,
                PrecioPorNoche1P = habitacionEnBaseDeDatos.PrecioPorNoche1P,
                PrecioPorNoche2P = habitacionEnBaseDeDatos.PrecioPorNoche2P,
                PrecioPorNoche3P = habitacionEnBaseDeDatos.PrecioPorNoche3P,
                PrecioPorNoche4P = habitacionEnBaseDeDatos.PrecioPorNoche4P,
                IdTipoHabitacion = habitacionEnBaseDeDatos.IdTipoHabitacion,
                Estado = habitacionEnBaseDeDatos.Estado,
                CapacidadMax=habitacionEnBaseDeDatos.CapacidadMax,
                CapacidadMin=habitacionEnBaseDeDatos.CapacidadMin,
                NumeroHabitacion = habitacionEnBaseDeDatos.NumeroHabitacion,
            };
        }


    }
}
