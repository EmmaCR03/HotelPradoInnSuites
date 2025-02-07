using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Departamentos.ObtenerPorId
{
    public class ObtenerDepartamentoPorIdAD : IObtenerDepartamentoPorIdAD
    {
        Contexto _contexto;

        public ObtenerDepartamentoPorIdAD() 
        {
            _contexto = new Contexto();
        }

        public DepartamentoTabla Obtener(int IdDepartamento)
        {
            return _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento);
        }

    }
}

