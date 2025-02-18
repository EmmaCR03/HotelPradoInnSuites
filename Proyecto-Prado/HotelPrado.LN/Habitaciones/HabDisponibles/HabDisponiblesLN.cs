using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.AccesoADatos.Habitacion.HabDisponibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.HabDisponibles
{
   public class HabDisponiblesLN : IHabDisponiblesLN
    {
        IHabDisponiblesAD _habitacionesDisponiblesAD;

        public HabDisponiblesLN()
        {
            _habitacionesDisponiblesAD = new HabDisponiblesAD();
        }

        public List<HabitacionesDTO> ListarDisponibles(DateTime check_in, DateTime check_out, int capacidad)
        {
            List<HabitacionesDTO> laListaDeHabitacion = _habitacionesDisponiblesAD.ListarDisponibles(check_in, check_out, capacidad);

            return laListaDeHabitacion;


        }
    }
}
