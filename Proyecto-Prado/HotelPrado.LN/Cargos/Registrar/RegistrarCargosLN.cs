using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Cargos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using HotelPrado.AccesoADatos.Cargos.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Cargos.Registrar
{
    public class RegistrarCargosLN : IRegistrarCargosLN
    {
        private readonly IRegistrarCargosAD _registrarCargosAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarCargosLN()
        {
            _registrarCargosAD = new RegistrarCargosAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(CargosDTO modelo)
        {
            try
            {
                // Convertir DTO a Tabla
                var cargoTabla = ConvertirObjetoCargosTabla(modelo);

                // Guardar el Cargo en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarCargosAD.Guardar(cargoTabla);

                // Preparar datos para registrar en la bitácora
                string datos = $@"{{
                    ""IdCargo"": {modelo.IdCargo},
                    ""DescripcionExtra"": ""{modelo.DescripcionExtra}"",
                    ""MontoTotal"": {modelo.MontoTotal},
                    ""FechaHora"": ""{modelo.FechaHora}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró un nuevo Cargo en la base de datos.",
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
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar el Cargo.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
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

