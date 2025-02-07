using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador
{

    [Table("Colaborador")]
    public class ColaboradorTabla
    {
        [Key]
        public int IdColaborador { get; set; }
        public string NombreColaborador { get; set; }
        public string PrimerApellidoColaborador { get; set; }
        public string SegundoApellidoColaborador { get; set; }
        public int CedulaColaborador { get; set; }
        public int IngresoColaborador { get; set; }
        public string PuestoColaborador { get; set; }
        public string EstadoLaboral { get; set; }
    }
}


