using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Editar
{
    public interface IEditarTemporadasAD
    {
        Task<int> Editar(TemporadasTabla laTemporadaActualizar);
    }
}

