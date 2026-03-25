using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Listar
{
    public interface IListarTemporadasLN
    {
        List<TemporadasDTO> Listar();
    }
}

