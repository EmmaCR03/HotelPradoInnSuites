using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Temporadas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using HotelPrado.AccesoADatos.Temporadas.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Temporadas.Registrar
{
    public class RegistrarTemporadasLN : IRegistrarTemporadasLN
    {
        private readonly IRegistrarTemporadasAD _registrarTemporadasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarTemporadasLN()
        {
            _registrarTemporadasAD = new RegistrarTemporadasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(TemporadasDTO modelo)
        {
            try
            {
                // Convertir DTO a Tabla
                var temporadaTabla = ConvertirObjetoTemporadasTabla(modelo);

                // Guardar la Temporada en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarTemporadasAD.Guardar(temporadaTabla);

                // Preparar datos para registrar en la bitácora
                string datos = $@"{{
                    ""IdTemporada"": {modelo.IdTemporada},
                    ""NumeroTemporada"": {modelo.NumeroTemporada},
                    ""Descripcion"": ""{modelo.Descripcion}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva Temporada en la base de datos.",
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
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar la Temporada.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
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

