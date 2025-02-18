using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Habitaciones
{
    public class HabitacionesDTO
    {
        [Key]
        public int IdHabitacion { get; set; }
        public string NumeroHabitacion { get; set; }
        public decimal PrecioPorNoche1P { get; set; }
        public decimal PrecioPorNoche2P { get; set; }
        public decimal PrecioPorNoche3P { get; set; }
        public decimal PrecioPorNoche4P { get; set; }

        public decimal PrecioFinal { get; set; }
        public int IdTipoHabitacion { get; set; }
        public string Estado { get; set; }

        public int Capacidad { get; set; }

    }
}
