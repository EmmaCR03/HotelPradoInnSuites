using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.TipoDeHabitacion
{
    public class TipoHabitacionDTO
    {
        [Key]
        public int IdTipoHabitacion { get; set; }
        public string Nombre { get; set; }
        public string Equipamiento { get; set; }
    }
}
