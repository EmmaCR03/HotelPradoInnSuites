using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas
{
    [Table("Temporadas")]
    public class TemporadasTabla
    {
        [Key]
        public int IdTemporada { get; set; }

        [Column("NumeroTemporada")]
        public int? NumeroTemporada { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("CodigoCuenta")]
        public long? CodigoCuenta { get; set; }

        [Column("AumentaAl")]
        public int? AumentaAl { get; set; }
    }
}

