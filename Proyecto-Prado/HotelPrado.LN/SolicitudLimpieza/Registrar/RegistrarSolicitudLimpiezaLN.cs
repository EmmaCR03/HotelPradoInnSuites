using HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza;
using HotelPrado.AccesoADatos.SolicitudLimpieza.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.SolicitudLimpieza.Registrar
{
    public class RegistrarSolicitudLimpiezaLN : IRegistrarSolicitudLimpiezaLN
    {
        private readonly IRegistrarSolicitudLimpiezaAD _registrarSolicitudLimpiezaAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarSolicitudLimpiezaLN()
        {
            _registrarSolicitudLimpiezaAD = new RegistrarSolicitudLimpiezaAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(SolicitudLimpiezaDTO modelo)
        {
            try
            {
                // Guardar la Solicitud de Limpieza en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarSolicitudLimpiezaAD.Guardar(ConvertirObjetoSolicitudLimpiezaTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdSolicitudLimpieza"": {modelo.IdSolicitudLimpieza},
                    ""Descripcion"": ""{modelo.Descripcion}"",
                    ""Estado"": ""{modelo.Estado}"",
                    ""idDepartamento"": {modelo.idDepartamento},
                    ""DepartamentoNombre"": ""{modelo.DepartamentoNombre ?? ""}"",
                    ""idHabitacion"": {modelo.idHabitacion},
                    ""HabitacionNombre"": ""{modelo.HabitacionNombre ?? ""}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloSolicitudLimpieza",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva Solicitud de Limpieza en la base de datos.",
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
                    TablaDeEvento = "ModuloSolicitudLimpieza",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar la Solicitud de Limpieza.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private SolicitudLimpiezaTabla ConvertirObjetoSolicitudLimpiezaTabla(SolicitudLimpiezaDTO laSolicitudLimpieza)
        {
            return new SolicitudLimpiezaTabla
            {
                IdSolicitudLimpieza = laSolicitudLimpieza.IdSolicitudLimpieza,
                Descripcion = laSolicitudLimpieza.Descripcion,
                Estado = laSolicitudLimpieza.Estado ?? "Pendiente",
                // Convertir 0 a NULL para las claves foráneas
                idDepartamento = laSolicitudLimpieza.idDepartamento > 0 ? (int?)laSolicitudLimpieza.idDepartamento : null,
                DepartamentoNombre = laSolicitudLimpieza.DepartamentoNombre,
                idHabitacion = laSolicitudLimpieza.idHabitacion > 0 ? (int?)laSolicitudLimpieza.idHabitacion : null,
                FechaSolicitud = laSolicitudLimpieza.FechaSolicitud.HasValue ? laSolicitudLimpieza.FechaSolicitud.Value : (DateTime?)DateTime.Now
            };
        }
    }
}

