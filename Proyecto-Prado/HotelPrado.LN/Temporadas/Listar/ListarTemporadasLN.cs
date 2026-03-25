using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Listar;
using HotelPrado.Abstracciones.Modelos.Temporadas;
using HotelPrado.AccesoADatos.Temporadas.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Temporadas.Listar
{
    public class ListarTemporadasLN : IListarTemporadasLN
    {
        IListarTemporadasAD _listarTemporadasAD;

        public ListarTemporadasLN()
        {
            _listarTemporadasAD = new ListarTemporadasAD();
        }

        public List<TemporadasDTO> Listar()
        {
            List<TemporadasDTO> laListaDeTemporadas = _listarTemporadasAD.Listar();
            return laListaDeTemporadas;
        }
    }
}

