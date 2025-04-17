using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System.Data.Entity;
using System.Linq;
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
            MantenimientoTabla elMantenimientoEnBaseDeDatos = _contexto.MantenimientoTabla
                .Where(elMantenimiento => elMantenimiento.IdMantenimiento == elMantenimientoActualizar.IdMantenimiento)
                .FirstOrDefault();
            elMantenimientoEnBaseDeDatos.Descripcion = elMantenimientoActualizar.Descripcion;
            elMantenimientoEnBaseDeDatos.Estado = elMantenimientoActualizar.Estado;
            elMantenimientoEnBaseDeDatos.DepartamentoNombre = elMantenimientoActualizar.DepartamentoNombre;
            EntityState estado = _contexto.Entry(elMantenimientoEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }
    }
}
