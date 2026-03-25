using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Registrar
{
    public interface IRegistrarCargosLN
    {
        Task<int> Guardar(CargosDTO modelo);
    }
}

