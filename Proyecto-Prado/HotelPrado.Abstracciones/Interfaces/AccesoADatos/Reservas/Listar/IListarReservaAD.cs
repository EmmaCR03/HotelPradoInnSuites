using HotelPrado.Abstracciones.Modelos.Reservas;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Listar
{
    public interface IListarReservaAD
    {
        List<ReservasDTO> Listar();
    }
}
