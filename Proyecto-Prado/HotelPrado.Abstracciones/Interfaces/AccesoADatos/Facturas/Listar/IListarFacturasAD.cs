using HotelPrado.Abstracciones.Modelos.Facturas;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Listar
{
    public interface IListarFacturasAD
    {
        List<FacturasDTO> Listar();
    }
}

