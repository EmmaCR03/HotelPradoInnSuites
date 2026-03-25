using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Listar;
using HotelPrado.Abstracciones.Modelos.Temporadas;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HotelPrado.AccesoADatos.Temporadas.Listar
{
    public class ListarTemporadasAD : IListarTemporadasAD
    {
        Contexto _contexto;

        public ListarTemporadasAD()
        {
            _contexto = new Contexto();
        }

        public List<TemporadasDTO> Listar()
        {
            var laListaDeTemporadas = _contexto.TemporadasTabla
                .Select(t => new TemporadasDTO
                {
                    IdTemporada = t.IdTemporada,
                    NumeroTemporada = t.NumeroTemporada,
                    Descripcion = t.Descripcion,
                    CodigoCuenta = t.CodigoCuenta,
                    AumentaAl = t.AumentaAl
                })
                .OrderBy(t => t.NumeroTemporada)
                .ToList();

            return laListaDeTemporadas;
        }
    }
}

