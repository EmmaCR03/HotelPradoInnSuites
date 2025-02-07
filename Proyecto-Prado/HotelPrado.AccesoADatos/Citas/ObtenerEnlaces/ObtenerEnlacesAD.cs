using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Citas.ObtenerEnlaces
{
    public class ObtenerEnlacesAD : IObtenerEnlacesAD
    {
        private readonly Contexto _contexto;

        public ObtenerEnlacesAD()
        {
            _contexto = new Contexto();
        }

        public List<CitasTabla> ObtenerCitas()
        {
            return _contexto.CitasTabla.ToList();
        }
    }
}
