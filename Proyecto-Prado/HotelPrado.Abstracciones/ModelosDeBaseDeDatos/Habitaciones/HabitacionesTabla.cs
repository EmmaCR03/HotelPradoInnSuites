using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoHabitacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones
{
    [Table("Habitaciones")]
    public class HabitacionesTabla
    {
        [Key]
        public int IdHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public int PrecioPorNoche { get; set; }
        public int IdTipoHabitacion { get; set; }
        public string Estado { get; set; }

        [ForeignKey("IdTipoHabitacion")]
        public virtual TipoHabitacionTabla TipoHabitacion { get; set; }
    }
}


