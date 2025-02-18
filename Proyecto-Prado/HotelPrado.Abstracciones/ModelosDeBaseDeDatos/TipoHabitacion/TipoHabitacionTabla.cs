using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoHabitacion
{
    [Table("TipoHabitacion")]
    public class TipoHabitacionTabla
    {
        [Key]
        public int IdTipoHabitacion { get; set; }
        public string Nombre { get; set; }
        public string Equipamiento { get; set; }
    }
}
