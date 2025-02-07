using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.ObtenerPorId
{
    public class ObtenerColaboradorPorIdAD : IObtenerColaboradorPorIdAD
    {
        Contexto _contexto;

        public ObtenerColaboradorPorIdAD()
        {
            _contexto = new Contexto();
        }

        public ColaboradorTabla Obtener(int IdColaborador)
        {
            return _contexto.ColaboradorTabla.FirstOrDefault(d => d.IdColaborador == IdColaborador);
        }

    }
}
