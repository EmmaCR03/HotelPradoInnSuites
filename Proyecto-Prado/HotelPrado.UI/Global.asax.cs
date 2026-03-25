using HotelPrado.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HotelPrado.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static System.Threading.Timer _limpiezaTimer;
        private const int DIAS_ANTIGUEDAD_LIMPIEZA = 30; // Eliminar solicitudes completadas con más de 30 días
        private const int DIAS_BITACORA_CONSERVAR = 90; // Eliminar bitácora con más de 90 días (evita llenar la BD)
        private const int INTERVALO_LIMPIEZA_HORAS = 24; // Ejecutar limpieza cada 24 horas

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Calentar la BD en segundo plano para que la primera petición del usuario no espere la primera conexión
            System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1500);
                try
                {
                    using (var ctx = new HotelPrado.AccesoADatos.Contexto())
                    {
                        ctx.Database.ExecuteSqlCommand("SELECT 1");
                    }
                }
                catch { /* Ignorar si la BD no está lista */ }
            });

            // RolInitialize se ejecuta en segundo plano y muy retrasado para no competir con las primeras peticiones
            System.Threading.Tasks.Task.Run(async () => {
                await System.Threading.Tasks.Task.Delay(10000);
                try {
                    RolInitialize.Inicializar();
                } catch { /* Ignorar errores en inicialización */ }
            });
            
            IniciarLimpiezaAutomatica();
        }

        /// <summary>Para OutputCache VaryByCustom="User": así la Home no muestra "sesión iniciada" después de cerrar sesión (caché distinta para anónimos y autenticados).</summary>
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (string.Equals(custom, "User", StringComparison.OrdinalIgnoreCase))
            {
                var user = context?.User;
                if (user?.Identity?.IsAuthenticated == true)
                    return "auth-" + (user.Identity.Name ?? "");
                return "anon";
            }
            return base.GetVaryByCustomString(context, custom);
        }

        private void IniciarLimpiezaAutomatica()
        {
            // No ejecutar limpieza al inicio; solo cada INTERVALO_LIMPIEZA_HORAS para no frenar el arranque
            var intervalo = TimeSpan.FromHours(INTERVALO_LIMPIEZA_HORAS);
            _limpiezaTimer = new System.Threading.Timer(
                callback: _ => {
                    try
                    {
                        LimpiarSolicitudesAntiguas();
                        LimpiarBitacoraAntigua();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en limpieza automática programada: {ex.Message}");
                    }
                },
                state: null,
                dueTime: intervalo,
                period: intervalo
            );
        }

        private void LimpiarSolicitudesAntiguas()
        {
            try
            {
                using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                {
                    var connectionString = contexto.Database.Connection.ConnectionString;
                    using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        using (var deleteCommand = new System.Data.SqlClient.SqlCommand(@"
                            DELETE FROM SolicitudesLimpieza 
                            WHERE Estado = 'Completado' 
                              AND FechaSolicitud < DATEADD(day, -@DiasAntiguedad, GETDATE())", connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@DiasAntiguedad", DIAS_ANTIGUEDAD_LIMPIEZA);
                            int rowsDeleted = deleteCommand.ExecuteNonQuery();
                            
                            if (rowsDeleted > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[Limpieza Automática] Se eliminaron {rowsDeleted} solicitudes de limpieza completadas con más de {DIAS_ANTIGUEDAD_LIMPIEZA} días de antigüedad.");
                                
                                // Registrar en bitácora
                                try
                                {
                                    var registrarBitacora = new HotelPrado.LN.Bitacora.Registrar.RegistrarBitacoraEventosLN();
                                    var bitacora = new HotelPrado.Abstracciones.Modelos.Bitacora.BitacoraEventosDTO
                                    {
                                        IdEvento = 0,
                                        TablaDeEvento = "ModuloSolicitudLimpieza",
                                        TipoDeEvento = "LimpiezaAutomatica",
                                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                                        DescripcionDeEvento = $"Limpieza automática: Se eliminaron {rowsDeleted} solicitudes de limpieza completadas con más de {DIAS_ANTIGUEDAD_LIMPIEZA} días de antigüedad.",
                                        StackTrace = "Sin errores",
                                        DatosAnteriores = "NA",
                                        DatosPosteriores = $"{{\"SolicitudesEliminadas\": {rowsDeleted}, \"DiasAntiguedad\": {DIAS_ANTIGUEDAD_LIMPIEZA}}}"
                                    };
                                    registrarBitacora.RegistrarBitacora(bitacora);
                                }
                                catch (Exception exBitacora)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al registrar en bitácora: {exBitacora.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en limpieza automática de solicitudes: {ex.Message}");
            }
        }

        /// <summary>Elimina registros antiguos de bitacoraEventos para que la BD no se llene (cada 24 h se borran &gt; 90 días).</summary>
        private void LimpiarBitacoraAntigua()
        {
            try
            {
                using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                {
                    var connectionString = contexto.Database.Connection.ConnectionString;
                    using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var cmd = new System.Data.SqlClient.SqlCommand(@"
                            DELETE FROM bitacoraEventos 
                            WHERE FechaDeEvento < DATEADD(day, -@Dias, GETDATE())", connection))
                        {
                            cmd.Parameters.AddWithValue("@Dias", DIAS_BITACORA_CONSERVAR);
                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                                System.Diagnostics.Debug.WriteLine($"[Limpieza Automática] Bitácora: se eliminaron {rows} registros con más de {DIAS_BITACORA_CONSERVAR} días.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en limpieza de bitácora: {ex.Message}");
            }
        }

        /// <summary>Si algo falla: guardamos el error y mostramos mensaje. NUNCA redirigir (evita bucle en hosting).</summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var msg = (ex != null ? ex.Message : "Error desconocido") ?? "Error";
            var exType = ex != null ? ex.GetType().FullName : "Desconocido";
            var stackTrace = ex != null ? ex.StackTrace : "No disponible";
            var innerMsg = ex != null && ex.InnerException != null ? ex.InnerException.Message : "";

            // Guardar en Application por si la petición termina en ~/Error (así la vista Error puede mostrar el detalle)
            try
            {
                Application["LastError"] = msg;
                Application["LastErrorStackTrace"] = stackTrace;
                Application["LastErrorType"] = exType;
                Application["LastInnerError"] = innerMsg;
            }
            catch { }

            try { Server.ClearError(); } catch { }
            try
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.ContentType = "text/html; charset=utf-8";
                try { Response.TrySkipIisCustomErrors = true; } catch { }

                Response.Write("<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error del Servidor</title></head><body style='font-family:sans-serif;padding:2rem;max-width:800px;margin:0 auto;'>");
                Response.Write("<h1 style='color:#c00'>Error 500 - Error del Servidor</h1>");
                Response.Write("<p><strong>Mensaje:</strong> " + System.Web.HttpUtility.HtmlEncode(msg) + "</p>");
                Response.Write("<p><strong>Tipo:</strong> " + System.Web.HttpUtility.HtmlEncode(exType) + "</p>");

                if (!string.IsNullOrEmpty(innerMsg))
                {
                    Response.Write("<p><strong>Error interno:</strong> " + System.Web.HttpUtility.HtmlEncode(innerMsg) + "</p>");
                }

                Response.Write("<details style='margin-top:1rem;' open='open'><summary style='cursor:pointer;color:#0066cc;'>Stack trace</summary>");
                Response.Write("<pre style='background:#f5f5f5;padding:1rem;overflow:auto;max-height:400px;'>" + System.Web.HttpUtility.HtmlEncode(stackTrace) + "</pre>");
                Response.Write("</details>");

                Response.Write("<p><strong>Posibles soluciones:</strong></p>");
                Response.Write("<ul>");
                Response.Write("<li>Verifique la cadena de conexión en Web.config</li>");
                Response.Write("<li>Verifique que la base de datos esté accesible</li>");
                Response.Write("<li>Verifique que todas las DLLs estén en la carpeta /bin</li>");
                Response.Write("<li>Revise los logs del servidor para más detalles</li>");
                Response.Write("</ul>");

                Response.Write("<p><a href='/'>Ir al inicio</a> | <a href='/Account/Login'>Iniciar sesión</a> | <a href='/Home/Ping'>Probar Ping</a> | <a href='/Home/Diagnostico'>Diagnóstico</a></p>");
                Response.Write("</body></html>");
                Response.End();
            }
            catch { }
        }

        protected void Application_End()
        {
            // Limpiar el timer cuando la aplicación termine
            _limpiezaTimer?.Dispose();
        }
    }
}
