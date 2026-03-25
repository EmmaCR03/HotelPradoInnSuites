using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Bitacora
{
    public class BitacoraEventosDTO
    {
        [Key]
        public int IdEvento { get; set; }
        [Display(Name = "Tabla")]
        public string TablaDeEvento { get; set; }
        [Display(Name = "Tipo de evento")]
        public string TipoDeEvento { get; set; }
        [Display(Name = "Fecha")]
        public string FechaDeEvento { get; set; }
        [Display(Name = "Descripción")]
        public string DescripcionDeEvento { get; set; }
        [Display(Name = "Stack trace")]
        public string StackTrace { get; set; }
        [Display(Name = "Datos anteriores")]
        public string DatosAnteriores { get; set; }
        [Display(Name = "Datos posteriores")]
        public string DatosPosteriores { get; set; }
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }
    }
}
