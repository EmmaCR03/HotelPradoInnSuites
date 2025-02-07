using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.AccesoADatos.Departamentos.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Departamentos.ObtenerPorId
{
    public class ObtenerCitasPorIdLN : IObtenerCitasPorIdLN
    {
        IObtenerDepartamentoPorIdAD _obtenerporId;

        public ObtenerCitasPorIdLN()
        {
            _obtenerporId = new ObtenerDepartamentoPorIdAD();
        }
        public DepartamentoDTO Obtener(int IdDepartamento)
        {
            DepartamentoTabla departamentoEnBaseDeDatos = _obtenerporId.Obtener(IdDepartamento);
            DepartamentoDTO elDepartamentoAMostrar = ConvertirAPersonaAMostrar(departamentoEnBaseDeDatos);
            return elDepartamentoAMostrar;
        }
        private DepartamentoDTO ConvertirAPersonaAMostrar(DepartamentoTabla departamentoEnBaseDeDatos)
        {
            return new DepartamentoDTO
            {
                IdDepartamento = departamentoEnBaseDeDatos.IdDepartamento,
                IdCliente = departamentoEnBaseDeDatos.IdCliente ?? 0            ,
                Nombre = departamentoEnBaseDeDatos.Nombre,
                Descripcion = departamentoEnBaseDeDatos.Descripcion,
                IdTipoDepartamento = departamentoEnBaseDeDatos.IdTipoDepartamento,
                Precio = departamentoEnBaseDeDatos.Precio,
                Estado = departamentoEnBaseDeDatos.Estado
            };
        }

    }
}
