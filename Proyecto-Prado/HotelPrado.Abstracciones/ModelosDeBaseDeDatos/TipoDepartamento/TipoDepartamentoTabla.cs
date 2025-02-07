using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.TipoDepartamento
{
    [Table("TipoDepartamento")]

    public class TipoDepartamentoTabla
    {
        [Key]
        public int IdTipoDepartamento { get; set; }
        public int NumeroHabitaciones { get; set; }
        public bool Amueblado { get; set; }
    }
}
