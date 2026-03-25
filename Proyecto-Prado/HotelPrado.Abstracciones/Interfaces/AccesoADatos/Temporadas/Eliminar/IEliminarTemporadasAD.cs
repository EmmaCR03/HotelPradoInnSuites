using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Eliminar
{
    public interface IEliminarTemporadasAD
    {
        Task<int> Eliminar(int IdTemporada);
    }
}

