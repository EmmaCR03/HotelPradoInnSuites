using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Eliminar;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Facturas.Eliminar
{
    public class EliminarFacturasAD : IEliminarFacturasAD
    {
        private readonly Contexto _contexto;

        public EliminarFacturasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Eliminar(Guid Id)
        {
            var factura = await _contexto.FacturasTabla
                .Where(f => f.Id == Id)
                .FirstOrDefaultAsync();

            if (factura != null)
            {
                _contexto.FacturasTabla.Remove(factura);
                _contexto.Entry(factura).State = System.Data.Entity.EntityState.Deleted;
                int cantidadDeDatosEliminados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosEliminados;
            }
            return 0;
        }
    }
}

