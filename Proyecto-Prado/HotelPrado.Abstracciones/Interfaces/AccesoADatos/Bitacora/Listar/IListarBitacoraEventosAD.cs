using HotelPrado.Abstracciones.Modelos.Bitacora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Bitacora.Listar
{
    public interface IListarBitacoraEventosAD
    {
        List<BitacoraEventosDTO> Listar();
    }
}
