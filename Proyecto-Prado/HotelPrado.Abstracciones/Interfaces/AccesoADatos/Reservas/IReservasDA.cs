using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas
{
    public interface IReservasDA
    {
        Task<int> Crear(ReservasTabla laReservaAGuardar);
        Task<int> CrearReservaUsuario(ReservasTabla laReservaAGuardar,int IdHabitacion);
        Task<int> Editar(ReservasTabla laReservaActualizar);
        ReservasTabla Obtener(int IdReserva);
        List<ReservasTabla> ObtenerReservas();

        List<ReservasTabla> ObtenerReservasPorUsuario(string IdUsuario);

        Task<int> Eliminar(int IdReserva);
    }
}
