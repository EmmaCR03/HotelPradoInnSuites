using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.LN.Citas.Enlaces;
using HotelPrado.LN.Reservas;
using HotelPrado.UI.Helpers;
using HotelPrado.UI.Models;
using HotelPrado.UI.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    /// <summary>
    /// Controlador para gestionar las reservas de los usuarios
    /// Sigue principios SOLID: Responsabilidad Única, Inversión de Dependencias
    /// </summary>
    public class ReservasController : Controller
    {
        private readonly IReservasLN _reservasLN;
        private readonly IObtenerEnlacesLN _obtenerEnlaces;
        private readonly ReservaService _reservaService;

        public ReservasController()
        {
            _reservasLN = new ReservasLN();
            _obtenerEnlaces = new ObtenerEnlacesLN();
            _reservaService = new ReservaService(_reservasLN);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ConfirmarReserva(int id, DateTime checkIn, DateTime checkOut, decimal totalPrecio, int cantidadPersonas)
        {
            // Asegurar Session["UserId"] antes del POST (en algunos entornos la cookie no trae el Id)
            if (Session != null && Session["UserId"] == null && User?.Identity?.IsAuthenticated == true)
            {
                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext);
                if (!string.IsNullOrEmpty(userId))
                    Session["UserId"] = userId;
            }

            var model = new ReservasDTO
            {
                IdHabitacion = id,
                FechaInicio = checkIn,
                FechaFinal = checkOut,
                cantidadPersonas = cantidadPersonas,
                MontoTotal = totalPrecio
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarReserva(ReservasDTO model)
        {
            try
            {
                // Validar que el modelo tenga los datos necesarios
                if (model == null)
                {
                    ModelState.AddModelError("", "Los datos de la reserva no son válidos.");
                    return View(new ReservasDTO());
                }

                // Validar campos requeridos manualmente si ModelState no es válido
                if (!ModelState.IsValid)
                {
                    // Mostrar errores de validación
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error de validación: {error.ErrorMessage}");
                    }
                }

                // Obtener IdUsuario (Identity, Session o email en host cuando la cookie no se lee)
                string idUsuario = UsuarioActualHelper.ObtenerId(this.HttpContext);
                
                if (string.IsNullOrEmpty(idUsuario))
                {
                    ModelState.AddModelError("", "No se pudo identificar al usuario. Por favor, cierre sesión, vuelva a iniciar sesión e intente de nuevo.");
                    return View(model);
                }

                // VERIFICAR que el usuario existe en AspNetUsers antes de continuar
                bool usuarioExiste = false;
                try
                {
                    using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                    {
                        using (var connection = new System.Data.SqlClient.SqlConnection(contexto.Database.Connection.ConnectionString))
                        {
                            connection.Open();
                            using (var command = new System.Data.SqlClient.SqlCommand(
                                "SELECT COUNT(*) FROM AspNetUsers WHERE Id = @IdUsuario", connection))
                            {
                                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                                var count = (int)command.ExecuteScalar();
                                usuarioExiste = count > 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al verificar usuario: {ex.Message}");
                    ModelState.AddModelError("", "Error al verificar el usuario. Por favor, inténtelo de nuevo.");
                    return View(model);
                }

                if (!usuarioExiste)
                {
                    ModelState.AddModelError("", "El usuario no existe en el sistema. Por favor, contacte al administrador.");
                    return View(model);
                }

                // Obtener IdCliente (INT) desde Cliente usando IdUsuario
                int? idCliente = null;
                bool esAdmin = User.IsInRole("Administrador") || User.IsInRole("Colaborador");
                
                try
                {
                    using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                    {
                        using (var connection = new System.Data.SqlClient.SqlConnection(contexto.Database.Connection.ConnectionString))
                        {
                            connection.Open();
                            using (var command = new System.Data.SqlClient.SqlCommand(
                                "SELECT IdCliente FROM Cliente WHERE IdUsuario = @IdUsuario", connection))
                            {
                                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                                var result = command.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                {
                                    idCliente = Convert.ToInt32(result);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al buscar cliente: {ex.Message}");
                    ModelState.AddModelError("", "Error al buscar información del cliente. Por favor, inténtelo de nuevo.");
                    return View(model);
                }

                // Si no hay cliente vinculado y no es admin, mostrar error
                if (!idCliente.HasValue && !esAdmin)
                {
                    ModelState.AddModelError("", "No se encontró un cliente vinculado a tu cuenta. Por favor, contacta al administrador.");
                    return View(model);
                }

                // Si es admin y no tiene cliente vinculado, crear uno automáticamente o usar un cliente genérico
                if (!idCliente.HasValue && esAdmin)
                {
                    try
                    {
                        using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                        {
                            using (var connection = new System.Data.SqlClient.SqlConnection(contexto.Database.Connection.ConnectionString))
                            {
                                connection.Open();
                                
                                // Intentar crear un cliente para el admin o usar uno existente
                                string nombreAdmin = User.Identity.GetUserName() ?? "Administrador";
                                string emailAdmin = User.Identity.Name ?? "";
                                
                                // Buscar si ya existe un cliente con este email
                                using (var command = new System.Data.SqlClient.SqlCommand(
                                    "SELECT IdCliente FROM Cliente WHERE EmailCliente = @Email", connection))
                                {
                                    command.Parameters.AddWithValue("@Email", emailAdmin);
                                    var result = command.ExecuteScalar();
                                    if (result != null && result != DBNull.Value)
                                    {
                                        idCliente = Convert.ToInt32(result);
                                        // Vincular el cliente con el usuario admin
                                        using (var updateCommand = new System.Data.SqlClient.SqlCommand(
                                            "UPDATE Cliente SET IdUsuario = @IdUsuario WHERE IdCliente = @IdCliente", connection))
                                        {
                                            updateCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                                            updateCommand.Parameters.AddWithValue("@IdCliente", idCliente.Value);
                                            updateCommand.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Crear nuevo cliente para el admin
                                        using (var insertCommand = new System.Data.SqlClient.SqlCommand(
                                            "INSERT INTO Cliente (NombreCliente, EmailCliente, IdUsuario) OUTPUT INSERTED.IdCliente VALUES (@Nombre, @Email, @IdUsuario)", connection))
                                        {
                                            insertCommand.Parameters.AddWithValue("@Nombre", nombreAdmin);
                                            insertCommand.Parameters.AddWithValue("@Email", emailAdmin);
                                            insertCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                                            var newId = insertCommand.ExecuteScalar();
                                            if (newId != null && newId != DBNull.Value)
                                            {
                                                idCliente = Convert.ToInt32(newId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al crear/vincular cliente para admin: {ex.Message}");
                        // Si falla, usar un cliente genérico o permitir continuar sin cliente
                        ModelState.AddModelError("", "Error al vincular cuenta de administrador. Por favor, inténtelo de nuevo.");
                        return View(model);
                    }
                }

                // IMPORTANTE: IdCliente en Reservas es NVARCHAR(128) que referencia a AspNetUsers.Id (GUID)
                // NO usar el INT de la tabla Cliente, sino el GUID del usuario
                model.IdCliente = idUsuario; // Usar el GUID directamente, no el INT de Cliente
                model.NombreCliente = User.Identity.GetUserName() ?? "Usuario";

                System.Diagnostics.Debug.WriteLine($"Intentando crear reserva - IdCliente (GUID): {model.IdCliente}, IdHabitacion: {model.IdHabitacion}");
                System.Diagnostics.Debug.WriteLine($"IdCliente INT (solo para referencia): {idCliente?.ToString() ?? "N/A"}");

                // Verificar disponibilidad y buscar habitación alternativa si es necesario
                int idHabitacionFinal = await BuscarHabitacionDisponible(model.IdHabitacion, model.FechaInicio.Value, model.FechaFinal.Value, model.cantidadPersonas);
                
                if (idHabitacionFinal > 0)
                {
                    // Si hay habitación disponible, asignarla y poner estado "Solicitada"
                    model.IdHabitacion = idHabitacionFinal;
                    model.EstadoReserva = "Solicitada";
                    System.Diagnostics.Debug.WriteLine($"Habitación asignada automáticamente: {idHabitacionFinal}");
                }
                else
                {
                    // Si NO hay habitación disponible, SIEMPRE permitir crear la reserva pero en "En Lista de Espera"
                    model.EstadoReserva = "En Lista de Espera";
                    System.Diagnostics.Debug.WriteLine($"No se encontró habitación disponible, se creará la reserva automáticamente en lista de espera");
                }

                int resultado = await _reservasLN.CrearReservasUsuario(model);
                
                if (resultado > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Reserva creada exitosamente. Resultado: {resultado}");
                    return RedirectToAction("Confirmacion");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No se pudo crear la reserva. Resultado: {resultado}");
                    ModelState.AddModelError("", "No se pudo crear la reserva. Inténtelo de nuevo.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al confirmar reserva: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Error al procesar la reserva: {ex.Message}");
            }
            
            return View(model);
        }

        public ActionResult Confirmacion()
        {
            return View();
        }

        /// <summary>
        /// Muestra las reservas del usuario autenticado (Identity, Session o email en host).
        /// </summary>
        [Authorize]
        [HttpGet]
        public ActionResult ReservasUsuario()
        {
            ViewBag.Title = "Mis Reservas";

            // Asegurar Session antes de usar (mismo problema que en ConfirmarReserva en algunos entornos)
            if (Session != null && Session["UserId"] == null && User?.Identity?.IsAuthenticated == true)
            {
                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext);
                if (!string.IsNullOrEmpty(userId))
                    Session["UserId"] = userId;
            }

            string idCliente = UsuarioActualHelper.ObtenerId(this.HttpContext);
            
            if (string.IsNullOrEmpty(idCliente))
            {
                return RedirectToAction("Login", "Account");
            }

            // Usar el servicio para obtener reservas con configuración (SRP)
            var reservas = _reservaService.ObtenerReservasConConfiguracion(idCliente);

            // Usar ViewModel para separar lógica de presentación (Clean Code)
            var viewModel = new ReservasUsuarioViewModel
            {
                Reservas = reservas
            };

            return View(viewModel);
        }

        /// <summary>
        /// Busca una habitación disponible. Si la habitación solicitada no está disponible,
        /// busca otra habitación con la misma capacidad que esté disponible.
        /// </summary>
        private async Task<int> BuscarHabitacionDisponible(int idHabitacionSolicitada, DateTime fechaInicio, DateTime fechaFinal, int cantidadPersonas)
        {
            try
            {
                using (var contexto = new HotelPrado.AccesoADatos.Contexto())
                {
                    using (var connection = new System.Data.SqlClient.SqlConnection(contexto.Database.Connection.ConnectionString))
                    {
                        await connection.OpenAsync();
                        
                        // Primero, verificar si la habitación solicitada está disponible
                        string checkDisponibilidadQuery = @"
                            SELECT COUNT(*) 
                            FROM Reservas 
                            WHERE IdHabitacion = @IdHabitacion
                              AND EstadoReserva IN ('Confirmada', 'Solicitada')
                              AND FechaInicio IS NOT NULL
                              AND FechaFinal IS NOT NULL
                              AND (
                                  (@FechaInicio >= FechaInicio AND @FechaInicio < FechaFinal) OR
                                  (@FechaFinal > FechaInicio AND @FechaFinal <= FechaFinal) OR
                                  (@FechaInicio <= FechaInicio AND @FechaFinal >= FechaFinal)
                              )";
                        
                        using (var checkCommand = new System.Data.SqlClient.SqlCommand(checkDisponibilidadQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@IdHabitacion", idHabitacionSolicitada);
                            checkCommand.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                            checkCommand.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                            
                            int reservasExistentes = (int)await checkCommand.ExecuteScalarAsync();
                            
                            if (reservasExistentes == 0)
                            {
                                // La habitación solicitada está disponible
                                System.Diagnostics.Debug.WriteLine($"Habitación {idHabitacionSolicitada} está disponible");
                                return idHabitacionSolicitada;
                            }
                        }
                        
                        // Si la habitación solicitada no está disponible, buscar otra con la misma capacidad
                        System.Diagnostics.Debug.WriteLine($"Habitación {idHabitacionSolicitada} no está disponible, buscando alternativa...");
                        
                        // Obtener la capacidad de la habitación solicitada
                        string obtenerCapacidadQuery = @"
                            SELECT CapacidadMin, CapacidadMax 
                            FROM Habitaciones 
                            WHERE IdHabitacion = @IdHabitacion";
                        
                        int? capacidadMin = null;
                        int? capacidadMax = null;
                        
                        using (var capacidadCommand = new System.Data.SqlClient.SqlCommand(obtenerCapacidadQuery, connection))
                        {
                            capacidadCommand.Parameters.AddWithValue("@IdHabitacion", idHabitacionSolicitada);
                            
                            using (var reader = await capacidadCommand.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    capacidadMin = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    capacidadMax = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                }
                            }
                        }
                        
                        if (!capacidadMin.HasValue || !capacidadMax.HasValue)
                        {
                            // Si no se puede obtener la capacidad, usar la cantidad de personas
                            capacidadMin = cantidadPersonas;
                            capacidadMax = cantidadPersonas;
                        }
                        
                        // Buscar habitaciones disponibles con la misma capacidad
                        string buscarAlternativaQuery = @"
                            SELECT TOP 1 h.IdHabitacion
                            FROM Habitaciones h
                            WHERE h.CapacidadMin <= @CantidadPersonas
                              AND h.CapacidadMax >= @CantidadPersonas
                              AND h.IdHabitacion != @IdHabitacionSolicitada
                              AND NOT EXISTS (
                                  SELECT 1 
                                  FROM Reservas r
                                  WHERE r.IdHabitacion = h.IdHabitacion
                                    AND r.EstadoReserva IN ('Confirmada', 'Solicitada')
                                    AND r.FechaInicio IS NOT NULL
                                    AND r.FechaFinal IS NOT NULL
                                    AND (
                                        (@FechaInicio >= r.FechaInicio AND @FechaInicio < r.FechaFinal) OR
                                        (@FechaFinal > r.FechaInicio AND @FechaFinal <= r.FechaFinal) OR
                                        (@FechaInicio <= r.FechaInicio AND @FechaFinal >= r.FechaFinal)
                                    )
                              )
                            ORDER BY h.IdHabitacion";
                        
                        using (var buscarCommand = new System.Data.SqlClient.SqlCommand(buscarAlternativaQuery, connection))
                        {
                            buscarCommand.Parameters.AddWithValue("@CantidadPersonas", cantidadPersonas);
                            buscarCommand.Parameters.AddWithValue("@IdHabitacionSolicitada", idHabitacionSolicitada);
                            buscarCommand.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                            buscarCommand.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                            
                            var resultado = await buscarCommand.ExecuteScalarAsync();
                            
                            if (resultado != null && resultado != DBNull.Value)
                            {
                                int idHabitacionAlternativa = Convert.ToInt32(resultado);
                                System.Diagnostics.Debug.WriteLine($"Habitación alternativa encontrada: {idHabitacionAlternativa}");
                                return idHabitacionAlternativa;
                            }
                        }
                        
                        // Si no se encuentra ninguna habitación disponible, devolver 0 para que se cree en lista de espera
                        System.Diagnostics.Debug.WriteLine("No se encontró habitación alternativa disponible");
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar habitación disponible: {ex.Message}");
                // En caso de error, devolver la habitación solicitada original
                return idHabitacionSolicitada;
            }
        }

    }
}