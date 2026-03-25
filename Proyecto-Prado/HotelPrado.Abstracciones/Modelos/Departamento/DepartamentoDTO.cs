using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Departamento
{
    public class DepartamentoDTO
    {
        [Key]
        public int IdDepartamento { get; set; }
        [Display(Name = "ID Cliente")]
        public int? IdCliente { get; set; }
        [Display(Name = "URL de imágenes")]
        public string UrlImagenes { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Número de departamento")]
        /// <summary>Opcional. Use 0 o vacío para "Sin número".</summary>
        public int? NumeroDepartamento { get; set; }

        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; }
        [Required]
        [Display(Name = "Tipo de departamento")]
        public int IdTipoDepartamento { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Required]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [Required]
        [Display(Name = "Número de habitaciones")]
        public int NumeroHabitaciones { get; set; }
        [Display(Name = "Amueblado")]
        public bool Amueblado { get; set; }
        [Display(Name = "Teléfono de la empresa")]
        public string NumeroEmpresa { get; set; }  // Antes era int, ahora es string
        [Display(Name = "Correo electrónico de la empresa")]
        public string CorreoEmpresa { get; set; }
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