using HotelPrado.Abstracciones.Modelos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Listar
{
    public interface IListarReservasLN
    {
        List<ReservasDTO> Listar();
    }
}
