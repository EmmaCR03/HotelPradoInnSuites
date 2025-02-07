using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Conversion;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Departamentos.Conversion
{
    public class ConvertirDepartamentoDTOADepartamentoTabla : IConvertirDepartamentoDTOADepartamentoTabla
    {
        public DepartamentoTabla Convertir(DepartamentoDTO elDepartamento) 
        {
            return new DepartamentoTabla
            {
                IdDepartamento = elDepartamento.IdDepartamento,
                IdCliente = (int)elDepartamento.IdCliente,
                Nombre = elDepartamento.Nombre,
                Descripcion = elDepartamento.Descripcion,
                IdTipoDepartamento = elDepartamento.IdTipoDepartamento,
                Precio = elDepartamento.Precio,
                Estado = elDepartamento.Estado
            };
        }
    }
}
