using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.TipoDeDepartamento
{
    public class TipoDepartamentoDTO
    {
        [Key]
        public int IdTipoDepartamento { get; set; }
        public int NumeroHabitaciones { get; set; }
        public bool Amueblado { get; set; }

    }
}
