using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Facturas.ObtenerPorId
{
    public class ObtenerFacturasPorIdAD : IObtenerFacturasPorIdAD
    {
        private readonly Contexto _contexto;

        public ObtenerFacturasPorIdAD()
        {
            _contexto = new Contexto();
        }

        public async Task<FacturasTabla> Obtener(Guid Id)
        {
            return await _contexto.FacturasTabla
                .Where(f => f.Id == Id)
                .FirstOrDefaultAsync();
        }
    }
}

