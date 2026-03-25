using System;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Eliminar
{
    public interface IEliminarFacturasAD
    {
        Task<int> Eliminar(Guid Id);
    }
}

