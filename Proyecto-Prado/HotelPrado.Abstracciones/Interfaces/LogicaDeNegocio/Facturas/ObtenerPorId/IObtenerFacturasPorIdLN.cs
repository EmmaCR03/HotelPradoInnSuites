using HotelPrado.Abstracciones.Modelos.Facturas;
using System;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.ObtenerPorId
{
    public interface IObtenerFacturasPorIdLN
    {
        Task<FacturasDTO> Obtener(Guid Id);
    }
}

