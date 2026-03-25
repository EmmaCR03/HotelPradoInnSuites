using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Configuracion
{
    public class ConfiguracionPreciosDepartamentosDTO
    {
        public int IdConfiguracion { get; set; }

        [Required(ErrorMessage = "El precio base es requerido")]
        [Range(0, 999999999.99, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        [Display(Name = "Precio base")]
        public decimal PrecioBase { get; set; }

        [StringLength(100)]
        [Display(Name = "Texto del precio")]
        public string TextoPrecio { get; set; }

        [Display(Name = "Mostrar precio")]
        public bool MostrarPrecio { get; set; }

        [Display(Name = "Fecha de actualización")]
        public DateTime FechaActualizacion { get; set; }

        [Display(Name = "Actualizado por")]
        public string ActualizadoPor { get; set; }
    }
}

