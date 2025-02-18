using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas
{
    [Table("Citas")]
    public class CitasTabla
    {
        [Key]
        public int IdCita { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public string MedioContacto { get; set; }
        public int IdDepartamento { get; set; }
        public int? IdColaborador { get; set; }
        public DateTime? FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string EnlaceWhatsApp { get; set; }
        public string EnlaceCorreo { get; set; }

        [ForeignKey("IdColaborador")]

        public virtual ColaboradorTabla Colaborador { get; set; }

    }
}
