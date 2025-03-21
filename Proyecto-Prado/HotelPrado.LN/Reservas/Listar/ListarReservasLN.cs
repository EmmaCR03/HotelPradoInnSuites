using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Listar;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.AccesoADatos.Reservas.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Reservas.Listar
{
    public class ListarReservasLN : IListarReservasLN
    {
        IListarReservaAD _listarReservasAD;

        public ListarReservasLN()
        {
            _listarReservasAD = new ListarReservaAD();
        }

        public List<ReservasDTO> Listar()
        {
            List<ReservasDTO> laListaDeReserva = _listarReservasAD.Listar();

            return laListaDeReserva;


        }
    }
}
