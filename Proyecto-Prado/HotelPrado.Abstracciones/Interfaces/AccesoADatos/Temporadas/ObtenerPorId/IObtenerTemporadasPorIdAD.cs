using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.ObtenerPorId
{
    public interface IObtenerTemporadasPorIdAD
    {
        Task<TemporadasTabla> Obtener(int IdTemporada);
    }
}

