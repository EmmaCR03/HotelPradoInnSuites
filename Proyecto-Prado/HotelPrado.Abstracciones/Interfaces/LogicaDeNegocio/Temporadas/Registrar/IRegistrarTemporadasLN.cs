using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Registrar
{
    public interface IRegistrarTemporadasLN
    {
        Task<int> Guardar(TemporadasDTO modelo);
    }
}

