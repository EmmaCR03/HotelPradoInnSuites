using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerEnlaces
{
    public interface IObtenerEnlacesAD
    {
        List<DepartamentoTabla> ObtenerDepartamentos();
    }
}
