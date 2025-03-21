using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System.Linq;

namespace HotelPrado.AccesoADatos.Reservas.ObtenerPorId
{
    public class ObtenerReservaPorIdAD : IObtenerReservaPorIdAD
    {
        Contexto _contexto;

        public ObtenerReservaPorIdAD()
        {
            _contexto = new Contexto();
        }

        public ReservasTabla Obtener(int IdReserva)
        {
            return _contexto.ReservasTabla
                .FirstOrDefault(d => d.IdReserva == IdReserva);
        }

    }
}