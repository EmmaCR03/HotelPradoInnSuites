using HotelPrado.Abstracciones.Modelos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Registrar
{
    public interface IRegistrarReservaLN
    {
        Task<int> Guardar(ReservasDTO modelo);
    }
}
