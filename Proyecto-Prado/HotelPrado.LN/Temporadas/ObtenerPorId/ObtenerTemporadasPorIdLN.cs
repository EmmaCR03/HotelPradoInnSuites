using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Temporadas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using HotelPrado.AccesoADatos.Temporadas.ObtenerPorId;
using System.Threading.Tasks;

namespace HotelPrado.LN.Temporadas.ObtenerPorId
{
    public class ObtenerTemporadasPorIdLN : IObtenerTemporadasPorIdLN
    {
        IObtenerTemporadasPorIdAD _obtenerPorId;

        public ObtenerTemporadasPorIdLN()
        {
            _obtenerPorId = new ObtenerTemporadasPorIdAD();
        }

        public async Task<TemporadasDTO> Obtener(int IdTemporada)
        {
            TemporadasTabla temporadaEnBaseDeDatos = await _obtenerPorId.Obtener(IdTemporada);
            TemporadasDTO laTemporadaAMostrar = ConvertirATemporadaAMostrar(temporadaEnBaseDeDatos);
            return laTemporadaAMostrar;
        }

        private TemporadasDTO ConvertirATemporadaAMostrar(TemporadasTabla temporadaEnBaseDeDatos)
        {
            if (temporadaEnBaseDeDatos == null)
                return null;

            return new TemporadasDTO
            {
                IdTemporada = temporadaEnBaseDeDatos.IdTemporada,
                NumeroTemporada = temporadaEnBaseDeDatos.NumeroTemporada,
                Descripcion = temporadaEnBaseDeDatos.Descripcion,
                CodigoCuenta = temporadaEnBaseDeDatos.CodigoCuenta,
                AumentaAl = temporadaEnBaseDeDatos.AumentaAl
            };
        }
    }
}

