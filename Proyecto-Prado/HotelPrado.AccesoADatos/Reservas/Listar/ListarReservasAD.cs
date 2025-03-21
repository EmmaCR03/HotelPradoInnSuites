using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Listar;
using HotelPrado.Abstracciones.Modelos.Reservas;
using System.Collections.Generic;
using System.Linq;

namespace HotelPrado.AccesoADatos.Reservas.Listar
{

    public class ListarReservaAD : IListarReservaAD
    {
        Contexto _contexto;

        public ListarReservaAD()
        {
            _contexto = new Contexto();
        }

        public List<ReservasDTO> Listar()
        {
            var laListaDeReservas = _contexto.ReservasTabla
                .Select(laReserva => new ReservasDTO
                {
                    IdReserva = laReserva.IdReserva,
                    IdCliente = laReserva.IdCliente,
                    cantidadPersonas = laReserva.cantidadPersonas,
                    NombreCliente = laReserva.NombreCliente,
                    IdHabitacion = laReserva.IdHabitacion,
                    FechaInicio = laReserva.FechaInicio,
                    FechaFinal = laReserva.FechaFinal,
                    EstadoReserva = laReserva.EstadoReserva,
                    MontoTotal = laReserva.MontoTotal,
                }).ToList();

            return laListaDeReservas;
        }
    }
}