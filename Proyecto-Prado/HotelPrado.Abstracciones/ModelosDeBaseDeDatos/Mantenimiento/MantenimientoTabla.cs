using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento
{
    [Table("Mantenimiento")]
    public class MantenimientoTabla
    {
        [Key]
        public int IdMantenimiento { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }
        public int idDepartamento { get; set; }
        public int idHabitacion { get; set; }

    }
}
