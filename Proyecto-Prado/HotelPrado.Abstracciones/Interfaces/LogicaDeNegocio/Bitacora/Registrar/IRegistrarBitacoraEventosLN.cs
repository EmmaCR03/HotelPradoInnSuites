using HotelPrado.Abstracciones.Modelos.Bitacora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar
{
    public interface IRegistrarBitacoraEventosLN
    {
        Task<int> RegistrarBitacora(BitacoraEventosDTO modelo);
    }
}
