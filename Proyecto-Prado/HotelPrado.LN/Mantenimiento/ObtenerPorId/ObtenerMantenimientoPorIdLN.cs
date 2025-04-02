using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.ObtenerPorId;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.ObtenerPorId
{
    public class ObtenerMantenimientoPorIdLN : IObtenerMantenimientoPorIdLN
    {
        IObtenerMantenimientoPorIdAD _obtenerporId;

        public ObtenerMantenimientoPorIdLN()
        {
            _obtenerporId = new ObtenerMantenimientoPorIdAD();
        }

        public async Task<MantenimientoDTO> Obtener(int IdMantenimiento)
        {
            // Llamada asíncrona para obtener el colaborador
            MantenimientoTabla mantenimientoEnBaseDeDatos = await _obtenerporId.Obtener(IdMantenimiento);
            MantenimientoDTO elMantenimientoAMostrar = ConvertirAMantenimientoAMostrar(mantenimientoEnBaseDeDatos);
            return elMantenimientoAMostrar;
        }

        private MantenimientoDTO ConvertirAMantenimientoAMostrar(MantenimientoTabla mantenimientoEnBaseDeDatos)
        {
            return new MantenimientoDTO
            {
                IdMantenimiento = mantenimientoEnBaseDeDatos.IdMantenimiento,
                Descripcion = mantenimientoEnBaseDeDatos.Descripcion,
                Estado = mantenimientoEnBaseDeDatos.Estado,
                idDepartamento = mantenimientoEnBaseDeDatos.idDepartamento,
                idHabitacion = mantenimientoEnBaseDeDatos.idHabitacion
            };
        }
    }
}