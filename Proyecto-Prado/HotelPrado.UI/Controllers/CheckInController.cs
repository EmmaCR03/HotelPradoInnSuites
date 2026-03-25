using HotelPrado.AccesoADatos;
using HotelPrado.UI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class CheckInController : Controller
    {
        private readonly Contexto _contexto;

        public CheckInController()
        {
            _contexto = new Contexto();
        }

        // GET: CheckIn (caché 60 s para aligerar en host)
        [OutputCache(Duration = 60, VaryByParam = "pagina;tamanoPagina;busqueda", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index(int pagina = 1, int tamanoPagina = 50, string busqueda = null)
        {
            try
            {
                // Validar parámetros
                if (pagina < 1) pagina = 1;
                if (tamanoPagina < 10) tamanoPagina = 10;
                if (tamanoPagina > 200) tamanoPagina = 200;

                var checkIns = new List<CheckInViewModel>();
                int totalCheckIns = 0;

                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();

                    // Contar total de check-ins
                    string countSql = @"
                        SELECT COUNT(*)
                        FROM CheckIn ci
                        LEFT JOIN Habitaciones h ON ci.IdHabitacion = h.IdHabitacion
                        WHERE (@busqueda IS NULL OR @busqueda = '' 
                            OR ci.NombreCliente LIKE '%' + @busqueda + '%'
                            OR ci.CedulaCliente LIKE '%' + @busqueda + '%'
                            OR h.NumeroHabitacion LIKE '%' + @busqueda + '%')";

                    using (var countCommand = new SqlCommand(countSql, connection))
                    {
                        countCommand.Parameters.AddWithValue("@busqueda", (object)busqueda ?? DBNull.Value);
                        totalCheckIns = (int)countCommand.ExecuteScalar();
                    }

                    // Calcular paginación
                    int totalPaginas = (int)Math.Ceiling((double)totalCheckIns / tamanoPagina);
                    if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

                    int skip = (pagina - 1) * tamanoPagina;

                    // Obtener check-ins paginados
                    string sql = @"
                        SELECT 
                            ci.IdCheckIn,
                            ci.CodigoFolio,
                            ci.NombreCliente,
                            ci.CedulaCliente,
                            ci.FechaCheckIn,
                            ci.FechaCheckOut,
                            h.NumeroHabitacion,
                            ci.Estado
                        FROM CheckIn ci
                        LEFT JOIN Habitaciones h ON ci.IdHabitacion = h.IdHabitacion
                        WHERE (@busqueda IS NULL OR @busqueda = '' 
                            OR ci.NombreCliente LIKE '%' + @busqueda + '%'
                            OR ci.CedulaCliente LIKE '%' + @busqueda + '%'
                            OR h.NumeroHabitacion LIKE '%' + @busqueda + '%')
                        ORDER BY ci.IdCheckIn DESC
                        OFFSET @skip ROWS
                        FETCH NEXT @tamanoPagina ROWS ONLY";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@busqueda", (object)busqueda ?? DBNull.Value);
                        command.Parameters.AddWithValue("@skip", skip);
                        command.Parameters.AddWithValue("@tamanoPagina", tamanoPagina);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                checkIns.Add(new CheckInViewModel
                                {
                                    IdCheckIn = reader.GetInt32(0),
                                    CodigoFolio = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    NombreCliente = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    CedulaCliente = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    FechaCheckIn = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                    FechaCheckOut = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    NumeroHabitacion = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    Estado = reader.IsDBNull(7) ? "" : reader.GetString(7)
                                });
                            }
                        }
                    }
                }

                // Configurar ViewBag para paginación
                ViewBag.TotalCheckIns = totalCheckIns;
                ViewBag.PaginaActual = pagina;
                ViewBag.TamanoPagina = tamanoPagina;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalCheckIns / tamanoPagina);
                ViewBag.Busqueda = busqueda;

                return View(checkIns);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los check-ins: " + ex.Message;
                return View(new List<CheckInViewModel>());
            }
        }

        // GET: CheckIn/ImprimirTarjeta/5
        public ActionResult ImprimirTarjeta(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            try
            {
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                        SELECT 
                            ci.IdCheckIn,
                            ci.CodigoFolio,
                            ci.NombreCliente,
                            ci.CedulaCliente,
                            ci.TelefonoCliente,
                            ci.DireccionCliente,
                            ci.FechaCheckIn,
                            ci.FechaCheckOut,
                            ci.NumeroAdultos,
                            ci.NumeroNinos,
                            ci.Observaciones,
                            ci.Total,
                            ci.Deposito,
                            h.NumeroHabitacion,
                            e.NombreEmpresa,
                            c.EmailCliente
                        FROM CheckIn ci
                        LEFT JOIN Habitaciones h ON ci.IdHabitacion = h.IdHabitacion
                        LEFT JOIN Empresas e ON ci.IdEmpresa = e.IdEmpresa
                        LEFT JOIN Cliente c ON ci.IdCliente = c.IdCliente
                        WHERE ci.IdCheckIn = @IdCheckIn", connection))
                    {
                        command.Parameters.AddWithValue("@IdCheckIn", id.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var checkInData = new CheckInDetalleViewModel
                                {
                                    IdCheckIn = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                                    CodigoFolio = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    NombreCliente = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    CedulaCliente = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    TelefonoCliente = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    DireccionCliente = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    FechaCheckIn = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                    FechaCheckOut = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                    NumeroAdultos = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                    NumeroNinos = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                                    Observaciones = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Total = reader.IsDBNull(11) ? (decimal?)null : reader.GetDecimal(11),
                                    Deposito = reader.IsDBNull(12) ? (decimal?)null : reader.GetDecimal(12),
                                    NumeroHabitacion = reader.IsDBNull(13) ? "" : reader.GetString(13),
                                    NombreEmpresa = reader.IsDBNull(14) ? "" : reader.GetString(14),
                                    EmailCliente = reader.IsDBNull(15) ? "" : reader.GetString(15)
                                };

                                return View(checkInData);
                            }
                        }
                    }
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los datos del check-in: " + ex.Message;
                return View(new CheckInDetalleViewModel());
            }
        }

        // GET: CheckIn/VerTarjeta/5 - Vista para mostrar la tarjeta en pantalla y firmar
        public ActionResult VerTarjeta(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            try
            {
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                        SELECT 
                            ci.IdCheckIn,
                            ci.CodigoFolio,
                            ci.NombreCliente,
                            ci.CedulaCliente,
                            ci.TelefonoCliente,
                            ci.DireccionCliente,
                            ci.FechaCheckIn,
                            ci.FechaCheckOut,
                            ci.NumeroAdultos,
                            ci.NumeroNinos,
                            ci.Observaciones,
                            ci.Total,
                            ci.Deposito,
                            h.NumeroHabitacion,
                            e.NombreEmpresa,
                            c.EmailCliente
                        FROM CheckIn ci
                        LEFT JOIN Habitaciones h ON ci.IdHabitacion = h.IdHabitacion
                        LEFT JOIN Empresas e ON ci.IdEmpresa = e.IdEmpresa
                        LEFT JOIN Cliente c ON ci.IdCliente = c.IdCliente
                        WHERE ci.IdCheckIn = @IdCheckIn", connection))
                    {
                        command.Parameters.AddWithValue("@IdCheckIn", id.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var checkInData = new CheckInDetalleViewModel
                                {
                                    IdCheckIn = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                                    CodigoFolio = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                    NombreCliente = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    CedulaCliente = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    TelefonoCliente = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    DireccionCliente = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    FechaCheckIn = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                                    FechaCheckOut = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                    NumeroAdultos = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                    NumeroNinos = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                                    Observaciones = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                    Total = reader.IsDBNull(11) ? (decimal?)null : reader.GetDecimal(11),
                                    Deposito = reader.IsDBNull(12) ? (decimal?)null : reader.GetDecimal(12),
                                    NumeroHabitacion = reader.IsDBNull(13) ? "" : reader.GetString(13),
                                    NombreEmpresa = reader.IsDBNull(14) ? "" : reader.GetString(14),
                                    EmailCliente = reader.IsDBNull(15) ? "" : reader.GetString(15)
                                };

                                return View(checkInData);
                            }
                        }
                    }
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los datos del check-in: " + ex.Message;
                return View(new CheckInDetalleViewModel());
            }
        }

        // POST: CheckIn/GuardarFirma/5
        [HttpPost]
        public ActionResult GuardarFirma(int id, string firmaHuésped, string firmaRecepcionista)
        {
            try
            {
                // Aquí puedes guardar las firmas en la base de datos si lo necesitas
                // Por ahora solo retornamos éxito
                return Json(new { success = true, message = "Firma guardada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar la firma: " + ex.Message });
            }
        }

        // GET: CheckIn/Create - Crear check-in manualmente
        public ActionResult Create(int? idReserva = null)
        {
            var model = new CheckInCrearViewModel
            {
                FechaCheckIn = DateTime.Now,
                FechaCheckOut = DateTime.Now.AddDays(1),
                NumeroAdultos = 1,
                NumeroNinos = 0,
                Estado = "Check-In"
            };

            // Si viene desde una reserva, cargar datos de la reserva
            if (idReserva.HasValue)
            {
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                        SELECT 
                            r.IdReserva,
                            c.IdCliente,
                            r.IdHabitacion,
                            r.NombreCliente,
                            r.cantidadPersonas,
                            r.FechaInicio,
                            r.FechaFinal,
                            r.MontoTotal,
                            c.CedulaCliente,
                            c.TelefonoCliente,
                            c.DireccionCliente,
                            c.EmailCliente,
                            h.NumeroHabitacion
                        FROM Reservas r
                        LEFT JOIN Cliente c ON r.IdCliente = c.IdUsuario
                        LEFT JOIN Habitaciones h ON r.IdHabitacion = h.IdHabitacion
                        WHERE r.IdReserva = @IdReserva AND r.EstadoReserva = 'Confirmada'", connection))
                    {
                        command.Parameters.AddWithValue("@IdReserva", idReserva.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model.IdReserva = reader.GetInt32(0);
                                model.IdCliente = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                model.IdHabitacion = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                                model.NombreCliente = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                model.NumeroAdultos = reader.IsDBNull(4) ? 1 : reader.GetInt32(4);
                                model.FechaCheckIn = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);
                                model.FechaCheckOut = reader.IsDBNull(6) ? DateTime.Now.AddDays(1) : reader.GetDateTime(6);
                                model.Total = reader.IsDBNull(7) ? (decimal?)null : reader.GetDecimal(7);
                                model.CedulaCliente = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                model.TelefonoCliente = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                model.DireccionCliente = reader.IsDBNull(10) ? "" : reader.GetString(10);
                                model.EmailCliente = reader.IsDBNull(11) ? "" : reader.GetString(11);
                            }
                        }
                    }
                }
            }

            // Cargar listas para dropdowns
            CargarListasParaVista();

            return View(model);
        }

        // POST: CheckIn/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CheckInCrearViewModel model)
        {
            try
            {
                // Manejar el campo Total manualmente para aceptar decimales con punto o coma
                if (!string.IsNullOrEmpty(Request.Form["Total"]))
                {
                    var totalValue = Request.Form["Total"].ToString().Trim().Replace(",", ".");
                    if (decimal.TryParse(totalValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal totalDecimal))
                    {
                        model.Total = totalDecimal;
                        ModelState.Remove("Total");
                    }
                }
                // Manejar el campo Deposito
                if (!string.IsNullOrEmpty(Request.Form["Deposito"]))
                {
                    var depValue = Request.Form["Deposito"].ToString().Trim().Replace(",", ".");
                    if (decimal.TryParse(depValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal depDecimal))
                    {
                        model.Deposito = depDecimal;
                        ModelState.Remove("Deposito");
                    }
                }

                if (ModelState.IsValid)
                {
                    // Validar fechas
                    if (model.FechaCheckOut <= model.FechaCheckIn)
                    {
                        ModelState.AddModelError("FechaCheckOut", "La fecha de salida debe ser posterior a la fecha de entrada");
                        CargarListasParaVista();
                        return View(model);
                    }

                    // Si no hay cliente seleccionado pero sí nombre, registrar cliente al vuelo (sin usuario web)
                    if ((!model.IdCliente.HasValue || model.IdCliente.Value == 0) && !string.IsNullOrWhiteSpace(model.NombreCliente))
                    {
                        using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                        {
                            connection.Open();
                            object telefonoVal = DBNull.Value;
                            if (!string.IsNullOrWhiteSpace(model.TelefonoCliente))
                            {
                                if (int.TryParse(model.TelefonoCliente.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""), out int telNum))
                                    telefonoVal = telNum;
                                else
                                    telefonoVal = model.TelefonoCliente.Trim();
                            }
                            using (var cmdCliente = new SqlCommand(@"
                                INSERT INTO Cliente (NombreCliente, PrimerApellidoCliente, SegundoApellidoCliente, EmailCliente, CedulaCliente, TelefonoCliente, DireccionCliente, IdUsuario)
                                VALUES (@NombreCliente, NULL, NULL, @EmailCliente, @CedulaCliente, @TelefonoCliente, @DireccionCliente, NULL);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                            {
                                cmdCliente.Parameters.AddWithValue("@NombreCliente", (object)model.NombreCliente.Trim() ?? DBNull.Value);
                                cmdCliente.Parameters.AddWithValue("@EmailCliente", (object)model.EmailCliente?.Trim() ?? DBNull.Value);
                                cmdCliente.Parameters.AddWithValue("@CedulaCliente", (object)model.CedulaCliente?.Trim() ?? DBNull.Value);
                                cmdCliente.Parameters.AddWithValue("@TelefonoCliente", telefonoVal);
                                cmdCliente.Parameters.AddWithValue("@DireccionCliente", (object)model.DireccionCliente?.Trim() ?? DBNull.Value);
                                var nuevoIdCliente = cmdCliente.ExecuteScalar();
                                if (nuevoIdCliente != null && nuevoIdCliente != DBNull.Value)
                                    model.IdCliente = Convert.ToInt32(nuevoIdCliente);
                            }
                        }
                    }

                    // Generar código de folio si no existe
                    int? codigoFolio = null;
                    if (model.IdReserva.HasValue)
                    {
                        codigoFolio = model.IdReserva.Value;
                    }
                    else
                    {
                        // Generar nuevo folio
                        using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                        {
                            connection.Open();
                            using (var command = new SqlCommand("SELECT ISNULL(MAX(CodigoFolio), 0) + 1 FROM CheckIn WHERE CodigoFolio IS NOT NULL", connection))
                            {
                                var result = command.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                {
                                    codigoFolio = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // Insertar check-in
                    using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand(@"
                            INSERT INTO CheckIn (
                                IdReserva, IdCliente, IdHabitacion, IdEmpresa,
                                NombreCliente, CedulaCliente, TelefonoCliente, DireccionCliente,
                                FechaCheckIn, FechaCheckOut, NumeroAdultos, NumeroNinos,
                                Total, Deposito, Estado, Observaciones, CodigoFolio
                            )
                            VALUES (
                                @IdReserva, @IdCliente, @IdHabitacion, @IdEmpresa,
                                @NombreCliente, @CedulaCliente, @TelefonoCliente, @DireccionCliente,
                                @FechaCheckIn, @FechaCheckOut, @NumeroAdultos, @NumeroNinos,
                                @Total, @Deposito, @Estado, @Observaciones, @CodigoFolio
                            );
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                        {
                            command.Parameters.AddWithValue("@IdReserva", (object)model.IdReserva ?? DBNull.Value);
                            command.Parameters.AddWithValue("@IdCliente", (object)model.IdCliente ?? DBNull.Value);
                            command.Parameters.AddWithValue("@IdHabitacion", (object)model.IdHabitacion ?? DBNull.Value);
                            command.Parameters.AddWithValue("@IdEmpresa", (object)model.IdEmpresa ?? DBNull.Value);
                            command.Parameters.AddWithValue("@NombreCliente", (object)model.NombreCliente ?? DBNull.Value);
                            command.Parameters.AddWithValue("@CedulaCliente", (object)model.CedulaCliente ?? DBNull.Value);
                            command.Parameters.AddWithValue("@TelefonoCliente", (object)model.TelefonoCliente ?? DBNull.Value);
                            command.Parameters.AddWithValue("@DireccionCliente", (object)model.DireccionCliente ?? DBNull.Value);
                            command.Parameters.AddWithValue("@FechaCheckIn", model.FechaCheckIn);
                            command.Parameters.AddWithValue("@FechaCheckOut", model.FechaCheckOut);
                            command.Parameters.AddWithValue("@NumeroAdultos", model.NumeroAdultos);
                            command.Parameters.AddWithValue("@NumeroNinos", model.NumeroNinos);
                            command.Parameters.AddWithValue("@Total", (object)model.Total ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Deposito", (object)model.Deposito ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Estado", model.Estado ?? "Check-In");
                            command.Parameters.AddWithValue("@Observaciones", (object)model.Observaciones ?? DBNull.Value);
                            command.Parameters.AddWithValue("@CodigoFolio", (object)codigoFolio ?? DBNull.Value);

                            int nuevoIdCheckIn = (int)command.ExecuteScalar();

                            // Si viene de una reserva, actualizar estado de la reserva y habitación
                            if (model.IdReserva.HasValue)
                            {
                                using (var updateCommand = new SqlCommand(@"
                                    UPDATE Reservas SET EstadoReserva = 'En Proceso' WHERE IdReserva = @IdReserva;
                                    UPDATE Habitaciones SET Estado = 'Ocupado' WHERE IdHabitacion = @IdHabitacion;", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@IdReserva", model.IdReserva.Value);
                                    updateCommand.Parameters.AddWithValue("@IdHabitacion", model.IdHabitacion.Value);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }

                            TempData["SuccessMessage"] = $"Check-in creado exitosamente. ID: {nuevoIdCheckIn}";
                            TempData["CheckInId"] = nuevoIdCheckIn;
                            return RedirectToAction("VerTarjeta", new { id = nuevoIdCheckIn });
                        }
                    }
                }

                // Si llegamos aquí, hay errores de validación
                if (!ModelState.IsValid)
                {
                    var errorMessages = new System.Collections.Generic.List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            if (!string.IsNullOrEmpty(error.ErrorMessage))
                            {
                                errorMessages.Add(error.ErrorMessage);
                            }
                        }
                    }
                    if (errorMessages.Count > 0)
                    {
                        ViewBag.Error = "Por favor corrija los siguientes errores: " + string.Join("; ", errorMessages);
                    }
                    else
                    {
                        ViewBag.Error = "No se pudo crear el check-in. Por favor verifique los datos e intente nuevamente.";
                    }
                }
                else
                {
                    ViewBag.Error = "No se pudo crear el check-in. Por favor verifique los datos e intente nuevamente.";
                }
                CargarListasParaVista();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al crear el check-in: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Error al crear check-in: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                CargarListasParaVista();
                return View(model);
            }
        }

        // Método auxiliar para cargar listas (una sola conexión para las 3 consultas = menos latencia)
        private void CargarListasParaVista()
        {
            var habitaciones = new List<System.Web.Mvc.SelectListItem>();
            var reservas = new List<System.Web.Mvc.SelectListItem> { new System.Web.Mvc.SelectListItem { Value = "", Text = "Ninguna (Check-in manual)" } };
            var empresas = new List<System.Web.Mvc.SelectListItem> { new System.Web.Mvc.SelectListItem { Value = "", Text = "Ninguna" } };

            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();

                using (var cmd = new SqlCommand("SELECT IdHabitacion, NumeroHabitacion, Estado FROM Habitaciones WHERE Estado IN ('Disponible', 'Ocupado') ORDER BY NumeroHabitacion", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        habitaciones.Add(new System.Web.Mvc.SelectListItem { Value = reader.GetInt32(0).ToString(), Text = reader.GetString(1) + " (" + reader.GetString(2) + ")" });
                }

                using (var cmd = new SqlCommand(@"
                    SELECT r.IdReserva, r.NombreCliente, h.NumeroHabitacion, r.FechaInicio
                    FROM Reservas r
                    LEFT JOIN Habitaciones h ON r.IdHabitacion = h.IdHabitacion
                    WHERE r.EstadoReserva = 'Confirmada' AND NOT EXISTS (SELECT 1 FROM CheckIn WHERE IdReserva = r.IdReserva)
                    ORDER BY r.FechaInicio DESC", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        reservas.Add(new System.Web.Mvc.SelectListItem { Value = reader.GetInt32(0).ToString(), Text = $"Reserva #{reader.GetInt32(0)} - {(reader.IsDBNull(1) ? "" : reader.GetString(1))} - Hab. {(reader.IsDBNull(2) ? "" : reader.GetString(2))}" });
                }

                using (var cmd = new SqlCommand("SELECT IdEmpresa, NombreEmpresa FROM Empresas ORDER BY NombreEmpresa", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        empresas.Add(new System.Web.Mvc.SelectListItem { Value = reader.GetInt32(0).ToString(), Text = reader.IsDBNull(1) ? "" : reader.GetString(1) });
                }
            }

            ViewBag.Habitaciones = new System.Web.Mvc.SelectList(habitaciones, "Value", "Text");
            ViewBag.Reservas = new System.Web.Mvc.SelectList(reservas, "Value", "Text");
            ViewBag.Empresas = new System.Web.Mvc.SelectList(empresas, "Value", "Text");
        }

        // GET: CheckIn/BuscarEmpresas - AJAX para buscar empresas
        [HttpGet]
        public JsonResult BuscarEmpresas(string busqueda = "")
        {
            try
            {
                var empresas = new List<object>();
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT TOP 20 IdEmpresa, NombreEmpresa, Telefono1, CorreoElectronico, Direccion
                        FROM Empresas
                        WHERE (@busqueda IS NULL OR @busqueda = '' 
                            OR NombreEmpresa LIKE '%' + @busqueda + '%'
                            OR CorreoElectronico LIKE '%' + @busqueda + '%')
                        ORDER BY NombreEmpresa";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@busqueda", (object)busqueda ?? DBNull.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                empresas.Add(new
                                {
                                    IdEmpresa = reader.GetInt32(0),
                                    NombreEmpresa = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Telefono = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Direccion = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
                return Json(empresas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: CheckIn/CrearEmpresa - Crear empresa desde modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CrearEmpresa(string NombreEmpresa, string Telefono1, string CorreoElectronico, string Direccion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NombreEmpresa))
                {
                    return Json(new { success = false, message = "El nombre de la empresa es requerido" });
                }

                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                        INSERT INTO Empresas (NombreEmpresa, Telefono1, CorreoElectronico, Direccion)
                        VALUES (@NombreEmpresa, @Telefono1, @CorreoElectronico, @Direccion);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                    {
                        command.Parameters.AddWithValue("@NombreEmpresa", NombreEmpresa);
                        command.Parameters.AddWithValue("@Telefono1", (object)Telefono1 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CorreoElectronico", (object)CorreoElectronico ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", (object)Direccion ?? DBNull.Value);
                        
                        int nuevoId = (int)command.ExecuteScalar();
                        
                        return Json(new { 
                            success = true, 
                            message = "Empresa creada exitosamente",
                            empresa = new {
                                IdEmpresa = nuevoId,
                                NombreEmpresa = NombreEmpresa
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la empresa: " + ex.Message });
            }
        }

        // POST: CheckIn/EditarEmpresa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditarEmpresa(int IdEmpresa, string NombreEmpresa, string Telefono1, string CorreoElectronico, string Direccion)
        {
            try
            {
                if (IdEmpresa <= 0) return Json(new { success = false, message = "Id de empresa no válido" });
                if (string.IsNullOrWhiteSpace(NombreEmpresa)) return Json(new { success = false, message = "El nombre de la empresa es requerido" });

                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                        UPDATE Empresas SET NombreEmpresa = @NombreEmpresa, Telefono1 = @Telefono1, CorreoElectronico = @CorreoElectronico, Direccion = @Direccion
                        WHERE IdEmpresa = @IdEmpresa", connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpresa", IdEmpresa);
                        command.Parameters.AddWithValue("@NombreEmpresa", NombreEmpresa);
                        command.Parameters.AddWithValue("@Telefono1", (object)Telefono1 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CorreoElectronico", (object)CorreoElectronico ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", (object)Direccion ?? DBNull.Value);
                        int rows = command.ExecuteNonQuery();
                        if (rows == 0) return Json(new { success = false, message = "Empresa no encontrada" });
                    }
                }
                return Json(new { success = true, message = "Empresa actualizada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: CheckIn/EliminarEmpresa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EliminarEmpresa(int IdEmpresa)
        {
            try
            {
                if (IdEmpresa <= 0) return Json(new { success = false, message = "Id de empresa no válido" });

                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    int enUso;
                    using (var cmd = new SqlCommand("SELECT COUNT(1) FROM CheckIn WHERE IdEmpresa = @IdEmpresa", connection))
                    {
                        cmd.Parameters.AddWithValue("@IdEmpresa", IdEmpresa);
                        enUso = (int)cmd.ExecuteScalar();
                    }
                    if (enUso > 0) return Json(new { success = false, message = "No se puede eliminar: la empresa está asociada a uno o más check-ins." });

                    using (var command = new SqlCommand("DELETE FROM Empresas WHERE IdEmpresa = @IdEmpresa", connection))
                    {
                        command.Parameters.AddWithValue("@IdEmpresa", IdEmpresa);
                        int rows = command.ExecuteNonQuery();
                        if (rows == 0) return Json(new { success = false, message = "Empresa no encontrada" });
                    }
                }
                return Json(new { success = true, message = "Empresa eliminada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: CheckIn/ObtenerReservaConCliente - Devuelve reserva + datos del cliente (AspNetUsers/Cliente) para auto-llenar formulario
        [HttpGet]
        public JsonResult ObtenerReservaConCliente(int IdReserva)
        {
            try
            {
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT 
                            r.IdReserva, r.IdHabitacion, r.NombreCliente, r.cantidadPersonas,
                            r.FechaInicio, r.FechaFinal, r.MontoTotal,
                            h.NumeroHabitacion,
                            u.NombreCompleto AS ClienteNombre, u.cedula AS ClienteCedula, u.Email AS ClienteEmail,
                            ISNULL(u.Telefono, u.PhoneNumber) AS ClienteTelefono,
                            c.DireccionCliente
                        FROM Reservas r
                        LEFT JOIN Habitaciones h ON r.IdHabitacion = h.IdHabitacion
                        LEFT JOIN AspNetUsers u ON r.IdCliente = u.Id
                        LEFT JOIN Cliente c ON c.IdUsuario = r.IdCliente
                        WHERE r.IdReserva = @IdReserva";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@IdReserva", IdReserva);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return Json(new { success = false, message = "Reserva no encontrada" }, JsonRequestBehavior.AllowGet);

                            var nombreCliente = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            var clienteNombre = reader.IsDBNull(8) ? null : reader.GetString(8);
                            var clienteCedula = reader.IsDBNull(9) ? null : reader.GetString(9);
                            var clienteEmail = reader.IsDBNull(10) ? null : reader.GetString(10);
                            var clienteTelefono = reader.IsDBNull(11) ? null : reader.GetString(11);
                            var direccionCliente = reader.IsDBNull(12) ? null : reader.GetString(12);

                            var reserva = new
                            {
                                IdReserva = reader.GetInt32(0),
                                IdHabitacion = reader.GetInt32(1),
                                NombreCliente = !string.IsNullOrEmpty(clienteNombre) ? clienteNombre : nombreCliente,
                                CantidadPersonas = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
                                FechaInicio = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                FechaFinal = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                MontoTotal = reader.IsDBNull(6) ? 0m : reader.GetDecimal(6),
                                NumeroHabitacion = reader.IsDBNull(7) ? "" : reader.GetString(7)
                            };
                            var cliente = new
                            {
                                NombreCompleto = reserva.NombreCliente,
                                Cedula = clienteCedula ?? "",
                                Email = clienteEmail ?? "",
                                Telefono = clienteTelefono ?? "",
                                Direccion = direccionCliente ?? ""
                            };
                            return Json(new { success = true, reserva = reserva, cliente = cliente }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CheckIn/BuscarClientes - AJAX para buscar clientes (AspNetUsers) y poder rellenar el formulario
        [HttpGet]
        public JsonResult BuscarClientes(string busqueda = "")
        {
            try
            {
                var list = new List<object>();
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT TOP 20 u.Id, u.NombreCompleto, u.cedula, u.Email, ISNULL(u.Telefono, u.PhoneNumber) AS Telefono, c.DireccionCliente
                        FROM AspNetUsers u
                        LEFT JOIN Cliente c ON c.IdUsuario = u.Id
                        WHERE (@busqueda IS NULL OR @busqueda = ''
                            OR u.NombreCompleto LIKE '%' + @busqueda + '%'
                            OR u.cedula LIKE '%' + @busqueda + '%'
                            OR u.Email LIKE '%' + @busqueda + '%'
                            OR u.UserName LIKE '%' + @busqueda + '%')
                        ORDER BY u.NombreCompleto";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@busqueda", (object)busqueda ?? DBNull.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new
                                {
                                    Id = reader.IsDBNull(0) ? "" : reader.GetString(0),
                                    NombreCompleto = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Cedula = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Direccion = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                });
                            }
                        }
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CheckIn/BuscarReservas - AJAX para buscar reservas
        [HttpGet]
        public JsonResult BuscarReservas(string busqueda = "")
        {
            try
            {
                var reservas = new List<object>();
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT TOP 20 
                            r.IdReserva,
                            r.IdHabitacion,
                            r.NombreCliente,
                            r.cantidadPersonas,
                            r.FechaInicio,
                            r.FechaFinal,
                            r.MontoTotal,
                            h.NumeroHabitacion,
                            r.EstadoReserva
                        FROM Reservas r
                        LEFT JOIN Habitaciones h ON r.IdHabitacion = h.IdHabitacion
                        WHERE r.EstadoReserva = 'Confirmada'
                            AND NOT EXISTS (SELECT 1 FROM CheckIn WHERE IdReserva = r.IdReserva)
                            AND (@busqueda IS NULL OR @busqueda = '' 
                                OR r.NombreCliente LIKE '%' + @busqueda + '%'
                                OR CAST(r.IdReserva AS VARCHAR) LIKE '%' + @busqueda + '%'
                                OR h.NumeroHabitacion LIKE '%' + @busqueda + '%')
                        ORDER BY r.FechaInicio DESC";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@busqueda", (object)busqueda ?? DBNull.Value);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reservas.Add(new
                                {
                                    IdReserva = reader.GetInt32(0),
                                    IdHabitacion = reader.GetInt32(1),
                                    NombreCliente = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    CantidadPersonas = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
                                    FechaInicio = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                    FechaFinal = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    MontoTotal = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
                                    NumeroHabitacion = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    EstadoReserva = reader.IsDBNull(8) ? "" : reader.GetString(8)
                                });
                            }
                        }
                    }
                }
                return Json(reservas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: CheckIn/CrearReserva - Crear reserva desde modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CrearReserva(int IdHabitacion, string NombreCliente, int CantidadPersonas, DateTime FechaInicio, DateTime FechaFinal, decimal MontoTotal)
        {
            try
            {
                if (FechaFinal <= FechaInicio)
                {
                    return Json(new { success = false, message = "La fecha de salida debe ser posterior a la fecha de entrada" });
                }

                if (CantidadPersonas < 1)
                {
                    return Json(new { success = false, message = "Debe haber al menos 1 persona" });
                }

                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    // Verificar disponibilidad
                    string checkDisponibilidad = @"
                        SELECT COUNT(*) 
                        FROM Reservas 
                        WHERE IdHabitacion = @IdHabitacion
                            AND EstadoReserva IN ('Confirmada', 'Solicitada')
                            AND (
                                (@FechaInicio >= FechaInicio AND @FechaInicio < FechaFinal) OR
                                (@FechaFinal > FechaInicio AND @FechaFinal <= FechaFinal) OR
                                (@FechaInicio <= FechaInicio AND @FechaFinal >= FechaFinal)
                            )";
                    
                    using (var checkCommand = new SqlCommand(checkDisponibilidad, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                        checkCommand.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                        checkCommand.Parameters.AddWithValue("@FechaFinal", FechaFinal);
                        
                        int reservasExistentes = (int)checkCommand.ExecuteScalar();
                        if (reservasExistentes > 0)
                        {
                            return Json(new { success = false, message = "La habitación no está disponible en esas fechas" });
                        }
                    }
                    
                    // Crear reserva
                    using (var command = new SqlCommand(@"
                        INSERT INTO Reservas (
                            IdCliente, NombreCliente, cantidadPersonas, IdHabitacion,
                            FechaInicio, FechaFinal, EstadoReserva, MontoTotal
                        )
                        VALUES (
                            '', @NombreCliente, @CantidadPersonas, @IdHabitacion,
                            @FechaInicio, @FechaFinal, 'Confirmada', @MontoTotal
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                    {
                        command.Parameters.AddWithValue("@NombreCliente", NombreCliente);
                        command.Parameters.AddWithValue("@CantidadPersonas", CantidadPersonas);
                        command.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                        command.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                        command.Parameters.AddWithValue("@FechaFinal", FechaFinal);
                        command.Parameters.AddWithValue("@MontoTotal", MontoTotal);
                        
                        int nuevoId = (int)command.ExecuteScalar();
                        
                        return Json(new { 
                            success = true, 
                            message = "Reserva creada exitosamente",
                            reserva = new {
                                IdReserva = nuevoId,
                                IdHabitacion = IdHabitacion,
                                NombreCliente = NombreCliente,
                                FechaInicio = FechaInicio.ToString("yyyy-MM-ddTHH:mm"),
                                FechaFinal = FechaFinal.ToString("yyyy-MM-ddTHH:mm")
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la reserva: " + ex.Message });
            }
        }
    }
}

