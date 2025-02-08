using HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeDepartamento.Listar;
using HotelPrado.Abstracciones.Modelos.TipoDeDepartamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.TipoDepartamentos.Listar
{
    public class ListarTipoDeDepartamentosAD : IListarTipoDeDepartamentosAD
    {
        Contexto _contexto;

        public ListarTipoDeDepartamentosAD()
        {
            _contexto = new Contexto();
        }

        public List<TipoDepartamentoDTO> Listar()
        {
            List<TipoDepartamentoDTO> laListaDeTipoDeDepartamento = (from TipoDeDepartamento in _contexto.TipoDepartamentoTabla
                                                                     select new TipoDepartamentoDTO
                                                                     {
                                                                         IdTipoDepartamento = TipoDeDepartamento.IdTipoDepartamento,
                                                                         NumeroHabitaciones = TipoDeDepartamento.NumeroHabitaciones,
                                                                         Amueblado = TipoDeDepartamento.Amueblado
                                                                     }).ToList();
            return laListaDeTipoDeDepartamento;

        }
    }
}
