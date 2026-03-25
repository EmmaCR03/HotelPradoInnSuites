using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.ObtenerPorId
{
    public interface IObtenerCargosPorIdAD
    {
        Task<CargosTabla> Obtener(int IdCargo);
    }
}

