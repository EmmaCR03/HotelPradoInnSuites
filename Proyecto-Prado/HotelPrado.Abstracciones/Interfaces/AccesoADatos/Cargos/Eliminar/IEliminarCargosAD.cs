using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Eliminar
{
    public interface IEliminarCargosAD
    {
        Task<int> Eliminar(int IdCargo);
    }
}

