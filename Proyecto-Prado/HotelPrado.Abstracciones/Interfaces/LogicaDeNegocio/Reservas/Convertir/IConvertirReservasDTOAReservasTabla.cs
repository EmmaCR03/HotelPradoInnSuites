using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Convertir
{
    public interface IConvertirReservasDTOAReservasTabla
    {
        ReservasTabla Convertir(ReservasDTO laReserva);
    }
}
