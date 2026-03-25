using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Colaborador
{
    public class ColaboradorDTO
    {
        [Key]
        public int IdColaborador { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ]+$", ErrorMessage = "El nombre solo puede contener letras y no admite espacios.")]
        [Display(Name = "Nombre")]
        public string NombreColaborador { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ]+$", ErrorMessage = "El primer apellido solo puede contener letras y no admite espacios.")]
        [Display(Name = "Primer apellido")]
        public string PrimerApellidoColaborador { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ]+$", ErrorMessage = "El segundo apellido solo puede contener letras y no admite espacios.")]
        [Display(Name = "Segundo apellido")]
        public string SegundoApellidoColaborador { get; set; }
        [Required]
        [Range(100000000, 999999999, ErrorMessage = "La cédula debe tener 9 dígitos.")]
        [Display(Name = "Cédula")]
        public int CedulaColaborador { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El salario debe ser un número mayor que 0.")]
        [Display(Name = "Ingreso")]
        public int IngresoColaborador { get; set; }
        [Required]
        [Display(Name = "Puesto")]
        public string PuestoColaborador { get; set; }
        [Required]
        [Display(Name = "Estado laboral")]
        public string EstadoLaboral { get; set; }
    }
}
