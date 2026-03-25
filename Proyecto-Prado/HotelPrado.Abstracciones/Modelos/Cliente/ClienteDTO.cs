using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Modelos.Cliente
{
    public class ClienteDTO
    {
        [Key]
        public int IdCliente { get; set; }
        [Display(Name = "Nombre")]
        public string NombreCliente { get; set; }
        [Display(Name = "Primer apellido")]
        public string PrimerApellidoCliente { get; set; }
        [Display(Name = "Segundo apellido")]
        public string SegundoApellidoCLiente { get; set; }
        [Display(Name = "Correo electrónico")]
        public string EmailCliente { get; set; }
        [Display(Name = "Teléfono")]
        public string TelefonoCliente { get; set; }
        [Display(Name = "Dirección")]
        public string DireccionCliente { get; set; }
        [Display(Name = "Cédula")]
        public string CedulaCliente { get; set; }
        [Display(Name = "ID Empresa")]
        public int? IdEmpresa { get; set; }
        [Display(Name = "ID Usuario")]
        public string IdUsuario { get; set; }
        
        // Propiedades calculadas para mostrar
        public string NombreCompleto 
        { 
            get 
            { 
                var nombre = $"{NombreCliente ?? ""} {PrimerApellidoCliente ?? ""} {SegundoApellidoCLiente ?? ""}".Trim();
                return string.IsNullOrEmpty(nombre) ? "Sin nombre" : nombre;
            } 
        }
    }
}
