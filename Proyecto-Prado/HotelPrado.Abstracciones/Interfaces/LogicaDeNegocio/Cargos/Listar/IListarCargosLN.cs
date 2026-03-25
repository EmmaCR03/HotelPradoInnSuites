using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Listar
{
    public interface IListarCargosLN
    {
        List<CargosDTO> Listar();
    }
}

