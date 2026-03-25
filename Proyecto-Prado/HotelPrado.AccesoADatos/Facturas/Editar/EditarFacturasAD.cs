using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Facturas.Editar
{
    public class EditarFacturasAD : IEditarFacturasAD
    {
        private readonly Contexto _contexto;

        public EditarFacturasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(FacturasTabla laFacturaActualizar)
        {
            FacturasTabla laFacturaEnBaseDeDatos = _contexto.FacturasTabla
                .Where(f => f.Id == laFacturaActualizar.Id)
                .FirstOrDefault();

            if (laFacturaEnBaseDeDatos != null)
            {
                // Actualizar campos de facturación electrónica
                laFacturaEnBaseDeDatos.Clave = laFacturaActualizar.Clave;
                laFacturaEnBaseDeDatos.NumeroConsecutivo = laFacturaActualizar.NumeroConsecutivo;
                laFacturaEnBaseDeDatos.FechaEmision = laFacturaActualizar.FechaEmision;
                laFacturaEnBaseDeDatos.EmisorId = laFacturaActualizar.EmisorId;
                laFacturaEnBaseDeDatos.ReceptorId = laFacturaActualizar.ReceptorId;
                laFacturaEnBaseDeDatos.TotalVenta = laFacturaActualizar.TotalVenta;
                laFacturaEnBaseDeDatos.TotalDescuento = laFacturaActualizar.TotalDescuento;
                laFacturaEnBaseDeDatos.TotalImpuesto = laFacturaActualizar.TotalImpuesto;
                laFacturaEnBaseDeDatos.TotalComprobante = laFacturaActualizar.TotalComprobante;
                laFacturaEnBaseDeDatos.Estado = laFacturaActualizar.Estado;
                laFacturaEnBaseDeDatos.MedioPago = laFacturaActualizar.MedioPago;
                laFacturaEnBaseDeDatos.FechaAprobacion = laFacturaActualizar.FechaAprobacion;
                laFacturaEnBaseDeDatos.Activo = laFacturaActualizar.Activo;

                // Actualizar campos de migración DBF
                laFacturaEnBaseDeDatos.NumeroFactura = laFacturaActualizar.NumeroFactura;
                laFacturaEnBaseDeDatos.IdCheckIn = laFacturaActualizar.IdCheckIn;
                laFacturaEnBaseDeDatos.IdEmpleado = laFacturaActualizar.IdEmpleado;
                laFacturaEnBaseDeDatos.FechaHoraFactura = laFacturaActualizar.FechaHoraFactura;
                laFacturaEnBaseDeDatos.TotalConsumos = laFacturaActualizar.TotalConsumos;
                laFacturaEnBaseDeDatos.ImpuestoICT = laFacturaActualizar.ImpuestoICT;
                laFacturaEnBaseDeDatos.ImpuestoServicio = laFacturaActualizar.ImpuestoServicio;
                laFacturaEnBaseDeDatos.ImpuestoVentas = laFacturaActualizar.ImpuestoVentas;
                laFacturaEnBaseDeDatos.TotalGeneral = laFacturaActualizar.TotalGeneral;
                laFacturaEnBaseDeDatos.QuienPaga = laFacturaActualizar.QuienPaga;
                laFacturaEnBaseDeDatos.Cerrado = laFacturaActualizar.Cerrado;
                laFacturaEnBaseDeDatos.EnFacturaExtras = laFacturaActualizar.EnFacturaExtras;
                laFacturaEnBaseDeDatos.FechaModificacion = DateTime.Now;

                EntityState estado = _contexto.Entry(laFacturaEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            return 0;
        }
    }
}

