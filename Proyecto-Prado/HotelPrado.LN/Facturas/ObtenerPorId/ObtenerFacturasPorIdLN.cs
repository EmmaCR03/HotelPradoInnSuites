using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Facturas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using HotelPrado.AccesoADatos.Facturas.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Facturas.ObtenerPorId
{
    public class ObtenerFacturasPorIdLN : IObtenerFacturasPorIdLN
    {
        IObtenerFacturasPorIdAD _obtenerPorId;

        public ObtenerFacturasPorIdLN()
        {
            _obtenerPorId = new ObtenerFacturasPorIdAD();
        }

        public async Task<FacturasDTO> Obtener(Guid Id)
        {
            FacturasTabla facturaEnBaseDeDatos = await _obtenerPorId.Obtener(Id);
            FacturasDTO laFacturaAMostrar = ConvertirAFacturaAMostrar(facturaEnBaseDeDatos);
            return laFacturaAMostrar;
        }

        private FacturasDTO ConvertirAFacturaAMostrar(FacturasTabla facturaEnBaseDeDatos)
        {
            if (facturaEnBaseDeDatos == null)
                return null;

            return new FacturasDTO
            {
                Id = facturaEnBaseDeDatos.Id,
                IdFactura = facturaEnBaseDeDatos.IdFactura,
                Clave = facturaEnBaseDeDatos.Clave,
                NumeroConsecutivo = facturaEnBaseDeDatos.NumeroConsecutivo,
                FechaEmision = facturaEnBaseDeDatos.FechaEmision,
                EmisorId = facturaEnBaseDeDatos.EmisorId,
                ReceptorId = facturaEnBaseDeDatos.ReceptorId,
                TotalVenta = facturaEnBaseDeDatos.TotalVenta,
                TotalDescuento = facturaEnBaseDeDatos.TotalDescuento,
                TotalImpuesto = facturaEnBaseDeDatos.TotalImpuesto,
                TotalComprobante = facturaEnBaseDeDatos.TotalComprobante,
                Estado = facturaEnBaseDeDatos.Estado,
                MedioPago = facturaEnBaseDeDatos.MedioPago,
                FechaAprobacion = facturaEnBaseDeDatos.FechaAprobacion,
                Activo = facturaEnBaseDeDatos.Activo,
                NumeroFactura = facturaEnBaseDeDatos.NumeroFactura,
                IdCheckIn = facturaEnBaseDeDatos.IdCheckIn,
                IdEmpleado = facturaEnBaseDeDatos.IdEmpleado,
                FechaHoraFactura = facturaEnBaseDeDatos.FechaHoraFactura,
                TotalConsumos = facturaEnBaseDeDatos.TotalConsumos,
                ImpuestoICT = facturaEnBaseDeDatos.ImpuestoICT,
                ImpuestoServicio = facturaEnBaseDeDatos.ImpuestoServicio,
                ImpuestoVentas = facturaEnBaseDeDatos.ImpuestoVentas,
                TotalGeneral = facturaEnBaseDeDatos.TotalGeneral,
                QuienPaga = facturaEnBaseDeDatos.QuienPaga,
                Cerrado = facturaEnBaseDeDatos.Cerrado,
                EnFacturaExtras = facturaEnBaseDeDatos.EnFacturaExtras,
                FechaCreacion = facturaEnBaseDeDatos.FechaCreacion,
                FechaModificacion = facturaEnBaseDeDatos.FechaModificacion
            };
        }
    }
}

