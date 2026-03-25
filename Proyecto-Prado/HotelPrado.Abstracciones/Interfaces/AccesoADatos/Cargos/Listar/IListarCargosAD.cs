using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Listar
{
    public interface IListarCargosAD
    {
        List<CargosDTO> Listar();
    }
}

