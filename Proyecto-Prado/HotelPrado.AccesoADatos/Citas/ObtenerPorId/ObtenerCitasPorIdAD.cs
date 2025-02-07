using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Citas.ObtenerPorId
{
    public class ObtenerCitasPorIdAD : IObtenerCitasPorIdAD
    {
        Contexto _contexto;

        public ObtenerCitasPorIdAD() 
        {
            _contexto = new Contexto();
        }

        public CitasTabla Obtener(int IdCita) 
        {
            return _contexto.CitasTabla.FirstOrDefault(d => d.IdCita == IdCita);
        }
    }
}
