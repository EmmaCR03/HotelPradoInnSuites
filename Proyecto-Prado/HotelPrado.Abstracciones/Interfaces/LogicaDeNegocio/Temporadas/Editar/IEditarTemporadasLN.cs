using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Editar
{
    public interface IEditarTemporadasLN
    {
        Task<int> Actualizar(TemporadasDTO laTemporadaEnVista);
    }
}

