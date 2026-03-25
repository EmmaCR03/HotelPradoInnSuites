using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Temporadas.ObtenerPorId
{
    public class ObtenerTemporadasPorIdAD : IObtenerTemporadasPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerTemporadasPorIdAD()
        {
            _contexto = new Contexto();
        }

        public async Task<TemporadasTabla> Obtener(int IdTemporada)
        {
            return await _contexto.TemporadasTabla
                .Where(t => t.IdTemporada == IdTemporada)
                .FirstOrDefaultAsync();
        }
    }
}

