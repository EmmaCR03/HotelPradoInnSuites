using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


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
            return _contexto.DepartamentoTabla
                .Include(d => d.TipoDepartamento)  // 🔹 Incluir la relación correctamente
                .FirstOrDefault(d => d.IdDepartamento == IdDepartamento);
        }

    }
}

