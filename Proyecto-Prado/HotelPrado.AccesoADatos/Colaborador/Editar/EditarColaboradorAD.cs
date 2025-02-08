using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.Editar
{
    public class EditarColaboradorAD : IEditarColaboradorAD
    {
        private readonly Contexto _contexto;

        public EditarColaboradorAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(ColaboradorTabla elColaboradorActualizar)
        {
            try
            {
                // Buscar el colaborador en la base de datos
                var elcolaboradorEnBaseDeDatos = await _contexto.ColaboradorTabla
                    .Where(c => c.IdColaborador == elColaboradorActualizar.IdColaborador)
                    .FirstOrDefaultAsync();


                // Actualizar los campos con los nuevos valores
                elcolaboradorEnBaseDeDatos.NombreColaborador = elColaboradorActualizar.NombreColaborador;
                elcolaboradorEnBaseDeDatos.PrimerApellidoColaborador = elColaboradorActualizar.PrimerApellidoColaborador;
                elcolaboradorEnBaseDeDatos.SegundoApellidoColaborador = elColaboradorActualizar.SegundoApellidoColaborador;
                elcolaboradorEnBaseDeDatos.CedulaColaborador = elColaboradorActualizar.CedulaColaborador;
                elcolaboradorEnBaseDeDatos.PuestoColaborador = elColaboradorActualizar.PuestoColaborador;
                elcolaboradorEnBaseDeDatos.IngresoColaborador = elColaboradorActualizar.IngresoColaborador;
                elcolaboradorEnBaseDeDatos.EstadoLaboral = elColaboradorActualizar.EstadoLaboral;

                // Cambiar el estado de la entidad a "Modified" para que Entity Framework realice el seguimiento del cambio
                _contexto.Entry(elcolaboradorEnBaseDeDatos).State = EntityState.Modified;

                // Guardar los cambios en la base de datos
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();

                // Devolver la cantidad de registros actualizados
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error
                throw new Exception("Error al editar el colaborador.", ex);
            }
        }
    }
}



