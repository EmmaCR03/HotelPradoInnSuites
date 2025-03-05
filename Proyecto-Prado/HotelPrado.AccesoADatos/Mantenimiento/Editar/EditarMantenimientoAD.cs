using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.Editar
{
    public class EditarMantenimientoAD : IEditarMantenimientoAD
    {
        Contexto _contexto;

        public EditarMantenimientoAD()
        {
            _contexto = new Contexto();
        }
        public async Task<int> Editar(MantenimientoTabla elMantenimientoAActualizar)
        {
            MantenimientoTabla elMantenimientoEnBaseDeDatos = _contexto.MantenimientoTabla
               .Where(elMantenimiento => elMantenimiento.IdMantenimiento == elMantenimientoAActualizar.IdMantenimiento)
               .FirstOrDefault();
            elMantenimientoEnBaseDeDatos.Descripcion = elMantenimientoAActualizar.Descripcion;
            elMantenimientoEnBaseDeDatos.Estado = elMantenimientoAActualizar.Estado;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }
    }
}

