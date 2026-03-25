using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Listar;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.UI.Models;
using HotelPrado.AccesoADatos;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace HotelPrado.UI.Services
{
    /// <summary>
    /// Servicio para obtener el estado de habitaciones, departamentos y reservas
    /// Separa la lógica de negocio de la presentación
    /// </summary>
    public class HabitacionEstadoService
    {
        private readonly IReservasLN _reservasLN;
        private readonly IListarHabitacionesLN _listarHabitacionesLN;
        private readonly IListarReservasLN _listarReservasLN;
        private readonly IListarMantenimientoLN _listarMantenimientoLN;
        private readonly IListarDepartamentosLN _listarDepartamentosLN;
        private readonly IListarSolicitudLimpiezaLN _listarSolicitudLimpiezaLN;
        private readonly Contexto _contexto;

        public HabitacionEstadoService(IReservasLN reservasLN, IListarHabitacionesLN listarHabitacionesLN, IListarReservasLN listarReservasLN)
        {
            _reservasLN = reservasLN;
            _listarHabitacionesLN = listarHabitacionesLN;
            _listarReservasLN = listarReservasLN;
            _listarMantenimientoLN = new HotelPrado.LN.Mantenimiento.Listar.ListarMantenimientoLN();
            _listarDepartamentosLN = new HotelPrado.LN.Departamentos.Listar.ListarDepartamentoLN();
            _listarSolicitudLimpiezaLN = new HotelPrado.LN.SolicitudLimpieza.Listar.ListarSolicitudLimpiezaLN();
            _contexto = new Contexto();
        }

        /// <summary>
        /// Obtiene los eventos del calendario para todas las habitaciones y departamentos
        /// Incluye reservas y mantenimientos
        /// </summary>
        public CalendarioHabitacionesViewModel ObtenerEventosCalendario()
        {
            var habitaciones = _listarHabitacionesLN?.Listar() ?? new List<HabitacionesDTO>();
            var departamentos = _listarDepartamentosLN?.Listar() ?? new List<DepartamentoDTO>();
            var todasLasReservas = ObtenerTodasLasReservas();
            var todosLosMantenimientos = _listarMantenimientoLN?.Listar() ?? new List<MantenimientoDTO>();
            var todasLasSolicitudesLimpieza = _listarSolicitudLimpiezaLN?.Listar() ?? new List<SolicitudLimpiezaDTO>();

            var eventos = new List<EventoCalendario>();
            var habitacionesInfo = new List<HabitacionInfo>();

            // Procesar habitaciones
            foreach (var habitacion in habitaciones)
            {
                habitacionesInfo.Add(new HabitacionInfo
                {
                    IdHabitacion = habitacion.IdHabitacion,
                    NumeroHabitacion = habitacion.NumeroHabitacion,
                    Estado = habitacion.Estado,
                    CapacidadMax = habitacion.CapacidadMax
                });

                // Agregar reservas de la habitación
                var reservasHabitacion = todasLasReservas
                    .Where(r => r.IdHabitacion == habitacion.IdHabitacion && 
                                r.FechaInicio.HasValue && 
                                r.FechaFinal.HasValue)
                    .ToList();

                foreach (var reserva in reservasHabitacion)
                {
                    var nombreCliente = !string.IsNullOrEmpty(reserva.NombreCliente) ? reserva.NombreCliente : "Sin nombre";
                    var numeroHab = !string.IsNullOrEmpty(habitacion.NumeroHabitacion) ? habitacion.NumeroHabitacion : habitacion.IdHabitacion.ToString();
                    var estadoReserva = !string.IsNullOrEmpty(reserva.EstadoReserva) ? reserva.EstadoReserva : "Pendiente";
                    
                    // Acortar nombre del cliente si es muy largo
                    var nombreCorto = nombreCliente.Length > 15 ? nombreCliente.Substring(0, 12) + "..." : nombreCliente;
                    
                    // Formatear fechas con horas para FullCalendar (formato ISO 8601)
                    var fechaInicioConHora = reserva.FechaInicio.Value;
                    var fechaFinConHora = reserva.FechaFinal.Value;
                    
                    // Si no tiene hora específica, usar hora por defecto (check-in: 15:00, check-out: 11:00)
                    if (fechaInicioConHora.Hour == 0 && fechaInicioConHora.Minute == 0)
                    {
                        fechaInicioConHora = fechaInicioConHora.Date.AddHours(15); // Check-in a las 3 PM
                    }
                    if (fechaFinConHora.Hour == 0 && fechaFinConHora.Minute == 0)
                    {
                        fechaFinConHora = fechaFinConHora.Date.AddHours(11); // Check-out a las 11 AM
                    }
                    
                    eventos.Add(new EventoCalendario
                    {
                        IdReserva = reserva.IdReserva,
                        IdHabitacion = reserva.IdHabitacion,
                        NumeroHabitacion = numeroHab,
                        Titulo = "Hab. " + numeroHab + " - " + nombreCorto,
                        FechaInicio = fechaInicioConHora.ToString("yyyy-MM-ddTHH:mm:ss"),
                        FechaFin = fechaFinConHora.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"), // FullCalendar usa exclusivo
                        HoraCheckIn = fechaInicioConHora.ToString("HH:mm"),
                        HoraCheckOut = fechaFinConHora.ToString("HH:mm"),
                        Estado = estadoReserva,
                        Color = ObtenerColorPorEstado(estadoReserva),
                        Cliente = nombreCliente,
                        CantidadPersonas = reserva.cantidadPersonas
                    });
                }

                // Agregar mantenimientos activos de la habitación
                var mantenimientosHabitacion = todosLosMantenimientos
                    .Where(m => m.idHabitacion == habitacion.IdHabitacion && 
                                (m.Estado == "Pendiente" || m.Estado == "En Proceso"))
                    .ToList();

                foreach (var mantenimiento in mantenimientosHabitacion)
                {
                    var numeroHab = !string.IsNullOrEmpty(habitacion.NumeroHabitacion) ? habitacion.NumeroHabitacion : habitacion.IdHabitacion.ToString();
                    var descripcion = !string.IsNullOrEmpty(mantenimiento.Descripcion) ? mantenimiento.Descripcion : "Mantenimiento";
                    
                    // Mostrar solo el día actual, se actualizará automáticamente cada día
                    var fechaHoy = DateTime.Now.Date;
                    var fechaManana = fechaHoy.AddDays(1);
                    
                    // Acortar descripción si es muy larga
                    var descripcionCorta = descripcion.Length > 20 ? descripcion.Substring(0, 17) + "..." : descripcion;
                    
                    eventos.Add(new EventoCalendario
                    {
                        IdReserva = 0, // No es una reserva
                        IdHabitacion = habitacion.IdHabitacion,
                        NumeroHabitacion = numeroHab,
                        Titulo = "Hab. " + numeroHab + " - Mant.",
                        FechaInicio = fechaHoy.ToString("yyyy-MM-dd"), // Solo el día de hoy
                        FechaFin = fechaManana.ToString("yyyy-MM-dd"), // Hasta mañana (FullCalendar usa exclusivo)
                        Estado = "Mantenimiento",
                        Color = "#ffc107", // Amarillo para mantenimiento
                        Cliente = descripcion,
                        CantidadPersonas = 0
                    });
                }

                // Agregar solicitudes de limpieza pendientes de la habitación
                var solicitudesLimpiezaHabitacion = todasLasSolicitudesLimpieza
                    .Where(s => s.idHabitacion == habitacion.IdHabitacion && 
                                (s.Estado == "Pendiente" || s.Estado == "En Proceso"))
                    .ToList();

                foreach (var solicitud in solicitudesLimpiezaHabitacion)
                {
                    var numeroHab = !string.IsNullOrEmpty(habitacion.NumeroHabitacion) ? habitacion.NumeroHabitacion : habitacion.IdHabitacion.ToString();
                    var descripcion = !string.IsNullOrEmpty(solicitud.Descripcion) ? solicitud.Descripcion : "Limpieza";
                    
                    // Mostrar solo el día actual, se actualizará automáticamente cada día
                    var fechaHoy = DateTime.Now.Date;
                    var fechaManana = fechaHoy.AddDays(1);
                    
                    // Acortar descripción si es muy larga
                    var descripcionCorta = descripcion.Length > 20 ? descripcion.Substring(0, 17) + "..." : descripcion;
                    
                    eventos.Add(new EventoCalendario
                    {
                        IdReserva = 0, // No es una reserva
                        IdHabitacion = habitacion.IdHabitacion,
                        NumeroHabitacion = numeroHab,
                        Titulo = "Hab. " + numeroHab + " - Limp.",
                        FechaInicio = fechaHoy.ToString("yyyy-MM-dd"), // Solo el día de hoy
                        FechaFin = fechaManana.ToString("yyyy-MM-dd"), // Hasta mañana (FullCalendar usa exclusivo)
                        Estado = "Limpieza",
                        Color = "#17a2b8", // Azul para limpieza
                        Cliente = descripcion,
                        CantidadPersonas = 0
                    });
                }
            }

            // Procesar departamentos - agregar mantenimientos y limpieza
            foreach (var departamento in departamentos)
            {
                var mantenimientosDepartamento = todosLosMantenimientos
                    .Where(m => m.idDepartamento == departamento.IdDepartamento && 
                                (m.Estado == "Pendiente" || m.Estado == "En Proceso"))
                    .ToList();

                foreach (var mantenimiento in mantenimientosDepartamento)
                {
                    var descripcion = !string.IsNullOrEmpty(mantenimiento.Descripcion) ? mantenimiento.Descripcion : "Mantenimiento";
                    
                    // Mostrar solo el día actual, se actualizará automáticamente cada día
                    var fechaHoy = DateTime.Now.Date;
                    var fechaManana = fechaHoy.AddDays(1);
                    
                    // Acortar descripción si es muy larga
                    var descripcionCorta = descripcion.Length > 20 ? descripcion.Substring(0, 17) + "..." : descripcion;
                    
                    var numDepto = (departamento.NumeroDepartamento == null || departamento.NumeroDepartamento == 0) ? "Sin número" : departamento.NumeroDepartamento.ToString();
                    eventos.Add(new EventoCalendario
                    {
                        IdReserva = 0, // No es una reserva
                        IdHabitacion = 0, // No es una habitación
                        NumeroHabitacion = numDepto,
                        Titulo = "Dept. " + numDepto + " - Mant.",
                        FechaInicio = fechaHoy.ToString("yyyy-MM-dd"), // Solo el día de hoy
                        FechaFin = fechaManana.ToString("yyyy-MM-dd"), // Hasta mañana (FullCalendar usa exclusivo)
                        Estado = "Mantenimiento",
                        Color = "#ffc107", // Amarillo para mantenimiento
                        Cliente = descripcion,
                        CantidadPersonas = 0
                    });
                }

                // Agregar solicitudes de limpieza pendientes del departamento
                var solicitudesLimpiezaDepartamento = todasLasSolicitudesLimpieza
                    .Where(s => s.idDepartamento == departamento.IdDepartamento && 
                                (s.Estado == "Pendiente" || s.Estado == "En Proceso"))
                    .ToList();

                foreach (var solicitud in solicitudesLimpiezaDepartamento)
                {
                    var descripcion = !string.IsNullOrEmpty(solicitud.Descripcion) ? solicitud.Descripcion : "Limpieza";
                    
                    // Mostrar solo el día actual, se actualizará automáticamente cada día
                    var fechaHoy = DateTime.Now.Date;
                    var fechaManana = fechaHoy.AddDays(1);
                    
                    // Acortar descripción si es muy larga
                    var descripcionCorta = descripcion.Length > 20 ? descripcion.Substring(0, 17) + "..." : descripcion;
                    
                    var numDeptoLimp = (departamento.NumeroDepartamento == null || departamento.NumeroDepartamento == 0) ? "Sin número" : departamento.NumeroDepartamento.ToString();
                    eventos.Add(new EventoCalendario
                    {
                        IdReserva = 0, // No es una reserva
                        IdHabitacion = 0, // No es una habitación
                        NumeroHabitacion = numDeptoLimp,
                        Titulo = "Dept. " + numDeptoLimp + " - Limp.",
                        FechaInicio = fechaHoy.ToString("yyyy-MM-dd"), // Solo el día de hoy
                        FechaFin = fechaManana.ToString("yyyy-MM-dd"), // Hasta mañana (FullCalendar usa exclusivo)
                        Estado = "Limpieza",
                        Color = "#17a2b8", // Azul para limpieza
                        Cliente = descripcion,
                        CantidadPersonas = 0
                    });
                }
            }

            return new CalendarioHabitacionesViewModel
            {
                Eventos = eventos != null ? eventos : new List<EventoCalendario>(),
                Habitaciones = habitacionesInfo != null ? habitacionesInfo : new List<HabitacionInfo>()
            };
        }

        /// <summary>
        /// Obtiene el estado actual de todas las habitaciones y departamentos.
        /// Si se pasa fecha, se usa como día de referencia (para ver estado en esa fecha).
        /// </summary>
        public EstadoHabitacionesViewModel ObtenerEstadoHabitaciones(DateTime? fecha = null)
        {
            List<HabitacionesDTO> habitaciones = new List<HabitacionesDTO>();
            List<DepartamentoDTO> departamentos = new List<DepartamentoDTO>();
            List<ReservasDTO> todasLasReservas = new List<ReservasDTO>();
            List<MantenimientoDTO> todosLosMantenimientos = new List<MantenimientoDTO>();
            
            try
            {
                if (_listarHabitacionesLN != null)
                {
                    habitaciones = _listarHabitacionesLN.Listar() ?? new List<HabitacionesDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log error pero continuar con lista vacía
                habitaciones = new List<HabitacionesDTO>();
            }
            
            try
            {
                if (_listarDepartamentosLN != null)
                {
                    departamentos = _listarDepartamentosLN.Listar() ?? new List<DepartamentoDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log error pero continuar con lista vacía
                departamentos = new List<DepartamentoDTO>();
            }
            
            try
            {
                todasLasReservas = ObtenerTodasLasReservas();
            }
            catch (Exception ex)
            {
                // Log error pero continuar con lista vacía
                todasLasReservas = new List<ReservasDTO>();
            }
            
            try
            {
                if (_listarMantenimientoLN != null)
                {
                    todosLosMantenimientos = _listarMantenimientoLN.Listar() ?? new List<MantenimientoDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log error pero continuar con lista vacía
                todosLosMantenimientos = new List<MantenimientoDTO>();
            }
            
            List<SolicitudLimpiezaDTO> todasLasSolicitudesLimpieza = new List<SolicitudLimpiezaDTO>();
            try
            {
                if (_listarSolicitudLimpiezaLN != null)
                {
                    todasLasSolicitudesLimpieza = _listarSolicitudLimpiezaLN.Listar() ?? new List<SolicitudLimpiezaDTO>();
                }
            }
            catch (Exception ex)
            {
                // Log error pero continuar con lista vacía
                todasLasSolicitudesLimpieza = new List<SolicitudLimpiezaDTO>();
            }
            
            var hoy = (fecha ?? DateTime.Now).Date;

            var estadosHabitaciones = new List<EstadoHabitacion>();
            var estadosDepartamentos = new List<EstadoDepartamento>();
            var estadisticas = new EstadisticasResumen
            {
                TotalHabitaciones = habitaciones != null ? habitaciones.Count : 0,
                TotalDepartamentos = departamentos != null ? departamentos.Count : 0
            };

            // Procesar habitaciones
            if (habitaciones != null)
            {
                foreach (var habitacion in habitaciones)
                {
                    if (habitacion == null) continue;
                    
                    ReservasDTO reservaActiva = null;
                    try
                    {
                        reservaActiva = todasLasReservas
                            .Where(r => r != null && 
                                       r.IdHabitacion == habitacion.IdHabitacion &&
                                       r.FechaInicio.HasValue &&
                                       r.FechaFinal.HasValue &&
                                       (r.EstadoReserva == "Confirmada" || r.EstadoReserva == "Solicitada" || r.EstadoReserva == "En Proceso") &&
                                       hoy >= r.FechaInicio.Value.Date &&
                                       hoy <= r.FechaFinal.Value.Date)
                            .OrderByDescending(r => r.FechaInicio)
                            .FirstOrDefault();
                    }
                    catch
                    {
                        // Continuar sin reserva activa
                    }

                    // Verificar si hay mantenimiento activo
                    MantenimientoDTO mantenimientoActivo = null;
                    try
                    {
                        mantenimientoActivo = todosLosMantenimientos
                            .Where(m => m != null && m.idHabitacion == habitacion.IdHabitacion && 
                                       (m.Estado == "Pendiente" || m.Estado == "En Proceso"))
                            .FirstOrDefault();
                    }
                    catch
                    {
                        // Continuar sin mantenimiento activo
                    }

                    // Verificar si hay solicitud de limpieza activa
                    SolicitudLimpiezaDTO solicitudLimpiezaActiva = null;
                    try
                    {
                        solicitudLimpiezaActiva = todasLasSolicitudesLimpieza
                            .Where(s => s != null && s.idHabitacion == habitacion.IdHabitacion && 
                                       (s.Estado == "Pendiente" || s.Estado == "En Proceso"))
                            .FirstOrDefault();
                    }
                    catch
                    {
                        // Continuar sin solicitud de limpieza activa
                    }

                    var estadoHabitacion = new EstadoHabitacion
                    {
                        IdHabitacion = habitacion.IdHabitacion,
                        NumeroHabitacion = !string.IsNullOrEmpty(habitacion.NumeroHabitacion) ? habitacion.NumeroHabitacion : habitacion.IdHabitacion.ToString(),
                        CapacidadMax = habitacion.CapacidadMax
                    };

                if (reservaActiva != null)
                {
                    estadoHabitacion.Estado = "Ocupada";
                    estadoHabitacion.EstadoReserva = !string.IsNullOrEmpty(reservaActiva.EstadoReserva) ? reservaActiva.EstadoReserva : "Confirmada";
                    estadoHabitacion.ClienteActual = !string.IsNullOrEmpty(reservaActiva.NombreCliente) ? reservaActiva.NombreCliente : "Sin nombre";
                    estadoHabitacion.FechaCheckIn = reservaActiva.FechaInicio;
                    estadoHabitacion.FechaCheckOut = reservaActiva.FechaFinal;
                    estadoHabitacion.CantidadPersonas = reservaActiva.cantidadPersonas;
                    estadoHabitacion.IdReserva = reservaActiva.IdReserva;
                    estadoHabitacion.DiasRestantes = (reservaActiva.FechaFinal.Value.Date - hoy).Days;
                    
                    // Calcular tiempo ocupada
                    if (reservaActiva.FechaInicio.HasValue)
                    {
                        var tiempoOcupada = DateTime.Now - reservaActiva.FechaInicio.Value;
                        estadoHabitacion.TiempoOcupada = FormatearTiempo(tiempoOcupada);
                    }
                    
                    // Calcular tiempo restante hasta check-out
                    if (reservaActiva.FechaFinal.HasValue)
                    {
                        var tiempoRestante = reservaActiva.FechaFinal.Value - DateTime.Now;
                        if (tiempoRestante.TotalSeconds > 0)
                        {
                            estadoHabitacion.TiempoRestanteOcupacion = FormatearTiempo(tiempoRestante);
                        }
                        else
                        {
                            estadoHabitacion.TiempoRestanteOcupacion = "Vencida";
                        }
                    }
                    
                    estadisticas.HabitacionesOcupadas++;
                }
                else if (mantenimientoActivo != null || habitacion.Estado == "Mantenimiento")
                {
                    estadoHabitacion.Estado = "Mantenimiento";
                    estadisticas.HabitacionesMantenimiento++;
                }
                else if (string.Equals(habitacion.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
                {
                    // La BD ya dice Disponible: no sobrescribir por solicitudes viejas (Pendiente/En Proceso)
                    estadoHabitacion.Estado = "Disponible";
                    estadisticas.HabitacionesDisponibles++;
                }
                else if (solicitudLimpiezaActiva != null)
                {
                    // Solo mostrar "Limpieza" si hay solicitud activa Y la habitación no está ya como Disponible en BD
                    estadoHabitacion.Estado = "Limpieza";
                    estadoHabitacion.DescripcionLimpieza = !string.IsNullOrEmpty(solicitudLimpiezaActiva.Descripcion) 
                        ? solicitudLimpiezaActiva.Descripcion 
                        : "Limpieza requerida";
                    
                    // Calcular tiempo en limpieza
                    if (solicitudLimpiezaActiva.FechaSolicitud.HasValue)
                    {
                        // Si está en proceso, usar FechaSolicitud como inicio de limpieza
                        if (solicitudLimpiezaActiva.Estado == "En Proceso")
                        {
                            estadoHabitacion.FechaInicioLimpieza = solicitudLimpiezaActiva.FechaSolicitud;
                            var tiempoEnLimpieza = DateTime.Now - solicitudLimpiezaActiva.FechaSolicitud.Value;
                            estadoHabitacion.TiempoEnLimpieza = FormatearTiempo(tiempoEnLimpieza);
                        }
                        else if (solicitudLimpiezaActiva.Estado == "Pendiente")
                        {
                            // Si está pendiente, mostrar tiempo desde solicitud
                            var tiempoDesdeSolicitud = DateTime.Now - solicitudLimpiezaActiva.FechaSolicitud.Value;
                            estadoHabitacion.TiempoEnLimpieza = "Pendiente: " + FormatearTiempo(tiempoDesdeSolicitud);
                        }
                    }
                    
                    estadisticas.HabitacionesLimpieza++;
                }
                else if (habitacion.Estado == "Limpieza")
                {
                    // Si el estado de la habitación es "Limpieza" pero no hay solicitud activa, 
                    // verificar directamente en la base de datos y corregir el estado
                    try
                    {
                        using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                        {
                            connection.Open();
                            using (var checkCommand = new System.Data.SqlClient.SqlCommand(@"
                                SELECT COUNT(*) 
                                FROM SolicitudesLimpieza 
                                WHERE idHabitacion = @IdHabitacion 
                                  AND LOWER(LTRIM(RTRIM(Estado))) IN ('pendiente', 'en proceso')", connection))
                            {
                                checkCommand.Parameters.AddWithValue("@IdHabitacion", habitacion.IdHabitacion);
                                int count = (int)checkCommand.ExecuteScalar();
                                
                                if (count > 0)
                                {
                                    // Hay solicitud activa, mostrar como limpieza
                                    estadoHabitacion.Estado = "Limpieza";
                                    estadoHabitacion.DescripcionLimpieza = "Limpieza requerida";
                                    estadisticas.HabitacionesLimpieza++;
                                }
                                else
                                {
                                    // No hay solicitud activa, actualizar estado a Disponible usando SQL directo
                                    using (var updateCommand = new System.Data.SqlClient.SqlCommand(@"
                                        UPDATE Habitaciones 
                                        SET Estado = 'Disponible'
                                        WHERE IdHabitacion = @IdHabitacion 
                                          AND LOWER(LTRIM(RTRIM(Estado))) = 'limpieza'", connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@IdHabitacion", habitacion.IdHabitacion);
                                        int rowsUpdated = updateCommand.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine($"Habitación {habitacion.IdHabitacion} corregida de Limpieza a Disponible: {rowsUpdated} filas");
                                    }
                                    estadoHabitacion.Estado = "Disponible";
                                    estadisticas.HabitacionesDisponibles++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al verificar/corregir estado de limpieza: {ex.Message}");
                        // Si falla la verificación, usar el estado de la habitación pero intentar corregir
                        estadoHabitacion.Estado = habitacion.Estado;
                        if (habitacion.Estado == "Limpieza")
                        {
                            estadisticas.HabitacionesLimpieza++;
                        }
                        else
                        {
                            estadisticas.HabitacionesDisponibles++;
                        }
                    }
                }
                else
                {
                    estadoHabitacion.Estado = "Disponible";
                    estadisticas.HabitacionesDisponibles++;
                }

                    // Colores y leyenda según estado detallado (check-in/check-out pronto, reservada, etc.)
                    AplicarColorYLeyendaHabitacion(estadoHabitacion, todasLasReservas, hoy);
                    estadoHabitacion.IconoEstado = "fas fa-bed"; // Siempre icono cama para habitación (diferenciar por icono, no por color)

                    estadosHabitaciones.Add(estadoHabitacion);
                }
            }

            // Procesar departamentos
            if (departamentos != null)
            {
                foreach (var departamento in departamentos)
                {
                    if (departamento == null) continue;
                    
                    // Verificar si hay mantenimiento activo
                    MantenimientoDTO mantenimientoActivo = null;
                    try
                    {
                        mantenimientoActivo = todosLosMantenimientos
                            .Where(m => m != null && m.idDepartamento == departamento.IdDepartamento && 
                                       (m.Estado == "Pendiente" || m.Estado == "En Proceso"))
                            .FirstOrDefault();
                    }
                    catch
                    {
                        // Continuar sin mantenimiento activo
                    }

                    // Verificar si hay solicitud de limpieza activa
                    SolicitudLimpiezaDTO solicitudLimpiezaActiva = null;
                    try
                    {
                        solicitudLimpiezaActiva = todasLasSolicitudesLimpieza
                            .Where(s => s != null && s.idDepartamento == departamento.IdDepartamento && 
                                       (s.Estado == "Pendiente" || s.Estado == "En Proceso"))
                            .FirstOrDefault();
                    }
                    catch
                    {
                        // Continuar sin solicitud de limpieza activa
                    }

                var numeroDepto = (departamento.NumeroDepartamento == null || departamento.NumeroDepartamento == 0) ? "Sin número" : departamento.NumeroDepartamento.ToString();
                var estadoDepartamento = new EstadoDepartamento
                {
                    IdDepartamento = departamento.IdDepartamento,
                    NumeroDepartamento = numeroDepto,
                    Nombre = !string.IsNullOrEmpty(departamento.Nombre) ? departamento.Nombre : "Sin nombre"
                };

                if (departamento.Estado == "Ocupado" || departamento.IdCliente.HasValue)
                {
                    estadoDepartamento.Estado = "Ocupado";
                    estadoDepartamento.ClienteActual = !string.IsNullOrEmpty(departamento.NombreCliente) ? departamento.NombreCliente : "Sin nombre";
                    estadisticas.DepartamentosOcupados++;
                }
                else if (mantenimientoActivo != null || departamento.Estado == "Mantenimiento")
                {
                    estadoDepartamento.Estado = "Mantenimiento";
                    estadoDepartamento.DescripcionMantenimiento = mantenimientoActivo != null ? mantenimientoActivo.Descripcion : "En mantenimiento";
                    estadisticas.DepartamentosMantenimiento++;
                }
                else if (string.Equals(departamento.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
                {
                    // La BD ya dice Disponible: no sobrescribir por solicitudes viejas
                    estadoDepartamento.Estado = "Disponible";
                    estadisticas.DepartamentosDisponibles++;
                }
                else if (solicitudLimpiezaActiva != null)
                {
                    estadoDepartamento.Estado = "Limpieza";
                    estadoDepartamento.DescripcionMantenimiento = solicitudLimpiezaActiva.Descripcion ?? "En limpieza";
                    estadisticas.DepartamentosDisponibles++;
                }
                else
                {
                    estadoDepartamento.Estado = "Disponible";
                    estadisticas.DepartamentosDisponibles++;
                }

                    estadoDepartamento.ColorEstado = ObtenerColorPorEstadoDepartamento(estadoDepartamento.Estado);
                    estadoDepartamento.IconoEstado = "fas fa-building"; // Siempre icono edificio para departamento
                    estadoDepartamento.EstadoLeyenda = estadoDepartamento.Estado == "Ocupado" ? "Ocupado" : estadoDepartamento.Estado == "Mantenimiento" ? "No Disponible" : estadoDepartamento.Estado == "Limpieza" ? "Limpieza" : "Libre";

                    estadosDepartamentos.Add(estadoDepartamento);
                }
            }

            // Calcular estadísticas adicionales
            try
            {
                estadisticas.ReservasHoy = todasLasReservas
                    .Where(r => r != null && r.FechaInicio.HasValue && 
                               r.FechaInicio.Value.Date == hoy && 
                               (r.EstadoReserva == "Confirmada" || string.IsNullOrEmpty(r.EstadoReserva)))
                    .Count();
            }
            catch
            {
                estadisticas.ReservasHoy = 0;
            }

            try
            {
                estadisticas.CheckOutsHoy = todasLasReservas
                    .Where(r => r != null && r.FechaFinal.HasValue && 
                               r.FechaFinal.Value.Date == hoy && 
                               (r.EstadoReserva == "Confirmada" || string.IsNullOrEmpty(r.EstadoReserva)))
                    .Count();
            }
            catch
            {
                estadisticas.CheckOutsHoy = 0;
            }

            return new EstadoHabitacionesViewModel
            {
                Habitaciones = estadosHabitaciones != null ? estadosHabitaciones.OrderBy(h => {
                    int num;
                    if (int.TryParse(h.NumeroHabitacion, out num))
                        return num;
                    return 999;
                }).ToList() : new List<EstadoHabitacion>(),
                Departamentos = estadosDepartamentos != null ? estadosDepartamentos.OrderBy(d => d.NumeroDepartamento).ToList() : new List<EstadoDepartamento>(),
                Estadisticas = estadisticas
            };
        }

        /// <summary>
        /// Obtiene todas las reservas del sistema
        /// </summary>
        private List<ReservasDTO> ObtenerTodasLasReservas()
        {
            var listado = _listarReservasLN != null ? _listarReservasLN.Listar() : null;
            return listado != null ? listado : new List<ReservasDTO>();
        }

        private string ObtenerColorPorEstado(string estadoReserva)
        {
            switch (estadoReserva)
            {
                case "Confirmada":
                    return "#28a745"; // Verde
                case "Solicitada":
                    return "#ffc107"; // Amarillo
                case "En Lista de Espera":
                    return "#17a2b8"; // Azul
                case "Pendiente":
                    return "#ffc107"; // Amarillo
                case "Cancelada":
                case "Rechazada":
                    return "#dc3545"; // Rojo
                default:
                    return "#6c757d"; // Gris
            }
        }

        private string ObtenerColorPorEstadoHabitacion(string estado)
        {
            switch (estado)
            {
                case "Disponible":
                    return "#28a745"; // Verde
                case "Ocupada":
                    return "#dc3545"; // Rojo
                case "Mantenimiento":
                    return "#ffc107"; // Amarillo
                case "Limpieza":
                    return "#17a2b8"; // Azul
                default:
                    return "#6c757d"; // Gris
            }
        }

        /// <summary>
        /// Asigna ColorEstado y EstadoLeyenda: Salen Hoy, Check-out pronto, Ocupada, Check-in pronto, Reservada, Libre, No Disponible, Limpieza.
        /// Habitación se diferencia por icono (cama), no por color.
        /// </summary>
        private void AplicarColorYLeyendaHabitacion(EstadoHabitacion estadoHabitacion, List<ReservasDTO> todasLasReservas, DateTime hoy)
        {
            if (estadoHabitacion.Estado == "Ocupada")
            {
                var fechaOut = estadoHabitacion.FechaCheckOut?.Date;
                var fechaIn = estadoHabitacion.FechaCheckIn?.Date;
                var diasOcupada = fechaIn.HasValue ? (hoy - fechaIn.Value).Days : 0;

                if (fechaOut == hoy)
                {
                    estadoHabitacion.EstadoLeyenda = "Check-out hoy";
                    estadoHabitacion.ColorEstado = "#b52a2a"; // Rojo oscuro: check-out ese día
                }
                else if (fechaOut.HasValue && (fechaOut.Value - hoy).Days >= 1 && (fechaOut.Value - hoy).Days <= 2)
                {
                    estadoHabitacion.EstadoLeyenda = "Check-out pronto";
                    estadoHabitacion.ColorEstado = "#dc3545"; // Rojo (ocupado)
                }
                else if (diasOcupada >= 1)
                {
                    estadoHabitacion.EstadoLeyenda = diasOcupada == 1 ? "Ocupada (+1 día)" : "Ocupada (+" + diasOcupada + " días)";
                    estadoHabitacion.ColorEstado = "#721c24"; // Granate: lleva todo un día o más ocupada
                }
                else
                {
                    estadoHabitacion.EstadoLeyenda = "Ocupada";
                    estadoHabitacion.ColorEstado = "#dc3545"; // Rojo (ocupado, entraron hoy)
                }
                return;
            }
            if (estadoHabitacion.Estado == "Mantenimiento")
            {
                estadoHabitacion.EstadoLeyenda = "No Disponible";
                estadoHabitacion.ColorEstado = "#dc3545"; // Rojo
                return;
            }
            if (estadoHabitacion.Estado == "Limpieza")
            {
                estadoHabitacion.EstadoLeyenda = "Limpieza";
                estadoHabitacion.ColorEstado = "#17a2b8"; // Azul
                return;
            }
            // Disponible: ver si tiene reservas futuras (check-in pronto o reservada); si hay varias, indicar cantidad
            var reservasFuturas = todasLasReservas
                .Where(r => r != null && r.IdHabitacion == estadoHabitacion.IdHabitacion && r.FechaInicio.HasValue &&
                            r.FechaInicio.Value.Date > hoy &&
                            (r.EstadoReserva == "Confirmada" || r.EstadoReserva == "Solicitada"))
                .OrderBy(r => r.FechaInicio)
                .ToList();
            var reservaFutura = reservasFuturas.FirstOrDefault();
            var cantidadReservas = reservasFuturas.Count;
            if (reservaFutura != null)
            {
                var diasHastaCheckIn = (reservaFutura.FechaInicio.Value.Date - hoy).Days;
                var textoReservas = cantidadReservas > 1 ? $"Reservada ({cantidadReservas})" : "Reservada";
                if (diasHastaCheckIn <= 1)
                {
                    estadoHabitacion.EstadoLeyenda = cantidadReservas > 1 ? "Check-in pronto (" + cantidadReservas + ")" : "Check-in pronto";
                    estadoHabitacion.ColorEstado = "#ffa500"; // Naranja
                }
                else if (diasHastaCheckIn <= 14)
                {
                    estadoHabitacion.EstadoLeyenda = textoReservas;
                    estadoHabitacion.ColorEstado = "#9370db"; // Púrpura
                }
                else
                {
                    estadoHabitacion.EstadoLeyenda = textoReservas;
                    estadoHabitacion.ColorEstado = "#9370db";
                }
            }
            else
            {
                estadoHabitacion.EstadoLeyenda = "Libre";
                estadoHabitacion.ColorEstado = "#28a745"; // Verde (disponible)
            }
        }

        private string ObtenerIconoPorEstado(string estado)
        {
            switch (estado)
            {
                case "Disponible":
                    return "fa-check-circle";
                case "Ocupada":
                case "Ocupado":
                    return "fa-bed";
                case "Mantenimiento":
                    return "fa-tools";
                case "Limpieza":
                    return "fa-broom";
                default:
                    return "fa-question-circle";
            }
        }

        private string ObtenerColorPorEstadoDepartamento(string estado)
        {
            switch (estado)
            {
                case "Disponible":
                    return "#28a745"; // Verde
                case "Ocupado":
                    return "#dc3545"; // Rojo
                case "Mantenimiento":
                    return "#ffc107"; // Amarillo
                case "Limpieza":
                    return "#17a2b8"; // Azul
                default:
                    return "#6c757d"; // Gris
            }
        }

        /// <summary>
        /// Formatea un TimeSpan a un string legible (ej: "2 días, 5 horas, 30 minutos")
        /// </summary>
        private string FormatearTiempo(TimeSpan tiempo)
        {
            if (tiempo.TotalDays >= 1)
            {
                var dias = (int)tiempo.TotalDays;
                var horas = tiempo.Hours;
                if (horas > 0)
                {
                    return $"{dias} día{(dias > 1 ? "s" : "")}, {horas} hora{(horas > 1 ? "s" : "")}";
                }
                return $"{dias} día{(dias > 1 ? "s" : "")}";
            }
            else if (tiempo.TotalHours >= 1)
            {
                var horas = (int)tiempo.TotalHours;
                var minutos = tiempo.Minutes;
                if (minutos > 0)
                {
                    return $"{horas} hora{(horas > 1 ? "s" : "")}, {minutos} minuto{(minutos > 1 ? "s" : "")}";
                }
                return $"{horas} hora{(horas > 1 ? "s" : "")}";
            }
            else if (tiempo.TotalMinutes >= 1)
            {
                var minutos = (int)tiempo.TotalMinutes;
                return $"{minutos} minuto{(minutos > 1 ? "s" : "")}";
            }
            else
            {
                return "Menos de 1 minuto";
            }
        }
    }
}

