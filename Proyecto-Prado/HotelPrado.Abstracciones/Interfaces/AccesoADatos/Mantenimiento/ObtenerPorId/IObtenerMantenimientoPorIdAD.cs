using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId
{
    public interface IObtenerMantenimientoPorIdAD
    {
        MantenimientoTabla Obtener(int IdMantenimiento);
    }
}
