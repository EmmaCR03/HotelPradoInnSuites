using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Convertir;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.Conversion
{
    public class ConvertirMantenimientoDTOAMantenimientoTabla : IConvertirMantenimientoDTOAMantenimientoTabla
    {
        public MantenimientoTabla Convertir(MantenimientoDTO elMantenimiento)
        {
            return new MantenimientoTabla
            {
                IdMantenimiento = elMantenimiento.IdMantenimiento,
                Descripcion = elMantenimiento.Descripcion,               
                Estado = elMantenimiento.Estado
            };
        }
    }
}
