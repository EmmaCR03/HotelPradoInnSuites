using HotelPrado.Abstracciones.Modelos.Reservas;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HotelPrado.UI.Models
{
    /// <summary>
    /// ViewModel para la vista de reservas del usuario
    /// Separa la lógica de presentación de la lógica de negocio
    /// </summary>
    public class ReservasUsuarioViewModel
    {
        public IEnumerable<ReservasDTO> Reservas { get; set; }
        [Display(Name = "Tiene reservas")]
        public bool TieneReservas => Reservas != null && Reservas.Any();
        [Display(Name = "Total de reservas")]
        public int TotalReservas => Reservas?.Count() ?? 0;

        // Configuración de la empresa (DRY - Don't Repeat Yourself)
        [Display(Name = "Teléfono de la empresa")]
        public string NumeroEmpresa { get; set; } = "+50685406105";
        [Display(Name = "Correo electrónico de la empresa")]
        public string CorreoEmpresa { get; set; } = "info@pradoinn.com";
    }
}

