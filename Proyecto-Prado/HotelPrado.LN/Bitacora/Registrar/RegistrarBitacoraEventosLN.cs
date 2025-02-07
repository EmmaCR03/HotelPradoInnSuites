using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.General.Fecha;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using HotelPrado.AccesoADatos.Bitacora.Registrar;
using HotelPrado.LN.General.Fecha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Bitacora.Registrar
{
    public class RegistrarBitacoraEventosLN : IRegistrarBitacoraEventosLN
    {
        IRegistrarBitacoraEventosAD _registrarBitacoraAD;
        IFechaActual _fecha;

        public RegistrarBitacoraEventosLN()
        {
            _registrarBitacoraAD = new RegistrarBitacoraEventosAD();
            _fecha = new FechaActual();
        }
        public async Task<int> RegistrarBitacora(BitacoraEventosDTO modelo)
        {
            int cantidadDeDAtosAlmacenados = await _registrarBitacoraAD.RegistrarEvento(ConvertirObjetoBitacoraTabla(modelo));
            return cantidadDeDAtosAlmacenados;
        }

        public BitacoraTabla ConvertirObjetoBitacoraTabla(BitacoraEventosDTO Bitacora)
        {
            return new BitacoraTabla()
            {
                IdEvento = Bitacora.IdEvento,
                TablaDeEvento = Bitacora.TablaDeEvento,
                TipoDeEvento = Bitacora.TipoDeEvento,
                FechaDeEvento = _fecha.ObtenerFecha(),
                DescripcionDeEvento = Bitacora.DescripcionDeEvento,
                StackTrace = Bitacora.StackTrace,
                DatosAnteriores = Bitacora.DatosAnteriores,
                DatosPosteriores = Bitacora.DatosPosteriores
            };
        }
    }
}
