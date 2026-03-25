using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Cargos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using HotelPrado.AccesoADatos.Cargos.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Cargos.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Cargos.Editar
{
    public class EditarCargosLN : IEditarCargosLN
    {
        private readonly IEditarCargosAD _editarCargosAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerCargosPorIdLN _obtenerCargosPorIdLN;

        public EditarCargosLN()
        {
            _editarCargosAD = new EditarCargosAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerCargosPorIdLN = new ObtenerCargosPorIdLN();
        }

        public async Task<int> Actualizar(CargosDTO elCargoEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerCargosPorIdLN.Obtener(elCargoEnVista.IdCargo);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"{{
                    ""IdCargo"": {datosAnteriores.IdCargo},
                    ""DescripcionExtra"": ""{datosAnteriores.DescripcionExtra}"",
                    ""MontoTotal"": {datosAnteriores.MontoTotal}
                }}";

                // Convertir el DTO a la entidad para su actualización
                var cargoTabla = ConvertirObjetoCargosTabla(elCargoEnVista);

                // Realizar la actualización
                int cantidadDeDatosActualizados = await _editarCargosAD.Editar(cargoTabla);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"{{
                    ""IdCargo"": {elCargoEnVista.IdCargo},
                    ""DescripcionExtra"": ""{elCargoEnVista.DescripcionExtra}"",
                    ""MontoTotal"": {elCargoEnVista.MontoTotal}
                }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloCargos",
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
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloCargos",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);
                throw;
            }
        }

        private CargosTabla ConvertirObjetoCargosTabla(CargosDTO elCargo)
        {
            return new CargosTabla
            {
                IdCargo = elCargo.IdCargo,
                IdCheckIn = elCargo.IdCheckIn,
                CodigoExtra = elCargo.CodigoExtra,
                DescripcionExtra = elCargo.DescripcionExtra,
                NumeroDocumento = elCargo.NumeroDocumento,
                MontoCargo = elCargo.MontoCargo,
                MontoServicio = elCargo.MontoServicio,
                ImpuestoVenta = elCargo.ImpuestoVenta,
                ImpuestoHotel = elCargo.ImpuestoHotel,
                ImpuestoServicio = elCargo.ImpuestoServicio,
                MontoTotal = elCargo.MontoTotal,
                QuienPaga = elCargo.QuienPaga,
                FechaHora = elCargo.FechaHora,
                NumeroEmpleado = elCargo.NumeroEmpleado,
                Cancelado = elCargo.Cancelado,
                Notas = elCargo.Notas,
                EnFacturaExtras = elCargo.EnFacturaExtras,
                CuentaError = elCargo.CuentaError,
                NumeroCierre = elCargo.NumeroCierre,
                PagoImpuesto = elCargo.PagoImpuesto,
                TipoCambio = elCargo.TipoCambio,
                FechaTraslado = elCargo.FechaTraslado,
                Facturar = elCargo.Facturar,
                Secuencia = elCargo.Secuencia,
                NoContable = elCargo.NoContable,
                NumeroFolioOriginal = elCargo.NumeroFolioOriginal
            };
        }
    }
}

