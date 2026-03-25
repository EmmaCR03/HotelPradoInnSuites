using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Cargos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using HotelPrado.AccesoADatos.Cargos.ObtenerPorId;
using System.Threading.Tasks;

namespace HotelPrado.LN.Cargos.ObtenerPorId
{
    public class ObtenerCargosPorIdLN : IObtenerCargosPorIdLN
    {
        IObtenerCargosPorIdAD _obtenerPorId;

        public ObtenerCargosPorIdLN()
        {
            _obtenerPorId = new ObtenerCargosPorIdAD();
        }

        public async Task<CargosDTO> Obtener(int IdCargo)
        {
            CargosTabla cargoEnBaseDeDatos = await _obtenerPorId.Obtener(IdCargo);
            CargosDTO elCargoAMostrar = ConvertirACargoAMostrar(cargoEnBaseDeDatos);
            return elCargoAMostrar;
        }

        private CargosDTO ConvertirACargoAMostrar(CargosTabla cargoEnBaseDeDatos)
        {
            if (cargoEnBaseDeDatos == null)
                return null;

            return new CargosDTO
            {
                IdCargo = cargoEnBaseDeDatos.IdCargo,
                IdCheckIn = cargoEnBaseDeDatos.IdCheckIn,
                CodigoExtra = cargoEnBaseDeDatos.CodigoExtra,
                DescripcionExtra = cargoEnBaseDeDatos.DescripcionExtra,
                NumeroDocumento = cargoEnBaseDeDatos.NumeroDocumento,
                MontoCargo = cargoEnBaseDeDatos.MontoCargo,
                MontoServicio = cargoEnBaseDeDatos.MontoServicio,
                ImpuestoVenta = cargoEnBaseDeDatos.ImpuestoVenta,
                ImpuestoHotel = cargoEnBaseDeDatos.ImpuestoHotel,
                ImpuestoServicio = cargoEnBaseDeDatos.ImpuestoServicio,
                MontoTotal = cargoEnBaseDeDatos.MontoTotal,
                QuienPaga = cargoEnBaseDeDatos.QuienPaga,
                FechaHora = cargoEnBaseDeDatos.FechaHora,
                NumeroEmpleado = cargoEnBaseDeDatos.NumeroEmpleado,
                Cancelado = cargoEnBaseDeDatos.Cancelado,
                Notas = cargoEnBaseDeDatos.Notas,
                EnFacturaExtras = cargoEnBaseDeDatos.EnFacturaExtras,
                CuentaError = cargoEnBaseDeDatos.CuentaError,
                NumeroCierre = cargoEnBaseDeDatos.NumeroCierre,
                PagoImpuesto = cargoEnBaseDeDatos.PagoImpuesto,
                TipoCambio = cargoEnBaseDeDatos.TipoCambio,
                FechaTraslado = cargoEnBaseDeDatos.FechaTraslado,
                Facturar = cargoEnBaseDeDatos.Facturar,
                Secuencia = cargoEnBaseDeDatos.Secuencia,
                NoContable = cargoEnBaseDeDatos.NoContable,
                NumeroFolioOriginal = cargoEnBaseDeDatos.NumeroFolioOriginal
            };
        }
    }
}

