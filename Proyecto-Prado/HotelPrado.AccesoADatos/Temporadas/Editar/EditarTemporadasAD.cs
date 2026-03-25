using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Temporadas.Editar
{
    public class EditarTemporadasAD : IEditarTemporadasAD
    {
        private readonly Contexto _contexto;

        public EditarTemporadasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(TemporadasTabla laTemporadaActualizar)
        {
            TemporadasTabla laTemporadaEnBaseDeDatos = _contexto.TemporadasTabla
                .Where(t => t.IdTemporada == laTemporadaActualizar.IdTemporada)
                .FirstOrDefault();

            if (laTemporadaEnBaseDeDatos != null)
            {
                laTemporadaEnBaseDeDatos.NumeroTemporada = laTemporadaActualizar.NumeroTemporada;
                laTemporadaEnBaseDeDatos.Descripcion = laTemporadaActualizar.Descripcion;
                laTemporadaEnBaseDeDatos.CodigoCuenta = laTemporadaActualizar.CodigoCuenta;
                laTemporadaEnBaseDeDatos.AumentaAl = laTemporadaActualizar.AumentaAl;

                EntityState estado = _contexto.Entry(laTemporadaEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            return 0;
        }
    }
}

