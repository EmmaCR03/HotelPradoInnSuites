using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.ImagenesHabitacion
{
    public class ImagenesHabitacionDTO
    {
        [Key]
        public int IdImagen { get; set; }
        public int IdHabitacion { get; set; }
        public string UrlImagen { get; set; }
    }
}
