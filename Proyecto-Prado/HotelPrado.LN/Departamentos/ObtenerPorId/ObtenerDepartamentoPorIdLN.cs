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
    public class ObtenerDepartamentoPorIdLN : IObtenerDepartamentoPorIdLN
    {
        IObtenerDepartamentoPorIdAD _obtenerporId;

        public ObtenerDepartamentoPorIdLN()
        {
            _obtenerporId = new ObtenerDepartamentoPorIdAD();
        }
        public DepartamentoDTO Obtener(int IdDepartamento)
        {
            DepartamentoTabla departamentoEnBaseDeDatos = _obtenerporId.Obtener(IdDepartamento);
            DepartamentoDTO elDepartamentoAMostrar = ConvertirADepartamentoAMostrar(departamentoEnBaseDeDatos);
            return elDepartamentoAMostrar;
        }
        private DepartamentoDTO ConvertirADepartamentoAMostrar(DepartamentoTabla departamentoEnBaseDeDatos)
        {
            return new DepartamentoDTO
            {
                IdDepartamento = departamentoEnBaseDeDatos.IdDepartamento,
                IdCliente = departamentoEnBaseDeDatos.IdCliente ?? 0,
                Nombre = departamentoEnBaseDeDatos.Nombre,
                Descripcion = departamentoEnBaseDeDatos.Descripcion,
                IdTipoDepartamento = departamentoEnBaseDeDatos.IdTipoDepartamento,
                Precio = departamentoEnBaseDeDatos.Precio,
                Estado = departamentoEnBaseDeDatos.Estado,
                UrlImagenes = departamentoEnBaseDeDatos.UrlImagenes ?? "", // 🔹 Evitar que sea null
                NumeroHabitaciones = departamentoEnBaseDeDatos.TipoDepartamento != null ? departamentoEnBaseDeDatos.TipoDepartamento.NumeroHabitaciones : 0,
                Amueblado = departamentoEnBaseDeDatos.TipoDepartamento != null && departamentoEnBaseDeDatos.TipoDepartamento.Amueblado
            };
        }


    }
}
