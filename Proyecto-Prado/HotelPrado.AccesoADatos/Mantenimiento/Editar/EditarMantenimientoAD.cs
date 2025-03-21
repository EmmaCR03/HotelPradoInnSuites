using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
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
        private readonly Contexto _contexto;

        public EditarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(MantenimientoTabla elMantenimientoActualizar)
        {
            try
            {
                // Buscar el colaborador en la base de datos
                var elMantenimientoEnBaseDeDatos = await _contexto.MantenimientoTabla
                    .Where(c => c.IdMantenimiento == elMantenimientoActualizar.IdMantenimiento)
                    .FirstOrDefaultAsync();


                // Actualizar los campos con los nuevos valores
                elMantenimientoEnBaseDeDatos.Descripcion = elMantenimientoActualizar.Descripcion;
                elMantenimientoEnBaseDeDatos.Estado = elMantenimientoActualizar.Estado;


                // Cambiar el estado de la entidad a "Modified" para que Entity Framework realice el seguimiento del cambio
                _contexto.Entry(elMantenimientoEnBaseDeDatos).State = EntityState.Modified;

                // Guardar los cambios en la base de datos
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();

                // Devolver la cantidad de registros actualizados
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error
                throw new Exception("Error al editar el Mantenimiento.", ex);
            }
        }
    }
}

