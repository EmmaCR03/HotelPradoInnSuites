using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Convertir
{
    public interface IConvertirMantenimientoDTOAMantenimientoTabla
    {
       MantenimientoTabla Convertir(MantenimientoDTO elMantenimiento);
    }
}
