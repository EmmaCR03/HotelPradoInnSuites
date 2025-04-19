using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Departamentos.Editar
{
    public class EditarDepartamentoAD : IEditarDepartamentoAD
    {
        Contexto _contexto;

        public EditarDepartamentoAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(DepartamentoTabla elDepartamentoActualizar)
        {
            DepartamentoTabla eldepartamentoEnBaseDeDatos = _contexto.DepartamentoTabla
                .Where(elDepartamento => elDepartamento.IdDepartamento == elDepartamentoActualizar.IdDepartamento)
                .FirstOrDefault();
            eldepartamentoEnBaseDeDatos.Nombre = elDepartamentoActualizar.Nombre;
            eldepartamentoEnBaseDeDatos.Descripcion = elDepartamentoActualizar.Descripcion;
            eldepartamentoEnBaseDeDatos.IdTipoDepartamento = elDepartamentoActualizar.IdTipoDepartamento;
            eldepartamentoEnBaseDeDatos.Precio = elDepartamentoActualizar.Precio;
            eldepartamentoEnBaseDeDatos.Estado = elDepartamentoActualizar.Estado;
            EntityState estado = _contexto.Entry(eldepartamentoEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }
    }
}
