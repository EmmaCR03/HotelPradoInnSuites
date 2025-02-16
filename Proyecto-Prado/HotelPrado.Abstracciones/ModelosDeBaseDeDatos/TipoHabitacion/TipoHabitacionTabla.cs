using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoHabitacion
{
    [Table("TipoHabitaciones")]
    public class TipoHabitacionTabla
    {
        [Key]
        public int IdTipoHabitacion { get; set; }
        public int Descripcion { get; set; }
        public bool CapacidadDePersonas { get; set; }
        public int NumeroHabitaciones { get; set; }
    }
}
