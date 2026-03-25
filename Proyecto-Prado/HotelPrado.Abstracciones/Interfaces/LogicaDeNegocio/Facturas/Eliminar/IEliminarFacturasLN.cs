using System;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Eliminar
{
    public interface IEliminarFacturasLN
    {
        Task<int> Eliminar(Guid Id);
    }
}

