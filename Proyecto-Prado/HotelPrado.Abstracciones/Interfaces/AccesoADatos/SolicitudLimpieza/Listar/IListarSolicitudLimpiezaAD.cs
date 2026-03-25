using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Listar
{
    public interface IListarSolicitudLimpiezaAD
    {
        List<SolicitudLimpiezaDTO> Listar();
    }
}

