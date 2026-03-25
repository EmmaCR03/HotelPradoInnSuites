using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Configuracion
{
    public class ConfiguracionHeroBannerDTO
    {
        public int IdConfiguracion { get; set; }

        [Required(ErrorMessage = "La página es requerida")]
        [StringLength(50)]
        [Display(Name = "Página")]
        public string Pagina { get; set; }

        [Required(ErrorMessage = "La URL de la imagen es requerida")]
        [Display(Name = "URL de la imagen")]
        public string UrlImagen { get; set; }

        [Display(Name = "Fecha de actualización")]
        public DateTime FechaActualizacion { get; set; }

        [Display(Name = "Actualizado por")]
        public string ActualizadoPor { get; set; }
    }
}

