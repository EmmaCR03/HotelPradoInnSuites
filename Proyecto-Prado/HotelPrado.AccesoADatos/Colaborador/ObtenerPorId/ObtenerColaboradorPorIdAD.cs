using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.ObtenerPorId
{
    public class ObtenerColaboradorPorIdAD : IObtenerColaboradorPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerColaboradorPorIdAD()
        {
            _contexto = new Contexto();
        }

        // Método para obtener un colaborador por ID
        public async Task<ColaboradorTabla> Obtener(int IdColaborador)
        {
            return await _contexto.ColaboradorTabla
                                   .FirstOrDefaultAsync(d => d.IdColaborador == IdColaborador);
        }

        // Método para actualizar los datos de un colaborador
        public async Task<bool> ActualizarColaborador(ColaboradorTabla colaborador)
        {
            // Buscar el colaborador en la base de datos usando el ID
            var colaboradorExistente = await _contexto.ColaboradorTabla
                                                       .FirstOrDefaultAsync(c => c.IdColaborador == colaborador.IdColaborador);
            if (colaboradorExistente != null)
            {
                // Actualizar los campos
                colaboradorExistente.NombreColaborador = colaborador.NombreColaborador;
                colaboradorExistente.PrimerApellidoColaborador = colaborador.PrimerApellidoColaborador;
                colaboradorExistente.SegundoApellidoColaborador = colaborador.SegundoApellidoColaborador;
                colaboradorExistente.CedulaColaborador = colaborador.CedulaColaborador;
                colaboradorExistente.IngresoColaborador = colaborador.IngresoColaborador;
                colaboradorExistente.PuestoColaborador = colaborador.PuestoColaborador;
                colaboradorExistente.EstadoLaboral = colaborador.EstadoLaboral;

                // Guardar los cambios en la base de datos
                int filasAfectadas = await _contexto.SaveChangesAsync();
                return filasAfectadas > 0; // Si se actualizó correctamente, se devuelve true
            }

            return false; // Si no se encontró el colaborador, se devuelve false
        }
    }
}
