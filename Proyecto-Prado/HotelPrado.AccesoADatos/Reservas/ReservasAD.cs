using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Reservas
{
    public class ReservasAD : IReservasDA
    {
        Contexto _contexto;

        public ReservasAD()
        {
            _contexto = new Contexto();
        }



        public async Task<int> CrearReservaUsuario(ReservasTabla laReservaAGuardar, int IdHabitacion)
        {
            laReservaAGuardar.IdHabitacion = IdHabitacion;

            _contexto.ReservasTabla.Add(laReservaAGuardar);

            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }





        public List<ReservasDTO> ObtenerReservasPorUsuario(string IdUsuario)
        {
            var reservasPorUsuario = (from reserva in _contexto.ReservasTabla
                                      where reserva.IdCliente == IdUsuario 
                                      select new ReservasDTO
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
                                      }).ToList();
            return reservasPorUsuario;
        }

    }
}