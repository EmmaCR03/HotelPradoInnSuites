using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.ObtenerPorId
{
    public interface IObtenerFacturasPorIdAD
    {
        Task<FacturasTabla> Obtener(Guid Id);
    }
}

