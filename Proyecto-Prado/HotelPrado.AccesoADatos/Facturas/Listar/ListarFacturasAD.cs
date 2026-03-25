using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Listar;
using HotelPrado.Abstracciones.Modelos.Facturas;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HotelPrado.AccesoADatos.Facturas.Listar
{
    public class ListarFacturasAD : IListarFacturasAD
    {
        Contexto _contexto;

        public ListarFacturasAD()
        {
            _contexto = new Contexto();
        }

        public List<FacturasDTO> Listar()
        {
            var laListaDeFacturas = _contexto.FacturasTabla
                .Select(f => new FacturasDTO
                {
                    Id = f.Id,
                    IdFactura = f.IdFactura,
                    Clave = f.Clave,
                    NumeroConsecutivo = f.NumeroConsecutivo,
                    FechaEmision = f.FechaEmision,
                    EmisorId = f.EmisorId,
                    ReceptorId = f.ReceptorId,
                    TotalVenta = f.TotalVenta,
                    TotalDescuento = f.TotalDescuento,
                    TotalImpuesto = f.TotalImpuesto,
                    TotalComprobante = f.TotalComprobante,
                    Estado = f.Estado,
                    MedioPago = f.MedioPago,
                    FechaAprobacion = f.FechaAprobacion,
                    Activo = f.Activo,
                    NumeroFactura = f.NumeroFactura,
                    IdCheckIn = f.IdCheckIn,
                    IdEmpleado = f.IdEmpleado,
                    FechaHoraFactura = f.FechaHoraFactura,
                    TotalConsumos = f.TotalConsumos,
                    ImpuestoICT = f.ImpuestoICT,
                    ImpuestoServicio = f.ImpuestoServicio,
                    ImpuestoVentas = f.ImpuestoVentas,
                    TotalGeneral = f.TotalGeneral,
                    QuienPaga = f.QuienPaga,
                    Cerrado = f.Cerrado,
                    EnFacturaExtras = f.EnFacturaExtras,
                    FechaCreacion = f.FechaCreacion,
                    FechaModificacion = f.FechaModificacion
                })
                .OrderByDescending(f => f.FechaEmision)
                .ToList();

            return laListaDeFacturas;
        }
    }
}

