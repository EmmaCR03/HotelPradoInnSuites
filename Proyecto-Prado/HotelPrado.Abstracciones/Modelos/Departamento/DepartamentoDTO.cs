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
        public  string Nombre { get; set; }
        public int NumeroDepartamento { get; set; }
        public string NombreCliente { get; set; }
        public int IdTipoDepartamento { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public int NumeroHabitaciones { get; set; }
        public bool Amueblado { get; set; }
        public string NumeroEmpresa { get; set; }  // Antes era int, ahora es string
        public string CorreoEmpresa { get; set; }



    }
}