using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Configuracion
{
    [Table("ConfiguracionHeroBanner")]
    public class ConfiguracionHeroBannerTabla
    {
        [Key]
        public int IdConfiguracion { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Pagina { get; set; }
        
        [Required]
        public string UrlImagen { get; set; }
        
        public DateTime FechaActualizacion { get; set; }
        
        [StringLength(128)]
        public string ActualizadoPor { get; set; }
    }
}

