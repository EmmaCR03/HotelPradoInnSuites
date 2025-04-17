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
            var laListaDeReservas = (from r in _contexto.ReservasTabla
                                     select new ReservasDTO
                                     {
                                         IdReserva = r.IdReserva,
                                         IdCliente = r.IdCliente,
                                         cantidadPersonas = r.cantidadPersonas,
                                         NombreCliente = r.NombreCliente,
                                         IdHabitacion = r.IdHabitacion,
                                         FechaInicio = r.FechaInicio,
                                         FechaFinal = r.FechaFinal,
                                         EstadoReserva = r.EstadoReserva,
                                         MontoTotal = r.MontoTotal
                                     }).ToList();

            return laListaDeReservas;
        }
    }
}