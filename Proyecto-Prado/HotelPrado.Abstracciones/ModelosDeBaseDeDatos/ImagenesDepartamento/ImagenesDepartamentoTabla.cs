using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesDepartamento
{
    [Table("ImagenesDepartamento")]
    public class ImagenesDepartamentoTabla
    {
        [Key]
        public int IdImagen { get; set; }
        public int IdDepartamento { get; set; }
        public string UrlImagen { get; set; }

    }
}
