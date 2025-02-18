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
        public decimal TotalPrecio { get; set; }
        public int IdTipoHabitacion { get; set; }
        public string Estado { get; set; }

        public int CapacidadMin { get; set; }
        public int CapacidadMax { get; set; }

        
        public int Capacidad { get; set; }
        public string UrlImagenes { get; set; }
        public List<string> ListaImagenes
        {
            get
            {
                return string.IsNullOrEmpty(UrlImagenes) ? new List<string>() : UrlImagenes.Split(',').ToList();
            }
            set
            {
                UrlImagenes = string.Join(",", value);
            }
        }
        public int TotalNoches { get; set; }

    }
}
