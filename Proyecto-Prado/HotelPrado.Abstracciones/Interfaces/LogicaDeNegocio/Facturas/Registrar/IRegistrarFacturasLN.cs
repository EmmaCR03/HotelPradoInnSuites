using HotelPrado.Abstracciones.Modelos.Facturas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Registrar
{
    public interface IRegistrarFacturasLN
    {
        Task<int> Guardar(FacturasDTO modelo);
    }
}

