using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.ObtenerPorId
{
    public class ObtenerMantenimientoPorIdAD : IObtenerMantenimientoPorIdAD
    {
        Contexto _contexto;

        public ObtenerMantenimientoPorIdAD()
        {
            _contexto = new Contexto();
        }

        public MantenimientoTabla Obtener(int IdMantenimiento)
        {
            return _contexto.MantenimientoTabla.FirstOrDefault(d => d.IdMantenimiento == IdMantenimiento);
        }
    }
}
