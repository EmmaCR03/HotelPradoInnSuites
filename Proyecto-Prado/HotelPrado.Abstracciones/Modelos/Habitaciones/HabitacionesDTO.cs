using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Habitaciones
{
    public class HabitacionesDTO
    {
        [Key]
        public int IdHabitacion { get; set; }
        [Required]
        public int NumeroHabitacion { get; set; }
        [Required]
        public int PrecioPorNoche { get; set; }
        [Required]
        public int IdTipoHabitacion { get; set; }
        [Required]
        public string Estado { get; set; }
    }
}
