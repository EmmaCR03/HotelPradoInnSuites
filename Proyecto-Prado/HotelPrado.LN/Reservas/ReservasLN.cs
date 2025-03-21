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

        public async Task<int> ActualizarReservas(ReservasDTO reserva)
        {
            try
            {
                var datosAnteriores = _reservasDA.Obtener(reserva.IdReserva);

                int cantidadDeDatosActualizados = await _reservasDA.Editar(Convertir(reserva));

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al actualizar la reserva.", ex);
            }
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

        public async Task<int> CrearReservasAdmin(ReservasDTO reserva)
        {
            var reservaTabla = Convertir(reserva);
            return await _reservasDA.Crear(reservaTabla);
        }

        public async Task<int> CrearReservasUsuario(ReservasDTO reserva)
        {
            var reservaTabla = Convertir(reserva);
            return await _reservasDA.CrearReservaUsuario(reservaTabla, reserva.IdHabitacion);
        }

        public async Task<int> EliminarReservas(int IdReserva)
        {
            return await _reservasDA.Eliminar(IdReserva);
        }

        public List<ReservasDTO> ListarReservas()
        {
            var reservas = _reservasDA.ObtenerReservas();
            return reservas.Select(r => new ReservasDTO
            {
                IdReserva = r.IdReserva,
                IdCliente = r.IdCliente,
                NombreCliente = r.NombreCliente,
                cantidadPersonas = r.cantidadPersonas,
                IdHabitacion = r.IdHabitacion,
                FechaInicio = r.FechaInicio,
                FechaFinal = r.FechaFinal,
                EstadoReserva = r.EstadoReserva,
                MontoTotal = r.MontoTotal
            }).ToList();
        }

        public List<ReservasDTO> ListarReservasId(int Id)
        {
            var reserva = _reservasDA.Obtener(Id);
            if (reserva == null) return null;

            return new List<ReservasDTO>
            {
                new ReservasDTO
                {
                    IdReserva = reserva.IdReserva,
                    IdCliente = reserva.IdCliente,
                    NombreCliente = reserva.NombreCliente,
                    cantidadPersonas = reserva.cantidadPersonas,
                    IdHabitacion = reserva.IdHabitacion,
                    FechaInicio = reserva.FechaInicio,
                    FechaFinal = reserva.FechaFinal,
                    EstadoReserva = reserva.EstadoReserva,
                    MontoTotal = reserva.MontoTotal
                }
            };
        }

        public List<ReservasDTO> ListarReservasUsuario(int IdUsuario)
        {
            var reservas = _reservasDA.ObtenerReservasPorUsuario(IdUsuario.ToString());
            return reservas.Select(r => new ReservasDTO
            {
                IdReserva = r.IdReserva,
                IdCliente = r.IdCliente,
                NombreCliente = r.NombreCliente,
                cantidadPersonas = r.cantidadPersonas,
                IdHabitacion = r.IdHabitacion,
                FechaInicio = r.FechaInicio,
                FechaFinal = r.FechaFinal,
                EstadoReserva = r.EstadoReserva,
                MontoTotal = r.MontoTotal
            }).ToList();
        }


    }
}
