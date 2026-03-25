
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
        public string NumeroHabitacion { get; set; }
        public decimal PrecioPorNoche1P { get; set; }
        public decimal PrecioPorNoche2P { get; set; }
        public decimal PrecioPorNoche3P { get; set; }
        public decimal PrecioPorNoche4P { get; set; }
        /// <summary>Tarifa general (clientes). Si > 0 se usa en vistas públicas.</summary>
        public decimal PrecioGeneral { get; set; }
        public decimal PrecioGobierno { get; set; }
        public decimal PrecioCorporativo { get; set; }

        public string UrlImagenes { get; set; }
        public string Estado { get; set; }



        public int CapacidadMax { get; set; }
        public int CapacidadMin { get; set; }

    }
}


