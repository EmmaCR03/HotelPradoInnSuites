using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesHabitacion
{
    [Table("ImagenesHabitacion")]
    public  class ImagenesHabitacionTabla
    {
        [Key]
        public int IdImagen { get; set; }
        public int IdHabitacion { get; set; }
        public string UrlImagen { get; set; }

    }
}
