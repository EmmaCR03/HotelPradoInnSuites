using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Editar
{
    public interface IEditarReservaAD
    {
        Task<int> Editar(ReservasTabla laReservaActualizar);
    }
}
