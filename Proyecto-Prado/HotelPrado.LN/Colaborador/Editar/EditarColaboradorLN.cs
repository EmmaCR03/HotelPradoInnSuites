using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Conversion;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using HotelPrado.AccesoADatos.Colaborador.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Colaborador.Conversion;
using HotelPrado.LN.Colaborador.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Colaborador.Editar
{
    public class EditarColaboradorLN : IEditarColaboradorLN
    {
        private readonly IConvertirColaboradorDTOAColaboradorTabla _convertir;
        private readonly IEditarColaboradorAD _editarColaborador;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerColaboradorPorIdLN _obtenerColaboradorPorIdLN;

        public EditarColaboradorLN()
        {
            _convertir = new ConvertirColaboradorDTOAColaboradorTabla();
            _editarColaborador = new EditarColaboradorAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerColaboradorPorIdLN = new ObtenerColaboradorPorIdLN();
        }

        public async Task<int> Actualizar(ColaboradorDTO elColaboradorEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerColaboradorPorIdLN.Obtener(elColaboradorEnVista.IdColaborador);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
            {{
                ""IdColaborador"": {datosAnteriores.IdColaborador},
                ""NombreColaborador"": ""{datosAnteriores.NombreColaborador}"",
                ""PrimerApellidoColaborador"": ""{datosAnteriores.PrimerApellidoColaborador}"",
                ""SegundoApellidoColaborador"": ""{datosAnteriores.SegundoApellidoColaborador}"",
                ""CedulaColaborador"": ""{datosAnteriores.CedulaColaborador}"",
                ""PuestoColaborador"": ""{datosAnteriores.PuestoColaborador}"",
                ""EstadoLaboral"": ""{datosAnteriores.EstadoLaboral}""
            }}";

                // Convertir el DTO a la entidad para su actualización
                var colaborador = _convertir.Convertir(elColaboradorEnVista);

                // Realizar la actualización con el método 'Update'
                int cantidadDeDatosActualizados = await _editarColaborador.Editar(colaborador);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
            {{
                ""IdColaborador"": {elColaboradorEnVista.IdColaborador},
                ""NombreColaborador"": ""{elColaboradorEnVista.NombreColaborador}"",
                ""PrimerApellidoColaborador"": ""{elColaboradorEnVista.PrimerApellidoColaborador}"",
                ""SegundoApellidoColaborador"": ""{elColaboradorEnVista.SegundoApellidoColaborador}"",
                ""CedulaColaborador"": ""{elColaboradorEnVista.CedulaColaborador}"",
                ""PuestoColaborador"": ""{elColaboradorEnVista.PuestoColaborador}"",
                ""EstadoLaboral"": ""{elColaboradorEnVista.EstadoLaboral}""
            }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloColaborador",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloColaborador",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosAnterioresJson,
                    DatosPosteriores = datosPosterioresJson
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                // Manejo de errores: registrar el evento en la bitácora
                var bitacoraError = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloColaborador",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloColaborador",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);

                throw; // Propagar el error después de registrarlo
            }
        }
    }
}

