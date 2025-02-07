using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.General.Fecha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.General.Fecha
{
    public class FechaActual : IFechaActual
    {
        public DateTime ObtenerFecha() 
        {
            return DateTime.Now;
        }
    }
}
