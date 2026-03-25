using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Configuracion
{
    [Table("ConfiguracionPreciosDepartamentos")]
    public class ConfiguracionPreciosDepartamentosTabla
    {
        [Key]
        public int IdConfiguracion { get; set; }
        
        [Required]
        public decimal PrecioBase { get; set; }
        
        [StringLength(100)]
        public string TextoPrecio { get; set; }
        
        public bool MostrarPrecio { get; set; }
        
        public DateTime FechaActualizacion { get; set; }
        
        [StringLength(128)]
        public string ActualizadoPor { get; set; }
    }
}

