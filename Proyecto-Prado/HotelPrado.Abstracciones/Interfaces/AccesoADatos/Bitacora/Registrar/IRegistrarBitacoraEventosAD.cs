using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Bitacora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Registrar
{
    public interface IRegistrarBitacoraEventosAD
    {
        Task<int> RegistrarEvento(BitacoraTabla evento);
    }
}
