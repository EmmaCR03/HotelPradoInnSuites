using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos
{
    [Table("Cargos")]
    public class CargosTabla
    {
        [Key]
        public int IdCargo { get; set; }

        [Column("IdCheckIn")]
        public int? IdCheckIn { get; set; }

        [Column("CodigoExtra")]
        public int? CodigoExtra { get; set; }

        [Column("DescripcionExtra")]
        public string DescripcionExtra { get; set; }

        [Column("NumeroDocumento")]
        public long? NumeroDocumento { get; set; }

        [Column("MontoCargo")]
        public decimal? MontoCargo { get; set; }

        [Column("MontoServicio")]
        public decimal? MontoServicio { get; set; }

        [Column("ImpuestoVenta")]
        public decimal? ImpuestoVenta { get; set; }

        [Column("ImpuestoHotel")]
        public decimal? ImpuestoHotel { get; set; }

        [Column("ImpuestoServicio")]
        public decimal? ImpuestoServicio { get; set; }

        [Column("MontoTotal")]
        public decimal? MontoTotal { get; set; }

        [Column("QuienPaga")]
        public long? QuienPaga { get; set; }

        [Column("FechaHora")]
        public DateTime? FechaHora { get; set; }

        [Column("NumeroEmpleado")]
        public long? NumeroEmpleado { get; set; }

        [Column("Cancelado")]
        public bool? Cancelado { get; set; }

        [Column("Notas")]
        public string Notas { get; set; }

        [Column("EnFacturaExtras")]
        public bool? EnFacturaExtras { get; set; }

        [Column("CuentaError")]
        public bool? CuentaError { get; set; }

        [Column("NumeroCierre")]
        public long? NumeroCierre { get; set; }

        [Column("PagoImpuesto")]
        public bool? PagoImpuesto { get; set; }

        [Column("TipoCambio")]
        public decimal? TipoCambio { get; set; }

        [Column("FechaTraslado")]
        public DateTime? FechaTraslado { get; set; }

        [Column("Facturar")]
        public bool? Facturar { get; set; }

        [Column("Secuencia")]
        public int? Secuencia { get; set; }

        [Column("NoContable")]
        public bool? NoContable { get; set; }

        [Column("NumeroFolioOriginal")]
        public long? NumeroFolioOriginal { get; set; }
    }
}

