using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.AccesoADatos.Colaborador.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Colaborador.Registrar
{
    public class RegistrarColaboradorLN : IRegistrarColaboradorLN
    {
        private readonly IRegistrarColaboradorAD _registrarColaboradorAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarColaboradorLN()
        {
            _registrarColaboradorAD = new RegistrarColaboradorAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(ColaboradorDTO modelo)
        {
            try
            {
                // Guardar el departamento en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarColaboradorAD.Guardar(ConvertirObjetoColaboradorTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdColaborador"": {modelo.IdColaborador},
                    ""NombreColaborador"": {modelo.NombreColaborador},
                    ""PrimerApellidoColaborador"": ""{modelo.PrimerApellidoColaborador}"",
                    ""SegundoApellidoColaborador"": ""{modelo.SegundoApellidoColaborador}"",
                    ""CedulaColaborador"": {modelo.CedulaColaborador},
                    ""IngresoColaborador"": {modelo.IngresoColaborador},
                    ""PuestoColaborador"": ""{modelo.PuestoColaborador}"",
                    ""Estado"": {modelo.EstadoLaboral},
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloColaborador",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró un nuevo Colaborador en la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = "NA",
                    DatosPosteriores = datos
                };

                // Registrar el evento en la bitácora
                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloColaborador",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar el Colaborador.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private ColaboradorTabla ConvertirObjetoColaboradorTabla(ColaboradorDTO elColaborador)
        {
            return new ColaboradorTabla
            {
                IdColaborador = elColaborador.IdColaborador,
                NombreColaborador = elColaborador.NombreColaborador,
                PrimerApellidoColaborador = elColaborador.PrimerApellidoColaborador,
                SegundoApellidoColaborador = elColaborador.SegundoApellidoColaborador,
                CedulaColaborador = elColaborador.CedulaColaborador,
                PuestoColaborador = elColaborador.PuestoColaborador,
                EstadoLaboral = elColaborador.EstadoLaboral,
                IngresoColaborador = elColaborador.IngresoColaborador
            };
        }
    }
}
