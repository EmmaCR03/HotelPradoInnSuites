using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Listar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Bitacora.Listar
{
    public class ListarBitacoraEventosAD : IListarBitacoraEventosAD
    {
        Contexto _contexto;

        public ListarBitacoraEventosAD() 
        {
            _contexto = new Contexto();
        }

        public List<BitacoraEventosDTO> Listar() 
        {
            List<BitacoraEventosDTO> laListaDeBitacora = (from laBitacora in _contexto.BitacoraTabla
                                                          select new BitacoraEventosDTO
                                                          {
                                                              IdEvento = laBitacora.IdEvento,
                                                              TablaDeEvento = laBitacora.TablaDeEvento,
                                                              TipoDeEvento = laBitacora.TipoDeEvento,
                                                              FechaDeEvento = laBitacora.FechaDeEvento.ToString(),
                                                              DescripcionDeEvento = laBitacora.DescripcionDeEvento,
                                                              StackTrace = laBitacora.StackTrace,
                                                              DatosAnteriores = laBitacora.DatosAnteriores,
                                                              DatosPosteriores = laBitacora.DatosPosteriores
                                                          }).ToList();

            return laListaDeBitacora;
        }

    }
}
