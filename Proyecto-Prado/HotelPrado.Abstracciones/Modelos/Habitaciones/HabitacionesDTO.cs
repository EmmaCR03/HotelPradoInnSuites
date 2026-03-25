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
        [Required]
        [Display(Name = "Número de habitación")]
        public string NumeroHabitacion { get; set; }

        [Display(Name = "Precio por noche 1 persona")]
        public decimal PrecioPorNoche1P { get; set; }

        [Display(Name = "Precio por noche 2 personas")]
        public decimal PrecioPorNoche2P { get; set; }

        [Display(Name = "Precio por noche 3 personas")]
        public decimal PrecioPorNoche3P { get; set; }

        [Display(Name = "Precio por noche 4 personas")]
        public decimal PrecioPorNoche4P { get; set; }

        /// <summary>Tarifa general (la que ven los clientes). Si es mayor a 0 se usa en lugar de PrecioPorNoche por capacidad.</summary>
        [Display(Name = "Tarifa general")]
        public decimal PrecioGeneral { get; set; }
        [Display(Name = "Tarifa corporativa gobierno")]
        public decimal PrecioGobierno { get; set; }
        [Display(Name = "Tarifa corporativa empresas")]
        public decimal PrecioCorporativo { get; set; }

        [Display(Name = "Precio final")]
        public decimal PrecioFinal { get; set; }
        [Display(Name = "Total")]
        public decimal TotalPrecio { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Capacidad mínima")]
        public int CapacidadMin { get; set; }
        [Display(Name = "Capacidad máxima")]
        public int CapacidadMax { get; set; }

        [Display(Name = "Capacidad")]
        public int Capacidad { get; set; }
        [Display(Name = "Total de noches")]
        public int TotalNoches { get; set; }

        [Display(Name = "URL de imágenes")]
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
    }
}

