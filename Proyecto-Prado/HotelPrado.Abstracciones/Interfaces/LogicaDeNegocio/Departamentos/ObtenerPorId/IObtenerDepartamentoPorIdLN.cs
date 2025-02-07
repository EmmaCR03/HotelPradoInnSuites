using HotelPrado.Abstracciones.Modelos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.ObtenerPorId
{
    public interface IObtenerCitasPorIdLN
    {
        DepartamentoDTO Obtener(int IdDepartamento);
    }
}
