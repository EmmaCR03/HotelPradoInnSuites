using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Conversion;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;

namespace HotelPrado.LN.Colaborador.Conversion
{
    public class ConvertirColaboradorDTOAColaboradorTabla : IConvertirColaboradorDTOAColaboradorTabla
    {
        public ColaboradorTabla Convertir(ColaboradorDTO elColaborador)
        {
            return new ColaboradorTabla
            {
                IdColaborador = elColaborador.IdColaborador,
                NombreColaborador = elColaborador.NombreColaborador,
                PrimerApellidoColaborador = elColaborador.PrimerApellidoColaborador,
                SegundoApellidoColaborador = elColaborador.SegundoApellidoColaborador,
                CedulaColaborador = (int)elColaborador.CedulaColaborador,
                PuestoColaborador = elColaborador.PuestoColaborador,
                EstadoLaboral = elColaborador.EstadoLaboral,
                IngresoColaborador = elColaborador.IngresoColaborador,
            };
        }
    }
}
