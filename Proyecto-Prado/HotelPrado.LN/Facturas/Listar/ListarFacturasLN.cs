using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Listar;
using HotelPrado.Abstracciones.Modelos.Facturas;
using HotelPrado.AccesoADatos.Facturas.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Facturas.Listar
{
    public class ListarFacturasLN : IListarFacturasLN
    {
        IListarFacturasAD _listarFacturasAD;

        public ListarFacturasLN()
        {
            _listarFacturasAD = new ListarFacturasAD();
        }

        public List<FacturasDTO> Listar()
        {
            List<FacturasDTO> laListaDeFacturas = _listarFacturasAD.Listar();
            return laListaDeFacturas;
        }
    }
}

