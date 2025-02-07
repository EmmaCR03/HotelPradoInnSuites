using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Listar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using HotelPrado.AccesoADatos.Bitacora.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Bitacora
{
    public class ListarBitacoraEventosLN : IListarBitacoraEventosLN
    {
        IListarBitacoraEventosAD _listarBitacoraEventosAD;

        public ListarBitacoraEventosLN()
        {
            _listarBitacoraEventosAD = new ListarBitacoraEventosAD();
        }

        public List<BitacoraEventosDTO> ListarBitacora()
        {
            List<BitacoraEventosDTO> laListasDeBitacora = _listarBitacoraEventosAD.Listar();
            return laListasDeBitacora;
        }
        private List<BitacoraEventosDTO> ObtenerLaListaConvertida(List<BitacoraTabla> laListasDeBitacoras)
        {
            List<BitacoraEventosDTO> laListaDeBitacoras = new List<BitacoraEventosDTO>();
            foreach (BitacoraTabla laBitacora in laListasDeBitacoras)
            {
                laListaDeBitacoras.Add(ConvertirObjetoDTO(laBitacora));
            }
            return laListaDeBitacoras;
        }
        private BitacoraEventosDTO ConvertirObjetoDTO(BitacoraTabla laBitacora)
        {

            return new BitacoraEventosDTO
            {
                IdEvento = laBitacora.IdEvento,
                TablaDeEvento = laBitacora.TablaDeEvento,
                TipoDeEvento = laBitacora.TipoDeEvento,
                FechaDeEvento = laBitacora.FechaDeEvento.ToString("dd-MM-yyyy hh:mm tt"),
                DescripcionDeEvento = laBitacora.DescripcionDeEvento,
                StackTrace = laBitacora.StackTrace,
                DatosAnteriores = laBitacora.DatosAnteriores,
                DatosPosteriores = laBitacora.DatosPosteriores

            };
        }
    }
}

