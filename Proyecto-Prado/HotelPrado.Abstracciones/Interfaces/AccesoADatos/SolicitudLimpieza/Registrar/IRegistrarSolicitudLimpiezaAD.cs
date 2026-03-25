using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Registrar
{
    public interface IRegistrarSolicitudLimpiezaAD
    {
        Task<int> Guardar(SolicitudLimpiezaTabla laSolicitudLimpiezaAGuardar);
    }
}

