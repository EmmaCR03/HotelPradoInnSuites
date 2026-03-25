using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.ObtenerPorId
{
    public interface IObtenerTemporadasPorIdLN
    {
        Task<TemporadasDTO> Obtener(int IdTemporada);
    }
}

