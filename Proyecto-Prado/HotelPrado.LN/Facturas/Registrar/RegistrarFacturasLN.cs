using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Facturas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using HotelPrado.AccesoADatos.Facturas.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Facturas.Registrar
{
    public class RegistrarFacturasLN : IRegistrarFacturasLN
    {
        private readonly IRegistrarFacturasAD _registrarFacturasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarFacturasLN()
        {
            _registrarFacturasAD = new RegistrarFacturasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(FacturasDTO modelo)
        {
            try
            {
                // Convertir DTO a Tabla
                var facturaTabla = ConvertirObjetoFacturasTabla(modelo);

                // Guardar la Factura en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarFacturasAD.Guardar(facturaTabla);

                // Preparar datos para registrar en la bitácora
                string datos = $@"{{
                    ""Id"": ""{modelo.Id}"",
                    ""Clave"": ""{modelo.Clave}"",
                    ""NumeroConsecutivo"": ""{modelo.NumeroConsecutivo}"",
                    ""TotalComprobante"": {modelo.TotalComprobante}
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva Factura en la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = "NA",
                    DatosPosteriores = datos
                };

                // Registrar el evento en la bitácora
                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar la Factura.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                throw;
            }
        }

        private FacturasTabla ConvertirObjetoFacturasTabla(FacturasDTO laFactura)
        {
            return new FacturasTabla
            {
                Id = laFactura.Id == Guid.Empty ? Guid.NewGuid() : laFactura.Id,
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
                FechaCreacion = laFactura.FechaCreacion ?? DateTime.Now,
                FechaModificacion = laFactura.FechaModificacion ?? DateTime.Now
            };
        }
    }
}

