using HotelPrado.Abstracciones.Modelos.TipoDeDepartamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.TipoDeDepartamento.Listar
{
    public interface IListarTipoDeDepartamentosAD
    {
        List<TipoDepartamentoDTO> Listar();
    }
}
