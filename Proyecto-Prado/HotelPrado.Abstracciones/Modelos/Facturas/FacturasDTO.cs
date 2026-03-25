using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Facturas
{
    public class FacturasDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "ID Factura")]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        [Display(Name = "Clave")]
        [StringLength(100, ErrorMessage = "La clave no puede tener más de 100 caracteres")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El número consecutivo es requerido")]
        [Display(Name = "Número Consecutivo")]
        [StringLength(40, ErrorMessage = "El número consecutivo no puede tener más de 40 caracteres")]
        public string NumeroConsecutivo { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es requerida")]
        [Display(Name = "Fecha de Emisión")]
        [DataType(DataType.DateTime)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El emisor es requerido")]
        [Display(Name = "Emisor")]
        public Guid EmisorId { get; set; }

        [Required(ErrorMessage = "El receptor es requerido")]
        [Display(Name = "Receptor")]
        public Guid ReceptorId { get; set; }

        [Required(ErrorMessage = "El total de venta es requerido")]
        [Display(Name = "Total Venta")]
        [Range(0, double.MaxValue, ErrorMessage = "El total de venta debe ser mayor o igual a 0")]
        public decimal TotalVenta { get; set; }

        [Required(ErrorMessage = "El total de descuento es requerido")]
        [Display(Name = "Total Descuento")]
        [Range(0, double.MaxValue, ErrorMessage = "El total de descuento debe ser mayor o igual a 0")]
        public decimal TotalDescuento { get; set; }

        [Required(ErrorMessage = "El total de impuesto es requerido")]
        [Display(Name = "Total Impuesto")]
        [Range(0, double.MaxValue, ErrorMessage = "El total de impuesto debe ser mayor o igual a 0")]
        public decimal TotalImpuesto { get; set; }

        [Required(ErrorMessage = "El total del comprobante es requerido")]
        [Display(Name = "Total Comprobante")]
        [Range(0, double.MaxValue, ErrorMessage = "El total del comprobante debe ser mayor o igual a 0")]
        public decimal TotalComprobante { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        [StringLength(40, ErrorMessage = "El estado no puede tener más de 40 caracteres")]
        public string Estado { get; set; }

        [Display(Name = "Medio de Pago")]
        [StringLength(100, ErrorMessage = "El medio de pago no puede tener más de 100 caracteres")]
        public string MedioPago { get; set; }

        [Display(Name = "Fecha de Aprobación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaAprobacion { get; set; }

        [Display(Name = "Activo")]
        public bool? Activo { get; set; }

        // Columnas de migración DBF (opcionales)
        [Display(Name = "Número de Factura")]
        public int? NumeroFactura { get; set; }

        [Display(Name = "Check-In")]
        public int? IdCheckIn { get; set; }

        [Display(Name = "Empleado")]
        public int? IdEmpleado { get; set; }

        [Display(Name = "Fecha/Hora Factura")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaHoraFactura { get; set; }

        [Display(Name = "Total Consumos")]
        [Range(0, double.MaxValue, ErrorMessage = "El total de consumos debe ser mayor o igual a 0")]
        public decimal? TotalConsumos { get; set; }

        [Display(Name = "Impuesto ICT")]
        [Range(0, double.MaxValue, ErrorMessage = "El impuesto ICT debe ser mayor o igual a 0")]
        public decimal? ImpuestoICT { get; set; }

        [Display(Name = "Impuesto Servicio")]
        [Range(0, double.MaxValue, ErrorMessage = "El impuesto de servicio debe ser mayor o igual a 0")]
        public decimal? ImpuestoServicio { get; set; }

        [Display(Name = "Impuesto Ventas")]
        [Range(0, double.MaxValue, ErrorMessage = "El impuesto de ventas debe ser mayor o igual a 0")]
        public decimal? ImpuestoVentas { get; set; }

        [Display(Name = "Total General")]
        [Range(0, double.MaxValue, ErrorMessage = "El total general debe ser mayor o igual a 0")]
        public decimal? TotalGeneral { get; set; }

        [Display(Name = "Quien Paga")]
        public long? QuienPaga { get; set; }

        [Display(Name = "Cerrado")]
        public bool? Cerrado { get; set; }

        [Display(Name = "En Factura Extras")]
        public bool? EnFacturaExtras { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaCreacion { get; set; }

        [Display(Name = "Fecha de Modificación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaModificacion { get; set; }
    }
}

