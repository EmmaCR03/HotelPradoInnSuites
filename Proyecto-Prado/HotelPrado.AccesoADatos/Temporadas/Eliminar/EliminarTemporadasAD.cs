using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Eliminar;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Temporadas.Eliminar
{
    public class EliminarTemporadasAD : IEliminarTemporadasAD
    {
        private readonly Contexto _contexto;

        public EliminarTemporadasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Eliminar(int IdTemporada)
        {
            var temporada = await _contexto.TemporadasTabla
                .Where(t => t.IdTemporada == IdTemporada)
                .FirstOrDefaultAsync();

            if (temporada != null)
            {
                _contexto.TemporadasTabla.Remove(temporada);
                _contexto.Entry(temporada).State = System.Data.Entity.EntityState.Deleted;
                int cantidadDeDatosEliminados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosEliminados;
            }
            return 0;
        }
    }
}

