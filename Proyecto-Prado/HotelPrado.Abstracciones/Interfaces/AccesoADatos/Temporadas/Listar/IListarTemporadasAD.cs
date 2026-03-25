using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Listar
{
    public interface IListarTemporadasAD
    {
        List<TemporadasDTO> Listar();
    }
}

