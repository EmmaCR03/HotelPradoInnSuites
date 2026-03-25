using HotelPrado.Abstracciones.Modelos.Facturas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Editar
{
    public interface IEditarFacturasLN
    {
        Task<int> Actualizar(FacturasDTO laFacturaEnVista);
    }
}

