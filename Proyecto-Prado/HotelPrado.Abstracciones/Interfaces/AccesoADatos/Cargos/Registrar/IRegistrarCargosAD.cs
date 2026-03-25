using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Registrar
{
    public interface IRegistrarCargosAD
    {
        Task<int> Guardar(CargosTabla elCargoAGuardar);
    }
}

