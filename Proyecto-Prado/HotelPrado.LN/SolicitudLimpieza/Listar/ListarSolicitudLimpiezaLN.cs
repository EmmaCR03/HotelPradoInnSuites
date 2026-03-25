using HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Listar;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.AccesoADatos.SolicitudLimpieza.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.SolicitudLimpieza.Listar
{
    public class ListarSolicitudLimpiezaLN : IListarSolicitudLimpiezaLN
    {
        private readonly IListarSolicitudLimpiezaAD _listarSolicitudLimpiezaAD;

        public ListarSolicitudLimpiezaLN()
        {
            _listarSolicitudLimpiezaAD = new ListarSolicitudLimpiezaAD();
        }

        public List<SolicitudLimpiezaDTO> Listar()
        {
            return _listarSolicitudLimpiezaAD.Listar();
        }
    }
}

