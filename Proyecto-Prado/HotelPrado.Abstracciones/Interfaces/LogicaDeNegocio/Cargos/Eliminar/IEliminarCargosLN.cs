using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Eliminar
{
    public interface IEliminarCargosLN
    {
        Task<int> Eliminar(int IdCargo);
    }
}

