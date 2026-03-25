using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Registrar
{
    public interface IRegistrarTemporadasAD
    {
        Task<int> Guardar(TemporadasTabla laTemporadaAGuardar);
    }
}

