using HotelPrado.Abstracciones.Modelos.Facturas;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Listar
{
    public interface IListarFacturasLN
    {
        List<FacturasDTO> Listar();
    }
}

