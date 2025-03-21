using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Convertir;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;

namespace HotelPrado.LN.Reservas.Conversion
{
    public class ConvertirReservasDTOAReservasTabla : IConvertirReservasDTOAReservasTabla
    {
        public ReservasTabla Convertir(ReservasDTO laReserva)
        {
            return new ReservasTabla
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
            };
        }
    }
}