using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.SolicitudLimpieza.Registrar;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.SolicitudLimpieza.Listar;
using HotelPrado.LN.SolicitudLimpieza.Registrar;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class SolicitudLimpiezaController : Controller
    {
        private readonly IListarSolicitudLimpiezaLN _listarSolicitudLimpiezaLN;
        private readonly IRegistrarSolicitudLimpiezaLN _registrarSolicitudLimpiezaLN;
        private readonly Contexto _contexto;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public SolicitudLimpiezaController()
        {
            _registrarSolicitudLimpiezaLN = new RegistrarSolicitudLimpiezaLN();
            _listarSolicitudLimpiezaLN = new ListarSolicitudLimpiezaLN();
            _contexto = new Contexto();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: SolicitudLimpieza (caché 60 s para aligerar en host)
        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult IndexSolicitudLimpieza()
        {
            ViewBag.Title = "Solicitudes de Limpieza";
            var laListaDeSolicitudesLimpieza = _listarSolicitudLimpiezaLN.Listar();
            return View(laListaDeSolicitudesLimpieza);
        }

        // GET: SolicitudLimpieza/Create
        public ActionResult Create(int? IdDepartamento, int? IdHabitacion)
        {
            // Obtener todos los departamentos disponibles
            var departamentosQuery = _contexto.DepartamentoTabla
                .Where(d => d.Estado == "Disponible");

            var departamentos = departamentosQuery
                .ToList()
                .Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                })
                .ToList();

            ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

            // Obtener todas las habitaciones disponibles
            var habitacionesQuery = _contexto.HabitacionesTabla
                .Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");

            var habitaciones = habitacionesQuery
                .ToList()
                .Select(h => new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                })
                .ToList();

            ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");

            // Inicializar un nuevo modelo para la vista
            var nuevoModelo = new SolicitudLimpiezaDTO
            {
                Estado = "Pendiente",
                idDepartamento = 0,
                idHabitacion = 0,
                FechaSolicitud = DateTime.Now
            };

            if (IdDepartamento.HasValue && IdDepartamento.Value > 0)
            {
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento.Value);
                if (departamento != null)
                {
                    nuevoModelo.idDepartamento = IdDepartamento.Value;
                    nuevoModelo.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                    nuevoModelo.Estado = "Pendiente";
                    ViewBag.IdDepartamento = IdDepartamento.Value;
                    ViewBag.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                    ViewBag.TipoSeleccionado = "Departamento";
                }
            }
            else if (IdHabitacion.HasValue && IdHabitacion.Value > 0)
            {
                var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == IdHabitacion.Value);
                if (habitacion != null)
                {
                    nuevoModelo.idHabitacion = IdHabitacion.Value;
                    nuevoModelo.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                    nuevoModelo.Estado = "Pendiente";
                    ViewBag.IdHabitacion = IdHabitacion.Value;
                    ViewBag.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                    ViewBag.TipoSeleccionado = "Habitacion";
                }
            }

            return View(nuevoModelo);
        }

        // POST: SolicitudLimpieza/Create
        [HttpPost]
        public async Task<ActionResult> Create(SolicitudLimpiezaDTO modeloDeSolicitudLimpieza)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recargar las listas si el modelo no es válido
                    var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                    var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                    }).ToList();
                    ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeSolicitudLimpieza);
                }

                // Validar que se haya seleccionado al menos un departamento o habitación
                if (modeloDeSolicitudLimpieza.idDepartamento == 0 && modeloDeSolicitudLimpieza.idHabitacion == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un departamento o una habitación.");
                    // Recargar listas
                    var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                    var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                    }).ToList();
                    ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeSolicitudLimpieza);
                }

                // Asegurar que el estado esté establecido
                if (string.IsNullOrEmpty(modeloDeSolicitudLimpieza.Estado))
                {
                    modeloDeSolicitudLimpieza.Estado = "Pendiente";
                }

                // Obtener el nombre del departamento o habitación desde la base de datos
                if (modeloDeSolicitudLimpieza.idDepartamento > 0)
                {
                    var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == modeloDeSolicitudLimpieza.idDepartamento);
                    if (departamento != null)
                    {
                        modeloDeSolicitudLimpieza.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                        modeloDeSolicitudLimpieza.idHabitacion = 0;
                    }
                }
                else if (modeloDeSolicitudLimpieza.idHabitacion > 0)
                {
                    var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == modeloDeSolicitudLimpieza.idHabitacion);
                    if (habitacion != null)
                    {
                        modeloDeSolicitudLimpieza.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                        modeloDeSolicitudLimpieza.idDepartamento = 0;
                    }
                }

                if (modeloDeSolicitudLimpieza.FechaSolicitud == null)
                {
                    modeloDeSolicitudLimpieza.FechaSolicitud = DateTime.Now;
                }

                // Validar que la descripción no esté vacía
                if (string.IsNullOrWhiteSpace(modeloDeSolicitudLimpieza.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción es requerida.");
                    // Recargar listas
                    var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                    var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                    }).ToList();
                    ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeSolicitudLimpieza);
                }

                int cantidadDeDatosGuardados = await _registrarSolicitudLimpiezaLN.Guardar(modeloDeSolicitudLimpieza);
                
                if (cantidadDeDatosGuardados > 0)
                {
                    return RedirectToAction("IndexSolicitudLimpieza");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo guardar la solicitud de limpieza. Por favor, intente nuevamente.");
                    // Recargar listas
                    var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                    var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                    {
                        Value = d.IdDepartamento.ToString(),
                        Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                    }).ToList();
                    ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeSolicitudLimpieza);
                }
            }
            catch (Exception ex)
            {
                // Recargar las listas en caso de error
                var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                }).ToList();
                ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "Ocupado" || h.Estado == "En Mantenimiento");
                var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                }).ToList();
                ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");

                ModelState.AddModelError("", "Error al guardar la solicitud de limpieza: " + ex.Message);
                return View(modeloDeSolicitudLimpieza);
            }
        }

        // GET: SolicitudLimpieza/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "El ID de la solicitud es requerido.");
            }

            // Obtener la solicitud desde la base de datos
            var solicitudTabla = _contexto.Set<SolicitudLimpiezaTabla>().FirstOrDefault(s => s.IdSolicitudLimpieza == id.Value);
            if (solicitudTabla == null)
            {
                return HttpNotFound();
            }

            // Convertir a DTO
            var solicitudDTO = new SolicitudLimpiezaDTO
            {
                IdSolicitudLimpieza = solicitudTabla.IdSolicitudLimpieza,
                Descripcion = solicitudTabla.Descripcion,
                Estado = solicitudTabla.Estado,
                idDepartamento = solicitudTabla.idDepartamento ?? 0,
                DepartamentoNombre = solicitudTabla.DepartamentoNombre,
                idHabitacion = solicitudTabla.idHabitacion ?? 0,
                FechaSolicitud = solicitudTabla.FechaSolicitud
            };

            // Obtener el nombre de la habitación si existe
            if (solicitudDTO.idHabitacion > 0)
            {
                var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == solicitudDTO.idHabitacion);
                if (habitacion != null)
                {
                    solicitudDTO.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                }
            }

            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, solicitudDTO.Estado);
            
            return View(solicitudDTO);
        }

        // POST: SolicitudLimpieza/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SolicitudLimpiezaDTO solicitudLimpieza)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que la solicitud existe
                    var solicitudExistente = _contexto.Set<SolicitudLimpiezaTabla>().FirstOrDefault(s => s.IdSolicitudLimpieza == solicitudLimpieza.IdSolicitudLimpieza);
                    if (solicitudExistente == null)
                    {
                        return HttpNotFound();
                    }

                    // Guardar datos anteriores para bitácora
                    var datosAnteriores = $@"{{
                        ""IdSolicitudLimpieza"": {solicitudExistente.IdSolicitudLimpieza},
                        ""Descripcion"": ""{solicitudExistente.Descripcion}"",
                        ""Estado"": ""{solicitudExistente.Estado}"",
                        ""idDepartamento"": {solicitudExistente.idDepartamento ?? 0},
                        ""idHabitacion"": {solicitudExistente.idHabitacion ?? 0}
                    }}";

                    // Actualizar los campos
                    solicitudExistente.Descripcion = solicitudLimpieza.Descripcion;
                    solicitudExistente.Estado = solicitudLimpieza.Estado;

                    // Actualizar los nombres si cambió la ubicación
                    if (solicitudLimpieza.idDepartamento > 0)
                    {
                        var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == solicitudLimpieza.idDepartamento);
                        if (departamento != null)
                        {
                            solicitudExistente.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                            solicitudExistente.idDepartamento = solicitudLimpieza.idDepartamento;
                            solicitudExistente.idHabitacion = null;
                        }
                    }
                    else if (solicitudLimpieza.idHabitacion > 0)
                    {
                        var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == solicitudLimpieza.idHabitacion);
                        if (habitacion != null)
                        {
                            solicitudExistente.idHabitacion = solicitudLimpieza.idHabitacion;
                            solicitudExistente.idDepartamento = null;
                        }
                    }

                    // Si se marca como Completado, actualizar la habitación o departamento a Disponible (para que el plano de estado lo muestre bien)
                    if (string.Equals(solicitudLimpieza.Estado, "Completado", StringComparison.OrdinalIgnoreCase))
                    {
                        if (solicitudExistente.idHabitacion.HasValue && solicitudExistente.idHabitacion.Value > 0)
                        {
                            var habitacion = _contexto.HabitacionesTabla.Find(solicitudExistente.idHabitacion.Value);
                            if (habitacion != null)
                            {
                                habitacion.Estado = "Disponible";
                                _contexto.Entry(habitacion).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        if (solicitudExistente.idDepartamento.HasValue && solicitudExistente.idDepartamento.Value > 0)
                        {
                            var departamento = _contexto.DepartamentoTabla.Find(solicitudExistente.idDepartamento.Value);
                            if (departamento != null)
                            {
                                departamento.Estado = "Disponible";
                                _contexto.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }

                    // Marcar como modificado y guardar
                    _contexto.Entry(solicitudExistente).State = System.Data.Entity.EntityState.Modified;
                    int cantidadDeDatosActualizados = await _contexto.SaveChangesAsync();

                    if (cantidadDeDatosActualizados > 0)
                    {
                        // Registrar en bitácora
                        var datosPosteriores = $@"{{
                            ""IdSolicitudLimpieza"": {solicitudExistente.IdSolicitudLimpieza},
                            ""Descripcion"": ""{solicitudExistente.Descripcion}"",
                            ""Estado"": ""{solicitudExistente.Estado}"",
                            ""idDepartamento"": {solicitudExistente.idDepartamento ?? 0},
                            ""idHabitacion"": {solicitudExistente.idHabitacion ?? 0}
                        }}";

                        var bitacora = new HotelPrado.Abstracciones.Modelos.Bitacora.BitacoraEventosDTO
                        {
                            IdEvento = 0,
                            TablaDeEvento = "ModuloSolicitudLimpieza",
                            TipoDeEvento = "Actualizar",
                            FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                            DescripcionDeEvento = "Se actualizó una Solicitud de Limpieza en la base de datos.",
                            StackTrace = "Sin errores",
                            DatosAnteriores = datosAnteriores,
                            DatosPosteriores = datosPosteriores
                        };

                        await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                        return RedirectToAction("IndexSolicitudLimpieza");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar la solicitud de limpieza.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló, recargar ViewBag
            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, solicitudLimpieza.Estado);
            
            return View(solicitudLimpieza);
        }

        // POST: SolicitudLimpieza/CambiarEstado - Cambio rápido de estado vía AJAX
        [HttpPost]
        public async Task<JsonResult> CambiarEstado(int id, string nuevoEstado)
        {
            int? idHabitacion = null;
            int? idDepartamento = null;
            string estadoAnterior = "";
            
            try
            {
                // Obtener datos de la solicitud usando SQL directo
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    // Obtener datos de la solicitud
                    using (var selectCommand = new System.Data.SqlClient.SqlCommand(@"
                        SELECT IdSolicitudLimpieza, Estado, idHabitacion, idDepartamento, FechaSolicitud
                        FROM SolicitudesLimpieza 
                        WHERE IdSolicitudLimpieza = @Id", connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Id", id);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return Json(new { success = false, message = "Solicitud no encontrada" });
                            }
                            
                            estadoAnterior = reader.IsDBNull(reader.GetOrdinal("Estado")) ? "" : reader.GetString(reader.GetOrdinal("Estado"));
                            idHabitacion = reader.IsDBNull(reader.GetOrdinal("idHabitacion")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("idHabitacion"));
                            idDepartamento = reader.IsDBNull(reader.GetOrdinal("idDepartamento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("idDepartamento"));
                        }
                    }
                    
                    // Actualizar TODOS los cambios en una sola transacción SQL
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Actualizar estado de la solicitud de limpieza
                            using (var updateSolicitud = new System.Data.SqlClient.SqlCommand(@"
                                UPDATE SolicitudesLimpieza 
                                SET Estado = @Estado,
                                    FechaSolicitud = CASE 
                                        WHEN @Estado = 'En Proceso' AND FechaSolicitud IS NULL THEN GETDATE()
                                        ELSE FechaSolicitud
                                    END
                                WHERE IdSolicitudLimpieza = @Id", connection, transaction))
                            {
                                updateSolicitud.Parameters.AddWithValue("@Estado", nuevoEstado);
                                updateSolicitud.Parameters.AddWithValue("@Id", id);
                                int rowsSolicitud = updateSolicitud.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"Solicitud actualizada: {rowsSolicitud} filas afectadas");
                                
                                if (rowsSolicitud == 0)
                                {
                                    throw new Exception("No se pudo actualizar la solicitud");
                                }
                            }
                            
                            // 2. Si se completó la limpieza, actualizar el estado de la habitación o departamento a "Disponible"
                            if (nuevoEstado == "Completado")
                            {
                                // Si es una habitación, actualizar su estado a "Disponible"
                                if (idHabitacion.HasValue && idHabitacion.Value > 0)
                                {
                                    using (var updateHabitacion = new System.Data.SqlClient.SqlCommand(@"
                                        UPDATE Habitaciones 
                                        SET Estado = @Estado
                                        WHERE IdHabitacion = @IdHabitacion", connection, transaction))
                                    {
                                        updateHabitacion.Parameters.AddWithValue("@Estado", "Disponible");
                                        updateHabitacion.Parameters.AddWithValue("@IdHabitacion", idHabitacion.Value);
                                        int rowsHabitacion = updateHabitacion.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine($"Habitación {idHabitacion.Value} actualizada a Disponible: {rowsHabitacion} filas afectadas");
                                        
                                        if (rowsHabitacion == 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Advertencia: No se encontró la habitación {idHabitacion.Value}");
                                        }
                                    }
                                }
                                
                                // Si es un departamento, actualizar su estado a "Disponible"
                                if (idDepartamento.HasValue && idDepartamento.Value > 0)
                                {
                                    using (var updateDepartamento = new System.Data.SqlClient.SqlCommand(@"
                                        UPDATE Departamento 
                                        SET Estado = @Estado
                                        WHERE IdDepartamento = @IdDepartamento", connection, transaction))
                                    {
                                        updateDepartamento.Parameters.AddWithValue("@Estado", "Disponible");
                                        updateDepartamento.Parameters.AddWithValue("@IdDepartamento", idDepartamento.Value);
                                        int rowsDepartamento = updateDepartamento.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine($"Departamento {idDepartamento.Value} actualizado a Disponible: {rowsDepartamento} filas afectadas");
                                        
                                        if (rowsDepartamento == 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Advertencia: No se encontró el departamento {idDepartamento.Value}");
                                        }
                                    }
                                }
                            }
                            
                            // Confirmar transacción
                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine("=== CAMBIO DE ESTADO EXITOSO - TRANSACCIÓN COMMITEADA ===");
                        }
                        catch (Exception exTrans)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"Error en transacción, rollback ejecutado: {exTrans.Message}");
                            throw;
                        }
                    }
                }
                
                // Registrar en bitácora
                var datosAnteriores = $@"{{""Estado"": ""{estadoAnterior}""}}";
                var datosPosteriores = $@"{{""Estado"": ""{nuevoEstado}""}}";

                var bitacora = new HotelPrado.Abstracciones.Modelos.Bitacora.BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloSolicitudLimpieza",
                    TipoDeEvento = "CambiarEstado",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Se cambió el estado de la solicitud #{id} de {estadoAnterior} a {nuevoEstado}." + 
                        (nuevoEstado == "Completado" && idHabitacion.HasValue ? $" Habitación {idHabitacion.Value} marcada como Disponible." : ""),
                    StackTrace = "Sin errores",
                    DatosAnteriores = datosAnteriores,
                    DatosPosteriores = datosPosteriores
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = true, message = "Estado actualizado correctamente" + 
                    (nuevoEstado == "Completado" ? ". La habitación/departamento ha sido marcado como Disponible." : ""), 
                    nuevoEstado = nuevoEstado });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CambiarEstado: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        
        // POST: SolicitudLimpieza/LimpiarSolicitudesAntiguas - Eliminar solicitudes completadas antiguas
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public JsonResult LimpiarSolicitudesAntiguas(int diasAntiguedad = 30)
        {
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    using (var deleteCommand = new System.Data.SqlClient.SqlCommand(@"
                        DELETE FROM SolicitudesLimpieza 
                        WHERE Estado = 'Completado' 
                          AND FechaSolicitud < DATEADD(day, -@DiasAntiguedad, GETDATE())", connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@DiasAntiguedad", diasAntiguedad);
                        int rowsDeleted = deleteCommand.ExecuteNonQuery();
                        
                        System.Diagnostics.Debug.WriteLine($"Solicitudes eliminadas: {rowsDeleted}");
                        
                        // Registrar en bitácora
                        var bitacora = new HotelPrado.Abstracciones.Modelos.Bitacora.BitacoraEventosDTO
                        {
                            IdEvento = 0,
                            TablaDeEvento = "ModuloSolicitudLimpieza",
                            TipoDeEvento = "LimpiezaAutomatica",
                            FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                            DescripcionDeEvento = $"Se eliminaron {rowsDeleted} solicitudes de limpieza completadas con más de {diasAntiguedad} días de antigüedad.",
                            StackTrace = "Sin errores",
                            DatosAnteriores = "NA",
                            DatosPosteriores = $"{{\"SolicitudesEliminadas\": {rowsDeleted}, \"DiasAntiguedad\": {diasAntiguedad}}}"
                        };
                        
                        _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                        
                        return Json(new { success = true, message = $"Se eliminaron {rowsDeleted} solicitudes de limpieza completadas con más de {diasAntiguedad} días de antigüedad." });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al limpiar solicitudes antiguas: {ex.Message}");
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}

