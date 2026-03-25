using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.ObtenerPorId
{
    public interface IObtenerCargosPorIdLN
    {
        Task<CargosDTO> Obtener(int IdCargo);
    }
}

