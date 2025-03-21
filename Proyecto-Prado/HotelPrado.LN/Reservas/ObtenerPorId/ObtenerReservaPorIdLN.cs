using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.AccesoADatos.Reservas.ObtenerPorId;
using System.Threading.Tasks;

namespace HotelPrado.LN.Reservas.ObtenerPorId
{
    class ObtenerReservaPorIdLN : IObtenerReservaPorIdLN
    {
        IObtenerReservaPorIdAD _obtenerporId;

        public ObtenerReservaPorIdLN()
        {
            _obtenerporId = new ObtenerReservaPorIdAD();
        }

        public async Task<ReservasDTO> Obtener(int IdReserva)
        {
            ReservasTabla reservaEnBaseDeDatos = await Task.Run(() => _obtenerporId.Obtener(IdReserva));
            ReservasDTO laReservaAMostrar = ConvertirAReservaAMostrar(reservaEnBaseDeDatos);
            return laReservaAMostrar;
        }

        private ReservasDTO ConvertirAReservaAMostrar(ReservasTabla reservaEnBaseDeDatos)
        {
            return new ReservasDTO
            {
                IdReserva = reservaEnBaseDeDatos.IdReserva,
                IdCliente = reservaEnBaseDeDatos.IdCliente,
                cantidadPersonas = reservaEnBaseDeDatos.cantidadPersonas,
                NombreCliente = reservaEnBaseDeDatos.NombreCliente,
                IdHabitacion = reservaEnBaseDeDatos.IdHabitacion,
                FechaInicio = reservaEnBaseDeDatos.FechaInicio,
                FechaFinal = reservaEnBaseDeDatos.FechaFinal,
                EstadoReserva = reservaEnBaseDeDatos.EstadoReserva,
                MontoTotal = reservaEnBaseDeDatos.MontoTotal
            };
        }
    }
}