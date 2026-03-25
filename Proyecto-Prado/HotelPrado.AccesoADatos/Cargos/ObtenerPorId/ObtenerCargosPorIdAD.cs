using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Cargos.ObtenerPorId
{
    public class ObtenerCargosPorIdAD : IObtenerCargosPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerCargosPorIdAD()
        {
            _contexto = new Contexto();
        }

        public async Task<CargosTabla> Obtener(int IdCargo)
        {
            return await _contexto.CargosTabla
                .Where(c => c.IdCargo == IdCargo)
                .FirstOrDefaultAsync();
        }
    }
}

