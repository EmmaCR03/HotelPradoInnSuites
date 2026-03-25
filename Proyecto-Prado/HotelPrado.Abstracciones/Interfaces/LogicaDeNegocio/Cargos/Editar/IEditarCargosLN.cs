using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Editar
{
    public interface IEditarCargosLN
    {
        Task<int> Actualizar(CargosDTO elCargoEnVista);
    }
}

