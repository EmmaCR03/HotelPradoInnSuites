using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas
{
    public interface IReservasLN
    {
        Task<int> CrearReservasUsuario(ReservasDTO reserva);
        Task<int> CrearReservasAdmin(ReservasDTO reserva);
        Task<int> ActualizarReservas(ReservasDTO reserva);
        Task<int> EliminarReservas(int IdReserva);

        List<ReservasDTO> ListarReservas();
        List<ReservasDTO> ListarReservasUsuario(int IdUsuario);
        List<ReservasDTO> ListarReservasId(int Id);

        ReservasTabla Convertir(ReservasDTO laReserva);
    }
}
