using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas
{
    [Table("Facturas")]
    public class FacturasTabla
    {
        // Clave primaria de facturación electrónica
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        // Columna adicional para migración DBF (IDENTITY, índice único)
        [Column("IdFactura")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFactura { get; set; }

        // ============================================
        // COLUMNAS DE FACTURACIÓN ELECTRÓNICA (NOT NULL)
        // ============================================
        
        [Column("Clave")]
        [Required]
        [StringLength(100)]
        public string Clave { get; set; }

        [Column("NumeroConsecutivo")]
        [Required]
        [StringLength(40)]
        public string NumeroConsecutivo { get; set; }

        [Column("FechaEmision")]
        [Required]
        public DateTime FechaEmision { get; set; }

        [Column("EmisorId")]
        [Required]
        public Guid EmisorId { get; set; }

        [Column("ReceptorId")]
        [Required]
        public Guid ReceptorId { get; set; }

        [Column("TotalVenta")]
        [Required]
        public decimal TotalVenta { get; set; }

        [Column("TotalDescuento")]
        [Required]
        public decimal TotalDescuento { get; set; }

        [Column("TotalImpuesto")]
        [Required]
        public decimal TotalImpuesto { get; set; }

        [Column("TotalComprobante")]
        [Required]
        public decimal TotalComprobante { get; set; }

        [Column("Estado")]
        [Required]
        [StringLength(40)]
        public string Estado { get; set; }

        // ============================================
        // COLUMNAS OPCIONALES DE FACTURACIÓN ELECTRÓNICA
        // ============================================

        [Column("MedioPago")]
        [StringLength(100)]
        public string MedioPago { get; set; }

        [Column("XmlFirmado")]
        public string XmlFirmado { get; set; }

        [Column("RespuestaHacienda")]
        public string RespuestaHacienda { get; set; }

        [Column("FechaAprobacion")]
        public DateTime? FechaAprobacion { get; set; }

        [Column("Activo")]
        public bool? Activo { get; set; }

        // ============================================
        // COLUMNAS DE MIGRACIÓN DBF (OPCIONALES)
        // ============================================

        [Column("NumeroFactura")]
        public int? NumeroFactura { get; set; }

        [Column("IdCheckIn")]
        public int? IdCheckIn { get; set; }

        [Column("IdEmpleado")]
        public int? IdEmpleado { get; set; }

        [Column("FechaHoraFactura")]
        public DateTime? FechaHoraFactura { get; set; }

        [Column("TotalConsumos")]
        public decimal? TotalConsumos { get; set; }

        [Column("ImpuestoICT")]
        public decimal? ImpuestoICT { get; set; }

        [Column("ImpuestoServicio")]
        public decimal? ImpuestoServicio { get; set; }

        [Column("ImpuestoVentas")]
        public decimal? ImpuestoVentas { get; set; }

        [Column("TotalGeneral")]
        public decimal? TotalGeneral { get; set; }

        [Column("QuienPaga")]
        public long? QuienPaga { get; set; }

        [Column("Cerrado")]
        public bool? Cerrado { get; set; }

        [Column("EnFacturaExtras")]
        public bool? EnFacturaExtras { get; set; }

        [Column("FechaCreacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("FechaModificacion")]
        public DateTime? FechaModificacion { get; set; }
    }
}

