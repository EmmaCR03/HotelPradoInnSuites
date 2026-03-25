using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Eliminar;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Cargos.Eliminar
{
    public class EliminarCargosAD : IEliminarCargosAD
    {
        private readonly Contexto _contexto;

        public EliminarCargosAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Eliminar(int IdCargo)
        {
            var cargo = await _contexto.CargosTabla
                .Where(c => c.IdCargo == IdCargo)
                .FirstOrDefaultAsync();

            if (cargo != null)
            {
                _contexto.CargosTabla.Remove(cargo);
                _contexto.Entry(cargo).State = System.Data.Entity.EntityState.Deleted;
                int cantidadDeDatosEliminados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosEliminados;
            }
            return 0;
        }
    }
}

