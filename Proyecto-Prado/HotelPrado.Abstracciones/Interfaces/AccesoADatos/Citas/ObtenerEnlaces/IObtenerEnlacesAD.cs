using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerEnlaces
{
    public interface IObtenerEnlacesAD
    {
         List<CitasTabla> ObtenerCitas();
    }
}
