using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Listar;
using HotelPrado.Abstracciones.Modelos.Cargos;
using HotelPrado.AccesoADatos.Cargos.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Cargos.Listar
{
    public class ListarCargosLN : IListarCargosLN
    {
        IListarCargosAD _listarCargosAD;

        public ListarCargosLN()
        {
            _listarCargosAD = new ListarCargosAD();
        }

        public List<CargosDTO> Listar()
        {
            List<CargosDTO> laListaDeCargos = _listarCargosAD.Listar();
            return laListaDeCargos;
        }
    }
}

