using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Conversion
{
    public interface IConvertirDepartamentoDTOADepartamentoTabla
    {
        DepartamentoTabla Convertir(DepartamentoDTO elDepartamento);
    }
}
