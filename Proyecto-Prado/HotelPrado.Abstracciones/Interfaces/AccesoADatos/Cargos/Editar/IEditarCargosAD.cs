using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Editar
{
    public interface IEditarCargosAD
    {
        Task<int> Editar(CargosTabla elCargoActualizar);
    }
}

