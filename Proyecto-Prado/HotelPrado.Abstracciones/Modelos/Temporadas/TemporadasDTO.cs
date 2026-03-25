using System.ComponentModel.DataAnnotations;

namespace HotelPrado.Abstracciones.Modelos.Temporadas
{
    public class TemporadasDTO
    {
        [Key]
        public int IdTemporada { get; set; }

        [Required(ErrorMessage = "El número de temporada es requerido")]
        [Display(Name = "Número de Temporada")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de temporada debe ser mayor a 0")]
        public int? NumeroTemporada { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [Display(Name = "Descripción")]
        [StringLength(60, ErrorMessage = "La descripción no puede tener más de 60 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Código de Cuenta")]
        public long? CodigoCuenta { get; set; }

        [Display(Name = "Aumenta Al")]
        public int? AumentaAl { get; set; }
    }
}

