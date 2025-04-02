using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.ObtenerPorId
{
    public class ObtenerMantenimientoPorIdAD : IObtenerMantenimientoPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerMantenimientoPorIdAD()
        {
            _contexto = new Contexto();
        }

        public async Task<MantenimientoTabla> Obtener(int IdMantenimiento)
        {
            return await _contexto.MantenimientoTabla
                                   .FirstOrDefaultAsync(d => d.IdMantenimiento == IdMantenimiento);
        }

        public async Task<bool> ActualizarMantenimiento(MantenimientoTabla mantenimiento)
        {
            var mantenimientoExistente = await _contexto.MantenimientoTabla
                                                       .FirstOrDefaultAsync(c => c.IdMantenimiento == mantenimiento.IdMantenimiento);
            if (mantenimientoExistente != null)
            {
                // Actualizar los campos
                mantenimientoExistente.Descripcion = mantenimiento.Descripcion;
                mantenimientoExistente.Estado = mantenimiento.Estado;
                mantenimientoExistente.idDepartamento = mantenimiento.idDepartamento;
                mantenimientoExistente.idHabitacion = mantenimiento.idHabitacion;

                // Guardar los cambios en la base de datos
                int filasAfectadas = await _contexto.SaveChangesAsync();
                return filasAfectadas > 0; // Si se actualizó correctamente, se devuelve true
            }

            return false; 
        }
    }
}
