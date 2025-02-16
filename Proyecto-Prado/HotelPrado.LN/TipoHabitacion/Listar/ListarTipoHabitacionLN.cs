using HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeHabitacion.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoHabitacion.Listar;
using HotelPrado.Abstracciones.Modelos.TipoDeHabitacion;
using HotelPrado.AccesoADatos.TipoDeHabitacion.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.TipoHabitacion.Listar
{
    public class ListarTipoHabitacionLN : IListarTipoHabitacionLN
    {
        IListarTipoDeHabitacionAD _listarTipoHabitacionLN;
        public ListarTipoHabitacionLN()
        {
            _listarTipoHabitacionLN = new ListarTipoDeHabitacionAD();
        }

        public List<TipoHabitacionDTO> Listar()
        {
            List<TipoHabitacionDTO> ListaTipoHabitacion = _listarTipoHabitacionLN.Listar();
            return ListaTipoHabitacion;
        }
    }
}
