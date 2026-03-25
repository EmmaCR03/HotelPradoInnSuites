using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Registrar
{
    public interface IRegistrarSolicitudLimpiezaLN
    {
        Task<int> Guardar(SolicitudLimpiezaDTO modelo);
    }
}

