using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Listar
{
    public interface IListarSolicitudLimpiezaLN
    {
        List<SolicitudLimpiezaDTO> Listar();
    }
}

