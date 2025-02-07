using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento
{
    [Table("Departamento")]

    public class DepartamentoTabla
    {
        [Key]
        public int IdDepartamento { get; set; }
        public int? IdCliente { get; set; }
        public string Nombre { get; set; }
        public int NumeroDepartamento { get; set; }
        public string UrlImagenes { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoDepartamento { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public string NumeroEmpresa { get; set; }  // Antes era int, ahora es string
        public string CorreoEmpresa { get; set; }

    }
}
