using HotelPrado.Abstracciones.Modelos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Enlaces
{
    public interface IObtenerEnlacesLN
    {
        List<DepartamentoDTO> ObtenerCitasConEnlaces();
    }
}
