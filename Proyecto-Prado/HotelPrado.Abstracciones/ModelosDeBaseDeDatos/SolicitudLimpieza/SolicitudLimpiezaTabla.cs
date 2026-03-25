using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza
{
    [Table("SolicitudesLimpieza")]
    public class SolicitudLimpiezaTabla
    {
        [Key]
        public int IdSolicitudLimpieza { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }

        [Column("idDepartamento")]
        public int? idDepartamento { get; set; }

        [Column("DepartamentoNombre")]
        public string DepartamentoNombre { get; set; }

        [Column("idHabitacion")]
        public int? idHabitacion { get; set; }

        [Column("FechaSolicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [NotMapped]
        public string HabitacionNombre { get; set; }
    }
}

