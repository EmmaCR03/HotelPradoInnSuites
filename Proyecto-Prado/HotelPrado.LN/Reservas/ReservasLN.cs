using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.AccesoADatos.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.LN.Reservas
{
    public class ReservasLN : IReservasLN
    {
        private readonly IReservasDA _reservasDA;

        public ReservasLN()
        {
            _reservasDA = new ReservasAD();
        }

        public async Task<int> CrearReservasUsuario(ReservasDTO reserva)
        {
            var reservaTabla = Convertir(reserva);
            return await _reservasDA.CrearReservaUsuario(reservaTabla, reserva.IdHabitacion);
        }

        public List<ReservasDTO> ListarReservasUsuario(string IdUsuario)
        {
           List<ReservasDTO> reservas = _reservasDA.ObtenerReservasPorUsuario(IdUsuario);
            return reservas;
        }

        public ReservasTabla Convertir(ReservasDTO laReserva)
        {
            return new ReservasTabla
            {
                IdReserva = laReserva.IdReserva,
                IdCliente = laReserva.IdCliente,
                NombreCliente = laReserva.NombreCliente,
                cantidadPersonas = laReserva.cantidadPersonas,
                IdHabitacion = laReserva.IdHabitacion,
                FechaInicio = laReserva.FechaInicio,
                FechaFinal = laReserva.FechaFinal,
                EstadoReserva = laReserva.EstadoReserva,
                MontoTotal = laReserva.MontoTotal
            };
        }
    }
}
