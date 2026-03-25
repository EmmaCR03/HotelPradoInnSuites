using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Editar
{
    public interface IEditarFacturasAD
    {
        Task<int> Editar(FacturasTabla laFacturaActualizar);
    }
}

