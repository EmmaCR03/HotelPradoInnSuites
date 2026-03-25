using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Temporadas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using HotelPrado.AccesoADatos.Temporadas.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Temporadas.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Temporadas.Editar
{
    public class EditarTemporadasLN : IEditarTemporadasLN
    {
        private readonly IEditarTemporadasAD _editarTemporadasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerTemporadasPorIdLN _obtenerTemporadasPorIdLN;

        public EditarTemporadasLN()
        {
            _editarTemporadasAD = new EditarTemporadasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerTemporadasPorIdLN = new ObtenerTemporadasPorIdLN();
        }

        public async Task<int> Actualizar(TemporadasDTO laTemporadaEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerTemporadasPorIdLN.Obtener(laTemporadaEnVista.IdTemporada);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"{{
                    ""IdTemporada"": {datosAnteriores.IdTemporada},
                    ""NumeroTemporada"": {datosAnteriores.NumeroTemporada},
                    ""Descripcion"": ""{datosAnteriores.Descripcion}""
                }}";

                // Convertir el DTO a la entidad para su actualización
                var temporadaTabla = ConvertirObjetoTemporadasTabla(laTemporadaEnVista);

                // Realizar la actualización
                int cantidadDeDatosActualizados = await _editarTemporadasAD.Editar(temporadaTabla);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"{{
                    ""IdTemporada"": {laTemporadaEnVista.IdTemporada},
                    ""NumeroTemporada"": {laTemporadaEnVista.NumeroTemporada},
                    ""Descripcion"": ""{laTemporadaEnVista.Descripcion}""
                }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloTemporadas",
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
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloTemporadas",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);
                throw;
            }
        }

        private TemporadasTabla ConvertirObjetoTemporadasTabla(TemporadasDTO laTemporada)
        {
            return new TemporadasTabla
            {
                IdTemporada = laTemporada.IdTemporada,
                NumeroTemporada = laTemporada.NumeroTemporada,
                Descripcion = laTemporada.Descripcion,
                CodigoCuenta = laTemporada.CodigoCuenta,
                AumentaAl = laTemporada.AumentaAl
            };
        }
    }
}

