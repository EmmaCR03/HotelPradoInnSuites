using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelPrado.AccesoADatos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;

namespace HotelPrado.UI.Controllers
{
    public class NotificacionesController : Controller
    {
        // GET: Notificaciones/Obtener
        [HttpGet]
        [Authorize(Roles = "Administrador, Colaborador")]
        public JsonResult Obtener()
        {
            var notificaciones = new List<object>();

            try
            {
                using (var contexto = new Contexto())
                {
                    // Usar SQL directo para evitar problemas de inicialización de DbSet
                    try
                    {
                        // Obtener reservas pendientes usando SQL directo
                        // Buscar estados que contengan "Solicitada" o "Lista de Espera" (case-insensitive)
                        var sqlReservas = @"
                            SELECT TOP 20 
                                IdReserva, 
                                NombreCliente, 
                                EstadoReserva, 
                                FechaInicio
                            FROM Reservas 
                            WHERE LOWER(LTRIM(RTRIM(EstadoReserva))) = 'solicitada'
                               OR LOWER(LTRIM(RTRIM(EstadoReserva))) = 'en lista de espera'
                               OR EstadoReserva LIKE '%Solicitada%'
                               OR EstadoReserva LIKE '%Lista de Espera%'
                            ORDER BY FechaInicio DESC";

                        var reservas = contexto.Database.SqlQuery<ReservaNotificacion>(sqlReservas).ToList();

                        System.Diagnostics.Debug.WriteLine($"=== NOTIFICACIONES DEBUG ===");
                        System.Diagnostics.Debug.WriteLine($"Reservas encontradas: {reservas?.Count ?? 0}");
                        if (reservas != null && reservas.Any())
                        {
                            foreach (var r in reservas)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - Reserva #{r.IdReserva}: {r.NombreCliente}, Estado: '{r.EstadoReserva}', Fecha: {r.FechaInicio}");
                            }
                        }

                        if (reservas != null && reservas.Any())
                        {
                            var reservasNotif = reservas.Select(r => new
                            {
                                tipo = "reserva",
                                id = r.IdReserva,
                                mensaje = (r.EstadoReserva ?? "").ToLower().Contains("lista de espera")
                                    ? $"Nueva solicitud en lista de espera de {r.NombreCliente ?? "Cliente"}"
                                    : $"Nueva solicitud de reserva de {r.NombreCliente ?? "Cliente"}",
                                fecha = DateTime.Now, // Usar fecha actual (cuando se genera la notificación), no la fecha de la reserva
                                url = (r.EstadoReserva ?? "").ToLower().Contains("lista de espera")
                                    ? Url.Action("ListaEspera", "ReservasA")
                                    : Url.Action("SolicitudesPendientes", "ReservasA")
                            }).ToList();

                            notificaciones.AddRange(reservasNotif);
                            System.Diagnostics.Debug.WriteLine($"Notificaciones de reservas agregadas: {reservasNotif.Count}");
                        }
                    }
                    catch (Exception exReservas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al obtener reservas: {exReservas.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {exReservas.StackTrace}");
                    }

                    // Obtener citas pendientes usando SQL directo
                    try
                    {
                        // Buscar estados que contengan "pendiente" o "RespuestaPendiente" (case-insensitive)
                        var sqlCitas = @"
                            SELECT TOP 10 
                                IdCita, 
                                IdDepartamento, 
                                FechaCreacion,
                                Estado
                            FROM Citas 
                            WHERE LOWER(LTRIM(RTRIM(Estado))) LIKE '%pendiente%'
                               OR LOWER(LTRIM(RTRIM(Estado))) LIKE '%respuestapendiente%'
                               OR LOWER(LTRIM(RTRIM(Estado))) LIKE '%citapendiente%'
                            ORDER BY FechaCreacion DESC";

                        var citas = contexto.Database.SqlQuery<CitaNotificacionCompleta>(sqlCitas).ToList();

                        System.Diagnostics.Debug.WriteLine($"Citas encontradas: {citas?.Count ?? 0}");

                        if (citas != null && citas.Any())
                        {
                            var citasNotif = citas.Select(c => new
                            {
                                tipo = "cita",
                                id = c.IdCita,
                                mensaje = $"Nueva solicitud de cita para el departamento #{c.IdDepartamento}",
                                fecha = c.FechaCreacion,
                                url = Url.Action("IndexCitas", "Citas", new { id = c.IdDepartamento })
                            }).ToList();

                            notificaciones.AddRange(citasNotif);
                            System.Diagnostics.Debug.WriteLine($"Notificaciones de citas agregadas: {citasNotif.Count}");
                        }
                    }
                    catch (Exception exCitas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al obtener citas: {exCitas.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {exCitas.StackTrace}");
                    }

                    // Obtener solicitudes de limpieza pendientes
                    try
                    {
                        var sqlLimpieza = @"
                            SELECT TOP 10 
                                IdSolicitudLimpieza, 
                                Descripcion, 
                                Estado,
                                FechaSolicitud,
                                HabitacionNombre,
                                DepartamentoNombre
                            FROM SolicitudesLimpieza 
                            WHERE LOWER(LTRIM(RTRIM(Estado))) = 'pendiente'
                               OR LOWER(LTRIM(RTRIM(Estado))) = 'en proceso'
                            ORDER BY FechaSolicitud DESC";

                        var limpiezas = contexto.Database.SqlQuery<SolicitudLimpiezaNotificacion>(sqlLimpieza).ToList();

                        if (limpiezas != null && limpiezas.Any())
                        {
                            var limpiezasNotif = limpiezas.Select(l => new
                            {
                                tipo = "limpieza",
                                id = l.IdSolicitudLimpieza,
                                mensaje = $"Solicitud de limpieza {(l.Estado == "En Proceso" ? "en proceso" : "pendiente")}: {l.Descripcion ?? "Sin descripción"}",
                                fecha = DateTime.Now, // Usar fecha actual (cuando se genera la notificación), no la fecha de solicitud
                                url = Url.Action("IndexSolicitudLimpieza", "SolicitudLimpieza")
                            }).ToList();

                            notificaciones.AddRange(limpiezasNotif);
                        }
                    }
                    catch (Exception exLimpieza)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al obtener limpiezas: {exLimpieza.Message}");
                    }

                    // Obtener check-ins próximos (hoy y mañana)
                    try
                    {
                        var hoy = DateTime.Now.Date;
                        var manana = hoy.AddDays(1);
                        var pasadoManana = hoy.AddDays(2);

                        var sqlCheckIns = $@"
                            SELECT TOP 10 
                                IdReserva, 
                                NombreCliente, 
                                FechaInicio,
                                IdHabitacion
                            FROM Reservas 
                            WHERE (EstadoReserva = 'Confirmada' OR EstadoReserva = 'Solicitada')
                              AND FechaInicio IS NOT NULL
                              AND CAST(FechaInicio AS DATE) >= '{hoy:yyyy-MM-dd}'
                              AND CAST(FechaInicio AS DATE) < '{pasadoManana:yyyy-MM-dd}'
                            ORDER BY FechaInicio ASC";

                        var checkIns = contexto.Database.SqlQuery<CheckInNotificacion>(sqlCheckIns).ToList();

                        if (checkIns != null && checkIns.Any())
                        {
                            var checkInsNotif = checkIns.Select(c => new
                            {
                                tipo = "checkin",
                                id = c.IdReserva,
                                mensaje = $"Check-in próximo: {c.NombreCliente ?? "Cliente"} - Habitación #{c.IdHabitacion} - {c.FechaInicio?.ToString("dd/MM/yyyy") ?? "Fecha no definida"}",
                                fecha = DateTime.Now, // Usar fecha actual (cuando se genera la notificación)
                                url = Url.Action("EstadoHabitaciones", "Habitacion")
                            }).ToList();

                            notificaciones.AddRange(checkInsNotif);
                        }
                    }
                    catch (Exception exCheckIn)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al obtener check-ins: {exCheckIn.Message}");
                    }

                    // Obtener check-outs próximos (hoy y mañana)
                    try
                    {
                        var hoy = DateTime.Now.Date;
                        var manana = hoy.AddDays(1);
                        var pasadoManana = hoy.AddDays(2);

                        var sqlCheckOuts = $@"
                            SELECT TOP 10 
                                IdReserva, 
                                NombreCliente, 
                                FechaFinal,
                                IdHabitacion
                            FROM Reservas 
                            WHERE (EstadoReserva = 'Confirmada' OR EstadoReserva = 'En Proceso')
                              AND FechaFinal IS NOT NULL
                              AND CAST(FechaFinal AS DATE) >= '{hoy:yyyy-MM-dd}'
                              AND CAST(FechaFinal AS DATE) < '{pasadoManana:yyyy-MM-dd}'
                            ORDER BY FechaFinal ASC";

                        var checkOuts = contexto.Database.SqlQuery<CheckOutNotificacion>(sqlCheckOuts).ToList();

                        if (checkOuts != null && checkOuts.Any())
                        {
                            var checkOutsNotif = checkOuts.Select(c => new
                            {
                                tipo = "checkout",
                                id = c.IdReserva,
                                mensaje = $"Check-out próximo: {c.NombreCliente ?? "Cliente"} - Habitación #{c.IdHabitacion} - {c.FechaFinal?.ToString("dd/MM/yyyy") ?? "Fecha no definida"}",
                                fecha = DateTime.Now, // Usar fecha actual (cuando se genera la notificación)
                                url = Url.Action("EstadoHabitaciones", "Habitacion")
                            }).ToList();

                            notificaciones.AddRange(checkOutsNotif);
                        }
                    }
                    catch (Exception exCheckOut)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al obtener check-outs: {exCheckOut.Message}");
                    }
                }

