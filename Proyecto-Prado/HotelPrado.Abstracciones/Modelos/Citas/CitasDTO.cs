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
        public string Nombre { get; set; }
        [Required]
        public string PrimerApellido { get; set; }
        [Required]
        public string SegundoApellido { get; set; }
        [Required]
        public int Telefono { get; set; }
        [Required]
        public string Correo { get; set; }
        [Required]
        public string MedioContacto { get; set; }
        public int IdDepartamento { get; set; }
        public int? IdColaborador { get; set; }
        public DateTime? FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreColaborador { get; set; }
        public string PrimerApellidoColaborador { get; set; }
        public int NumeroHabitacion { get; set; }
        public string EnlaceWhatsApp { get; set; }
        public string EnlaceCorreo { get; set; }
        public List<CitasDTO> LaListaDeCita { get; set; }
        public List<CitasDTO> CitasConEnlaces { get; set; }

    }
}
