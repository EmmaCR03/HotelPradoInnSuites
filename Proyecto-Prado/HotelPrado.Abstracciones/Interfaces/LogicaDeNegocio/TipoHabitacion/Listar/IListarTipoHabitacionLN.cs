using HotelPrado.Abstracciones.Modelos.TipoDeHabitacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoHabitacion.Listar
{
    public interface IListarTipoHabitacionLN
    {
        List<TipoHabitacionDTO> Listar();
    }
}
