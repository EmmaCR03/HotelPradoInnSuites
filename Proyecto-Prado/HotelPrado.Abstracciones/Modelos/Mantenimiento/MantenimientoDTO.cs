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

        public string Descripcion { get; set; }

        public string Estado { get; set; }  


    }
}
