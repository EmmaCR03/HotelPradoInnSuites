using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.Modelos.Cargos
{
    public class CargosDTO
    {
        [Key]
        public int IdCargo { get; set; }

        [Display(Name = "Check-In")]
        public int? IdCheckIn { get; set; }

        [Display(Name = "Código Extra")]
        public int? CodigoExtra { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [Display(Name = "Descripción")]
        [StringLength(60, ErrorMessage = "La descripción no puede tener más de 60 caracteres")]
        public string DescripcionExtra { get; set; }

        [Display(Name = "Número de Documento")]
        public long? NumeroDocumento { get; set; }

        [Display(Name = "Monto del Cargo")]
        [DataType(DataType.Currency)]
        public decimal? MontoCargo { get; set; }

        [Display(Name = "Monto de Servicio")]
        [DataType(DataType.Currency)]
        public decimal? MontoServicio { get; set; }

        [Display(Name = "Impuesto de Venta")]
        [DataType(DataType.Currency)]
        public decimal? ImpuestoVenta { get; set; }

        [Display(Name = "Impuesto de Hotel")]
        [DataType(DataType.Currency)]
        public decimal? ImpuestoHotel { get; set; }

        [Display(Name = "Impuesto de Servicio")]
        [DataType(DataType.Currency)]
        public decimal? ImpuestoServicio { get; set; }

        [Required(ErrorMessage = "El monto total es requerido")]
        [Display(Name = "Monto Total")]
        [DataType(DataType.Currency)]
        public decimal? MontoTotal { get; set; }

        [Display(Name = "Quien Paga")]
        public long? QuienPaga { get; set; }

        [Display(Name = "Fecha y Hora")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaHora { get; set; }

        [Display(Name = "Número de Empleado")]
        public long? NumeroEmpleado { get; set; }

        [Display(Name = "Cancelado")]
        public bool? Cancelado { get; set; }

        [Display(Name = "Notas")]
        [StringLength(200, ErrorMessage = "Las notas no pueden tener más de 200 caracteres")]
        public string Notas { get; set; }

        [Display(Name = "En Factura Extras")]
        public bool? EnFacturaExtras { get; set; }

        [Display(Name = "Cuenta Error")]
        public bool? CuentaError { get; set; }

        [Display(Name = "Número de Cierre")]
        public long? NumeroCierre { get; set; }

        [Display(Name = "Pago Impuesto")]
        public bool? PagoImpuesto { get; set; }

        [Display(Name = "Tipo de Cambio")]
        [DataType(DataType.Currency)]
        public decimal? TipoCambio { get; set; }

        [Display(Name = "Fecha de Traslado")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaTraslado { get; set; }

        [Display(Name = "Facturar")]
        public bool? Facturar { get; set; }

        [Display(Name = "Secuencia")]
        public int? Secuencia { get; set; }

        [Display(Name = "No Contable")]
        public bool? NoContable { get; set; }

        [Display(Name = "Número de Folio Original")]
        public long? NumeroFolioOriginal { get; set; }

        // Campos adicionales para mostrar información relacionada
        [NotMapped]
        public string CheckInNombre { get; set; }

        [NotMapped]
        public string ClienteNombre { get; set; }
    }
}

