using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Eliminar
{
    public interface IEliminarTemporadasLN
    {
        Task<int> Eliminar(int IdTemporada);
    }
}

