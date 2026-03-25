using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Registrar
{
    public interface IRegistrarFacturasAD
    {
        Task<int> Guardar(FacturasTabla laFacturaAGuardar);
    }
}

