using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Bitacora.Registrar
{
    public class RegistrarBitacoraEventosAD : IRegistrarBitacoraEventosAD
    {
        Contexto _contexto;

        public RegistrarBitacoraEventosAD() 
        {
            _contexto = new Contexto();
        }
        public async Task<int> RegistrarEvento(BitacoraTabla evento)
        {
            try
            {
                _contexto.BitacoraTabla.Add(evento);
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
