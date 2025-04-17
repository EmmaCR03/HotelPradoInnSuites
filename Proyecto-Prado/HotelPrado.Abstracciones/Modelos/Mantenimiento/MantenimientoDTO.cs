using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Mantenimiento
{
    public class MantenimientoDTO
    {
        [Key]
        public int IdMantenimiento { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; }

        public string Estado { get; set; } = "Pendiente"; // Valor por defecto

        public int idDepartamento { get; set; } = 0; // Valor por defecto 0

        public string DepartamentoNombre { get; set; }



    }
}
