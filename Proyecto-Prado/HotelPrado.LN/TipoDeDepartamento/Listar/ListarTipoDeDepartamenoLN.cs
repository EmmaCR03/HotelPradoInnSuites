using HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeDepartamento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoDeDepartamentos;
using HotelPrado.Abstracciones.Modelos.TipoDeDepartamento;
using HotelPrado.AccesoADatos.TipoDepartamentos.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.TipoDeDepartamento.Listar
{
    public class ListarTipoDeDepartamenoLN : IListarTipoDeDepartamenoLN
    {
        IListarTipoDeDepartamentosAD _listarTipoDeDepartamentoLN;
        public ListarTipoDeDepartamenoLN() 
        {
            _listarTipoDeDepartamentoLN = new ListarTipoDeDepartamentosAD();
        }

        public List<TipoDepartamentoDTO> Listar() 
        { 
        List<TipoDepartamentoDTO> ListaTipoDepartamento = _listarTipoDeDepartamentoLN.Listar();
            return ListaTipoDepartamento;
        }
    }
}
