using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Facturas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using HotelPrado.AccesoADatos.Facturas.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Facturas.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Facturas.Editar
{
    public class EditarFacturasLN : IEditarFacturasLN
    {
        private readonly IEditarFacturasAD _editarFacturasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerFacturasPorIdLN _obtenerFacturasPorIdLN;

        public EditarFacturasLN()
        {
            _editarFacturasAD = new EditarFacturasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerFacturasPorIdLN = new ObtenerFacturasPorIdLN();
        }

        public async Task<int> Actualizar(FacturasDTO laFacturaEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerFacturasPorIdLN.Obtener(laFacturaEnVista.Id);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"{{
                    ""Id"": ""{datosAnteriores.Id}"",
                    ""Clave"": ""{datosAnteriores.Clave}"",
                    ""NumeroConsecutivo"": ""{datosAnteriores.NumeroConsecutivo}"",
                    ""TotalComprobante"": {datosAnteriores.TotalComprobante}
                }}";

                // Convertir el DTO a la entidad para su actualización
                var facturaTabla = ConvertirObjetoFacturasTabla(laFacturaEnVista);

                // Realizar la actualización
                int cantidadDeDatosActualizados = await _editarFacturasAD.Editar(facturaTabla);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"{{
                    ""Id"": ""{laFacturaEnVista.Id}"",
                    ""Clave"": ""{laFacturaEnVista.Clave}"",
                    ""NumeroConsecutivo"": ""{laFacturaEnVista.NumeroConsecutivo}"",
                    ""TotalComprobante"": {laFacturaEnVista.TotalComprobante}
                }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloFacturas",
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
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloFacturas",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);
                throw;
            }
        }

        private FacturasTabla ConvertirObjetoFacturasTabla(FacturasDTO laFactura)
        {
            return new FacturasTabla
            {
                Id = laFactura.Id,
                Clave = laFactura.Clave,
                NumeroConsecutivo = laFactura.NumeroConsecutivo,
                FechaEmision = laFactura.FechaEmision,
                EmisorId = laFactura.EmisorId,
                ReceptorId = laFactura.ReceptorId,
                TotalVenta = laFactura.TotalVenta,
                TotalDescuento = laFactura.TotalDescuento,
                TotalImpuesto = laFactura.TotalImpuesto,
                TotalComprobante = laFactura.TotalComprobante,
                Estado = laFactura.Estado,
                MedioPago = laFactura.MedioPago,
                FechaAprobacion = laFactura.FechaAprobacion,
                Activo = laFactura.Activo,
                NumeroFactura = laFactura.NumeroFactura,
                IdCheckIn = laFactura.IdCheckIn,
                IdEmpleado = laFactura.IdEmpleado,
                FechaHoraFactura = laFactura.FechaHoraFactura,
                TotalConsumos = laFactura.TotalConsumos,
                ImpuestoICT = laFactura.ImpuestoICT,
                ImpuestoServicio = laFactura.ImpuestoServicio,
                ImpuestoVentas = laFactura.ImpuestoVentas,
                TotalGeneral = laFactura.TotalGeneral,
                QuienPaga = laFactura.QuienPaga,
                Cerrado = laFactura.Cerrado,
                EnFacturaExtras = laFactura.EnFacturaExtras,
                FechaModificacion = DateTime.Now
            };
        }
    }
}

