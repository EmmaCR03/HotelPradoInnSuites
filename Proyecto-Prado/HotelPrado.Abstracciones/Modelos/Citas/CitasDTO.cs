using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Citas
{
    public class CitasDTO
    {
        [Key]
        public int IdCita { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Primer apellido")]
        public string PrimerApellido { get; set; }
        [Required]
        [Display(Name = "Segundo apellido")]
        public string SegundoApellido { get; set; }
        [Required]
        [Display(Name = "Teléfono")]
        public int Telefono { get; set; }
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; }
        [Required]
        [Display(Name = "Medio de contacto")]
        public string MedioContacto { get; set; }
        [Display(Name = "ID Departamento")]
        public int IdDepartamento { get; set; }
        [Display(Name = "ID Colaborador")]
        public int? IdColaborador { get; set; }
        [Display(Name = "Fecha y hora de inicio")]
        public DateTime? FechaHoraInicio { get; set; }
        [Display(Name = "Fecha y hora de fin")]
        public DateTime? FechaHoraFin { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "Nombre del colaborador")]
        public string NombreColaborador { get; set; }
        [Display(Name = "Primer apellido del colaborador")]
        public string PrimerApellidoColaborador { get; set; }
        [Display(Name = "Número de habitación")]
        public int NumeroHabitacion { get; set; }
        [Display(Name = "Enlace WhatsApp")]
        public string EnlaceWhatsApp { get; set; }
        [Display(Name = "Enlace correo")]
        public string EnlaceCorreo { get; set; }
        public List<CitasDTO> LaListaDeCita { get; set; }
        public List<CitasDTO> CitasConEnlaces { get; set; }

    }
}
