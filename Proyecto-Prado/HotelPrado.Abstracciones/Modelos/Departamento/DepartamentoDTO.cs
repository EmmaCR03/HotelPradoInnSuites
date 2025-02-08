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
        public int? IdCliente { get; set; }
        public string UrlImagenes { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Range(11, 21, ErrorMessage = "El número de departamento debe estar entre 11 y 21.")]
        public int NumeroDepartamento { get; set; }

        public string NombreCliente { get; set; }
        [Required]
        public int IdTipoDepartamento { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        public int NumeroHabitaciones { get; set; }
        public bool Amueblado { get; set; }
        public string NumeroEmpresa { get; set; }  // Antes era int, ahora es string
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