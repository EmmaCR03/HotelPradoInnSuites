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
        Contexto _contexto;

        public EditarColaboradorAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(ColaboradorTabla elColaboradorActualizar)
        {
            try
            {
                ColaboradorTabla elcolaboradorEnBaseDeDatos = _contexto.ColaboradorTabla
                    .Where(elColaborador => elColaborador.IdColaborador == elColaboradorActualizar.IdColaborador)
                    .FirstOrDefault();

                if (elcolaboradorEnBaseDeDatos == null)
                {
                    throw new Exception("El colaborador no fue encontrado.");
                }

                // Actualización de los campos
                elcolaboradorEnBaseDeDatos.NombreColaborador = elColaboradorActualizar.NombreColaborador;
                elcolaboradorEnBaseDeDatos.PrimerApellidoColaborador = elColaboradorActualizar.PrimerApellidoColaborador;
                elcolaboradorEnBaseDeDatos.SegundoApellidoColaborador = elColaboradorActualizar.SegundoApellidoColaborador;
                elcolaboradorEnBaseDeDatos.CedulaColaborador = (int)elColaboradorActualizar.CedulaColaborador;
                elcolaboradorEnBaseDeDatos.PuestoColaborador = elColaboradorActualizar.PuestoColaborador;
                elcolaboradorEnBaseDeDatos.IngresoColaborador = elColaboradorActualizar.IngresoColaborador;

                // Estado de la entidad
                EntityState estado = _contexto.Entry(elColaboradorActualizar).State = System.Data.Entity.EntityState.Modified;

                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Log de error
                // Puedes agregar un log para registrar el error
                throw new Exception("Error al editar el colaborador.", ex);
            }
        }
    }
}



