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

        [Column("idDepartamento")]
        public int idDepartamento { get; set; } 

        [Column("DepartamentoNombre")]
        public string DepartamentoNombre { get; set; }

        [Column("IdHabitacion")]
        public int idHabitacion { get; set; }

        [NotMapped]
        public string HabitacionNombre { get; set; }



    }
}
