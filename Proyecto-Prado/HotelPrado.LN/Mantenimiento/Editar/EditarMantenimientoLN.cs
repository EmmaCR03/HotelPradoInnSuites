using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Convertir;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Mantenimiento.Conversion;
using HotelPrado.LN.Mantenimiento.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.Editar
{
    public class EditarMantenimientoLN : IEditarMantenimientoLN
    {
        private readonly IConvertirMantenimientoDTOAMantenimientoTabla _convertir;
        private readonly IEditarMantenimientoAD _editarMantenimiento;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerMantenimientoPorIdLN _obtenerMantenimientoPorIdLN;

        public EditarMantenimientoLN()
        {
            _convertir = new ConvertirMantenimientoDTOAMantenimientoTabla();
            _editarMantenimiento = new EditarMantenimientoAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerMantenimientoPorIdLN = new ObtenerMantenimientoPorIdLN();
        }

        public async Task<int> Actualizar(MantenimientoDTO elMantenimientoEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerMantenimientoPorIdLN.Obtener(elMantenimientoEnVista.IdMantenimiento);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
            {{
                ""IdMantenimiento"": {datosAnteriores.IdMantenimiento},
                ""Descripcion"": ""{datosAnteriores.Descripcion}"",
                ""Estado"": ""{datosAnteriores.Estado}"",
                ""idDepartamento"": ""{datosAnteriores.idDepartamento}"",
                ""DepartamentoNombre"": ""{datosAnteriores.DepartamentoNombre}""
            }}";

                // Convertir el DTO a la entidad para su actualización
                var mantenimiento = _convertir.Convertir(elMantenimientoEnVista);

                // Realizar la actualización con el método 'Update'
                int cantidadDeDatosActualizados = await _editarMantenimiento.Editar(mantenimiento);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
            {{
                ""IdMantenimiento"": {elMantenimientoEnVista.IdMantenimiento},
                ""Descripcion"": ""{elMantenimientoEnVista.Descripcion}"",
                ""Estado"": ""{elMantenimientoEnVista.Estado}"",
                ""idDepartamento"": ""{elMantenimientoEnVista.idDepartamento}"",
                ""DepartamentoNombre"": ""{elMantenimientoEnVista.DepartamentoNombre}""
            }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloMantenimiento",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosAnterioresJson,
                    DatosPosteriores = datosPosterioresJson
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                // Manejo de errores: registrar el evento en la bitácora
                var bitacoraError = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloMantenimiento",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);

                throw; // Propagar el error después de registrarlo
            }
        }
    }
}

