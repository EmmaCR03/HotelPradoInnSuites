using HotelPrado.Abstracciones.Modelos.TipoDeDepartamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoDeDepartamentos
{
    public interface IListarTipoDeDepartamenoLN
    {
        List<TipoDepartamentoDTO> Listar();
    }
}
