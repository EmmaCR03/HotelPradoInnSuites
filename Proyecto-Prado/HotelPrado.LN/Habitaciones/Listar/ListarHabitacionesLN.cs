using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.AccesoADatos.Habitacion.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.Listar
{
   public class ListarHabitacionesLN : IListarHabitacionesLN
    {
        IListarHabitacionesAD _listarHabitacionesAD;

        public ListarHabitacionesLN()
        {
            _listarHabitacionesAD = new ListarHabitacionesAD();
        }

        public List<HabitacionesDTO> Listar()
        {
            List<HabitacionesDTO> laListaDeHabitacion = _listarHabitacionesAD.Listar();

            return laListaDeHabitacion;


        }
    }
}
