using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.SolicitudLimpieza
{
    public class SolicitudLimpiezaDTO
    {
        [Key]
        public int IdSolicitudLimpieza { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Valor por defecto

        [Display(Name = "ID Departamento")]
        public int idDepartamento { get; set; } = 0; // Valor por defecto 0

        [Display(Name = "Nombre del departamento")]
        public string DepartamentoNombre { get; set; }

        [Display(Name = "ID Habitación")]
        public int idHabitacion { get; set; } = 0; // Valor por defecto 0

        [Display(Name = "Nombre de la habitación")]
        public string HabitacionNombre { get; set; }

        [Display(Name = "Fecha de solicitud")]
        public DateTime? FechaSolicitud { get; set; }
    }
}