                // Ordenar por fecha más reciente (sin límite, mostrar todas)
                notificaciones = notificaciones
                    .OrderByDescending(n =>
                    {
                        var fecha = ((dynamic)n).fecha;
                        return fecha is DateTime ? fecha : DateTime.MinValue;
                    })
                    .ToList()
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Total de notificaciones a retornar: {notificaciones.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al obtener notificaciones: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return Json(notificaciones, JsonRequestBehavior.AllowGet);
        }

        // Clases auxiliares para SQL directo
        private class ReservaNotificacion
        {
            public int IdReserva { get; set; }
            public string NombreCliente { get; set; }
            public string EstadoReserva { get; set; }
            public DateTime? FechaInicio { get; set; }
        }

        private class CitaNotificacion
        {
            public int IdCita { get; set; }
            public int IdDepartamento { get; set; }
            public DateTime FechaCreacion { get; set; }
        }

        private class CitaNotificacionCompleta
        {
            public int IdCita { get; set; }
            public int IdDepartamento { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string Estado { get; set; }
        }

        private class SolicitudLimpiezaNotificacion
        {
            public int IdSolicitudLimpieza { get; set; }
            public string Descripcion { get; set; }
            public string Estado { get; set; }
            public DateTime? FechaSolicitud { get; set; }
            public string HabitacionNombre { get; set; }
            public string DepartamentoNombre { get; set; }
        }

        private class CheckInNotificacion
        {
            public int IdReserva { get; set; }
            public string NombreCliente { get; set; }
            public DateTime? FechaInicio { get; set; }
            public int IdHabitacion { get; set; }
        }

        private class CheckOutNotificacion
        {
            public int IdReserva { get; set; }
            public string NombreCliente { get; set; }
            public DateTime? FechaFinal { get; set; }
            public int IdHabitacion { get; set; }
        }

        // POST: Notificaciones/MarcarLeida
        [HttpPost]
        public JsonResult MarcarLeida(int id, string tipo)
        {
            // En una implementación completa, aquí se marcaría como leída en la base de datos
            // Por ahora, solo retornamos éxito
            return Json(new { success = true });
        }
    }
}

