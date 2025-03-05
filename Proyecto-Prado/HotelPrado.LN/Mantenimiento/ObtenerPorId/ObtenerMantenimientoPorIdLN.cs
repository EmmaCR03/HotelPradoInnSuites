using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.ObtenerPorId
{
    public class ObtenerMantenimientoPorIdLN : IObtenerMantenimientoPorIdLN
    {
        IObtenerMantenimientoPorIdAD _obtenerId;

        public ObtenerMantenimientoPorIdLN()
        {
            _obtenerId = new ObtenerMantenimientoPorIdAD();
        }

        public MantenimientoDTO Obtener(int IdMantenimiento)
        {
            MantenimientoTabla mantenimientoEnBaseDeDatos = _obtenerId.Obtener(IdMantenimiento);
            MantenimientoDTO elMantenimientoAMostrar = ConvertirAMantenimientoAMostrar(mantenimientoEnBaseDeDatos);
            return elMantenimientoAMostrar;
        }
        private MantenimientoDTO ConvertirAMantenimientoAMostrar(MantenimientoTabla mantenimientoEnBaseDeDatos)
        {
            return new MantenimientoDTO
            {
                IdMantenimiento = mantenimientoEnBaseDeDatos.IdMantenimiento,
                Descripcion = mantenimientoEnBaseDeDatos.Descripcion,
                Estado = mantenimientoEnBaseDeDatos.Estado
            };
        }

    }
}
