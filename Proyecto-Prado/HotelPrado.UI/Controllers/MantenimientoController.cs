using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Mantenimiento.Editar;
using HotelPrado.LN.Mantenimiento.Listar;
using HotelPrado.LN.Mantenimiento.ObtenerPorId;
using HotelPrado.LN.Mantenimiento.Registrar;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class MantenimientoController : Controller
    {
        private readonly IListarMantenimientoLN _listarMantenimientoLN;
        private readonly IRegistrarMantenimientoLN _registrarMantenimientoLN;
        private readonly IEditarMantenimientoLN _editarMantenimientoLN;
        private readonly Contexto _contexto;
        private readonly IObtenerMantenimientoPorIdLN _obtenerMantenimientoPorIdLN;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public MantenimientoController()
        {
            _registrarMantenimientoLN = new RegistrarMantenimientoLN();
            _listarMantenimientoLN = new ListarMantenimientoLN();
            _editarMantenimientoLN = new EditarMantenimientoLN();
            _contexto = new Contexto();
            _obtenerMantenimientoPorIdLN = new ObtenerMantenimientoPorIdLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        // GET: Mantenimiento (caché 60 s para aligerar carga en host)
        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult IndexMantenimiento()
        {
            ViewBag.Title = "El Mantenimiento";
            var laListaDeMantenimiento = _listarMantenimientoLN.Listar();
            return View(laListaDeMantenimiento);
        }

        // GET: Mantenimiento/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id);
                if (mantenimiento == null)
                {
                    return HttpNotFound();
                }
                return View(mantenimiento);
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al obtener los detalles del mantenimiento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                return RedirectToAction("IndexMantenimiento");
            }
        }

        // GET: Mantenimiento/Create
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
                .Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");

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
            var nuevoModelo = new MantenimientoDTO
            {
                Estado = "Pendiente", // Valor por defecto
                idDepartamento = 0,
                idHabitacion = 0
            };

            if (IdDepartamento.HasValue)
            {
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento.Value);
                if (departamento != null)
                {
                    nuevoModelo.idDepartamento = IdDepartamento.Value;
                    ViewBag.IdDepartamento = IdDepartamento.Value;
                    ViewBag.TipoSeleccionado = "Departamento";
                }
            }
            else if (IdHabitacion.HasValue)
            {
                var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == IdHabitacion.Value);
                if (habitacion != null)
                {
                    nuevoModelo.idHabitacion = IdHabitacion.Value;
                    ViewBag.IdHabitacion = IdHabitacion.Value;
                    ViewBag.TipoSeleccionado = "Habitacion";
                }
            }

            return View(nuevoModelo);
        }

        // POST: Mantenimiento/Create
        [HttpPost]
        public async Task<ActionResult> Create(MantenimientoDTO modeloDeMantenimiento)
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

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeMantenimiento);
                }

                // Validar que se haya seleccionado al menos un departamento o habitación
                if (modeloDeMantenimiento.idDepartamento == 0 && modeloDeMantenimiento.idHabitacion == 0)
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

                    var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                    var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                    {
                        Value = h.IdHabitacion.ToString(),
                        Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                    }).ToList();
                    ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                    return View(modeloDeMantenimiento);
                }

                // Obtener el nombre del departamento o habitación desde la base de datos
                if (modeloDeMantenimiento.idDepartamento > 0)
                {
                    var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == modeloDeMantenimiento.idDepartamento);
                    if (departamento != null)
                    {
                        modeloDeMantenimiento.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                        modeloDeMantenimiento.idHabitacion = 0; // Asegurar que habitación sea 0
                    }
                    else
                    {
                        ModelState.AddModelError("", "El departamento seleccionado no existe.");
                        // Recargar listas
                        var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                        var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                        }).ToList();
                        ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                        var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                        var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                        {
                            Value = h.IdHabitacion.ToString(),
                            Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                        }).ToList();
                        ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                        return View(modeloDeMantenimiento);
                    }
                }
                else if (modeloDeMantenimiento.idHabitacion > 0)
                {
                    var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == modeloDeMantenimiento.idHabitacion);
                    if (habitacion != null)
                    {
                        modeloDeMantenimiento.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                        modeloDeMantenimiento.idDepartamento = 0; // Asegurar que departamento sea 0
                    }
                    else
                    {
                        ModelState.AddModelError("", "La habitación seleccionada no existe.");
                        // Recargar listas
                        var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                        var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre
                        }).ToList();
                        ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                        var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                        var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                        {
                            Value = h.IdHabitacion.ToString(),
                            Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                        }).ToList();
                        ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                        return View(modeloDeMantenimiento);
                    }
                }

                int cantidadDeDatosGuardados = await _registrarMantenimientoLN.Guardar(modeloDeMantenimiento);
                return RedirectToAction("IndexMantenimiento");
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

                var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString())
                }).ToList();
                ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");

                ModelState.AddModelError("", "Error al guardar el mantenimiento: " + ex.Message);
                return View(modeloDeMantenimiento);
            }
        }

        // GET: Mantenimiento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "El ID del mantenimiento es requerido.");
            }

            var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id.Value);
            if (mantenimiento == null)
            {
                return HttpNotFound();
            }

            // Obtener la lista de departamentos
            var departamentosQuery = _contexto.DepartamentoTabla
                .Where(d => d.Estado == "Disponible");

            var departamentos = departamentosQuery
                .ToList()
                .Select(d => new SelectListItem
                {
                    Value = d.IdDepartamento.ToString(),
                    Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre,
                    Selected = d.IdDepartamento == mantenimiento.idDepartamento
                })
                .ToList();

            ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

            // Obtener la lista de habitaciones
            var habitacionesQuery = _contexto.HabitacionesTabla
                .Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");

            var habitaciones = habitacionesQuery
                .ToList()
                .Select(h => new SelectListItem
                {
                    Value = h.IdHabitacion.ToString(),
                    Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString()),
                    Selected = h.IdHabitacion == mantenimiento.idHabitacion
                })
                .ToList();

            ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, mantenimiento.Estado);
            
            // Determinar el tipo seleccionado
            if (mantenimiento.idHabitacion > 0)
            {
                ViewBag.TipoSeleccionado = "Habitacion";
            }
            else if (mantenimiento.idDepartamento > 0)
            {
                ViewBag.TipoSeleccionado = "Departamento";
            }

            return View(mantenimiento);
        }

        // POST: Mantenimiento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MantenimientoDTO elmantenimiento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que se haya seleccionado al menos un departamento o habitación
                    if (elmantenimiento.idDepartamento == 0 && elmantenimiento.idHabitacion == 0)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un departamento o una habitación.");
                    }
                    else
                    {
                        // Obtener el nombre del departamento o habitación
                        if (elmantenimiento.idDepartamento > 0)
                        {
                            var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == elmantenimiento.idDepartamento);
                            if (departamento != null)
                            {
                                elmantenimiento.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());
                                elmantenimiento.idHabitacion = 0; // Asegurar que habitación sea 0
                            }
                        }
                        else if (elmantenimiento.idHabitacion > 0)
                        {
                            var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == elmantenimiento.idHabitacion);
                            if (habitacion != null)
                            {
                                elmantenimiento.HabitacionNombre = "Habitación " + (habitacion.NumeroHabitacion ?? habitacion.IdHabitacion.ToString());
                                elmantenimiento.idDepartamento = 0; // Asegurar que departamento sea 0
                            }
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        // Recargar listas si hay errores
                        var departamentosQuery = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
                        var departamentos = departamentosQuery.ToList().Select(d => new SelectListItem
                        {
                            Value = d.IdDepartamento.ToString(),
                            Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre,
                            Selected = d.IdDepartamento == elmantenimiento.idDepartamento
                        }).ToList();
                        ViewBag.Departamentos = new SelectList(departamentos, "Value", "Text");

                        var habitacionesQuery = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
                        var habitaciones = habitacionesQuery.ToList().Select(h => new SelectListItem
                        {
                            Value = h.IdHabitacion.ToString(),
                            Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString()),
                            Selected = h.IdHabitacion == elmantenimiento.idHabitacion
                        }).ToList();
                        ViewBag.Habitaciones = new SelectList(habitaciones, "Value", "Text");
                        ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, elmantenimiento.Estado);
                        
                        if (elmantenimiento.idHabitacion > 0)
                        {
                            ViewBag.TipoSeleccionado = "Habitacion";
                        }
                        else if (elmantenimiento.idDepartamento > 0)
                        {
                            ViewBag.TipoSeleccionado = "Departamento";
                        }
                        
                        return View(elmantenimiento);
                    }

                    var cantidadDeDatosActualizados = await _editarMantenimientoLN.Actualizar(elmantenimiento);
                    if (cantidadDeDatosActualizados == 0)
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar el mantenimiento.");
                    }
                    else
                    {
                        return RedirectToAction("IndexMantenimiento");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }

            // Si llegamos aquí, algo falló, volvemos a cargar los dropdowns
            var departamentosQueryFinal = _contexto.DepartamentoTabla.Where(d => d.Estado == "Disponible");
            var departamentosFinal = departamentosQueryFinal.ToList().Select(d => new SelectListItem
            {
                Value = d.IdDepartamento.ToString(),
                Text = (d.NumeroDepartamento == 0 ? "Sin número" : d.NumeroDepartamento.ToString()) + " - " + d.Nombre,
                Selected = d.IdDepartamento == elmantenimiento.idDepartamento
            }).ToList();
            ViewBag.Departamentos = new SelectList(departamentosFinal, "Value", "Text");

            var habitacionesQueryFinal = _contexto.HabitacionesTabla.Where(h => h.Estado == "Disponible" || h.Estado == "En Mantenimiento");
            var habitacionesFinal = habitacionesQueryFinal.ToList().Select(h => new SelectListItem
            {
                Value = h.IdHabitacion.ToString(),
                Text = "Habitación " + (h.NumeroHabitacion ?? h.IdHabitacion.ToString()),
                Selected = h.IdHabitacion == elmantenimiento.idHabitacion
            }).ToList();
            ViewBag.Habitaciones = new SelectList(habitacionesFinal, "Value", "Text");
            ViewBag.Estados = new SelectList(new[] { "Pendiente", "En Proceso", "Completado" }, elmantenimiento.Estado);
            
            if (elmantenimiento.idHabitacion > 0)
            {
                ViewBag.TipoSeleccionado = "Habitacion";
            }
            else if (elmantenimiento.idDepartamento > 0)
            {
                ViewBag.TipoSeleccionado = "Departamento";
            }

            return View(elmantenimiento);
        }

        // GET: Mantenimiento/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id);
            if (mantenimiento == null)
                return HttpNotFound();
            return View(mantenimiento);
        }

        // POST: Mantenimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var mantenimiento = await _obtenerMantenimientoPorIdLN.Obtener(id);
            if (mantenimiento == null)
                return HttpNotFound();

            try
            {
                var entidad = _contexto.MantenimientoTabla.Find(id);
                if (entidad != null)
                {
                    _contexto.MantenimientoTabla.Remove(entidad);
                    await _contexto.SaveChangesAsync();

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloMantenimiento",
                        TipoDeEvento = "Eliminar mantenimiento",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = $"Se eliminó el mantenimiento (Id: {id}): {entidad.Descripcion}.",
                        StackTrace = "Sin errores",
                        DatosAnteriores = $@"{{ ""IdMantenimiento"": {id}, ""Descripcion"": ""{(entidad.Descripcion ?? "").Replace("\"", "'")}"", ""Estado"": ""{entidad.Estado}"" }}",
                        DatosPosteriores = "{}",
                        Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
                    };
                    await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                TempData["MensajeExito"] = "Mantenimiento eliminado correctamente.";
                return RedirectToAction("IndexMantenimiento");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
            {
                ModelState.AddModelError("", "No se puede eliminar este mantenimiento porque tiene registros relacionados.");
                return View(mantenimiento);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(mantenimiento);
            }
        }

        [HttpPost]
        public ActionResult ToggleEstado(int IdMantenimiento, string Estado)
        {
            try
            {
                // Usar SQL directo para evitar problemas de contexto asíncrono
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    // Obtener datos del mantenimiento antes de actualizar
                    string descripcion = "";
                    int? idDepartamento = null;
                    string departamentoNombre = "";
                    int? idHabitacion = null;
                    
                    // Verificar si la columna idHabitacion existe
                    bool columnaHabitacionExiste = false;
                    using (var checkColumn = new System.Data.SqlClient.SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Mantenimiento' 
                        AND COLUMN_NAME = 'idHabitacion'", connection))
                    {
                        columnaHabitacionExiste = ((int)checkColumn.ExecuteScalar()) > 0;
                    }
                    
                    string selectQuery = columnaHabitacionExiste
                        ? @"SELECT Descripcion, idDepartamento, DepartamentoNombre, idHabitacion
                           FROM Mantenimiento 
                           WHERE IdMantenimiento = @IdMantenimiento"
                        : @"SELECT Descripcion, idDepartamento, DepartamentoNombre, NULL as idHabitacion
                           FROM Mantenimiento 
                           WHERE IdMantenimiento = @IdMantenimiento";
                    
                    using (var selectCommand = new System.Data.SqlClient.SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@IdMantenimiento", IdMantenimiento);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? "" : reader.GetString(reader.GetOrdinal("Descripcion"));
                                
                                int deptOrdinal = reader.GetOrdinal("idDepartamento");
                                idDepartamento = reader.IsDBNull(deptOrdinal) ? (int?)null : reader.GetInt32(deptOrdinal);
                                
                                departamentoNombre = reader.IsDBNull(reader.GetOrdinal("DepartamentoNombre")) ? "" : reader.GetString(reader.GetOrdinal("DepartamentoNombre"));
                                
                                if (columnaHabitacionExiste)
                                {
                                    int habOrdinal = reader.GetOrdinal("idHabitacion");
                                    idHabitacion = reader.IsDBNull(habOrdinal) ? (int?)null : reader.GetInt32(habOrdinal);
                                }
                            }
                            else
                            {
                                if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                                {
                                    return Json(new { success = false, message = "Mantenimiento no encontrado" });
                                }
                                return RedirectToAction("IndexMantenimiento");
                            }
                        }
                    }
                    
                    // Actualizar estado usando SQL directo
                    using (var updateCommand = new System.Data.SqlClient.SqlCommand(@"
                        UPDATE Mantenimiento 
                        SET Estado = @Estado
                        WHERE IdMantenimiento = @IdMantenimiento", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Estado", Estado);
                        updateCommand.Parameters.AddWithValue("@IdMantenimiento", IdMantenimiento);
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        
                        if (rowsAffected == 0)
                        {
                            if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {
                                return Json(new { success = false, message = "No se pudo actualizar el mantenimiento" });
                            }
                            return RedirectToAction("IndexMantenimiento");
                        }
                    }
                    
                    // Si se completó el mantenimiento, actualizar el estado de la habitación o departamento correspondiente
                    if (Estado == "Completado")
                    {
                        // Si está asociado a una habitación (idHabitacion no es NULL)
                        if (idHabitacion.HasValue && idHabitacion.Value > 0)
                        {
                            try
                            {
                                using (var updateHabitacion = new System.Data.SqlClient.SqlCommand(@"
                                    UPDATE Habitaciones 
                                    SET Estado = 'Disponible'
                                    WHERE IdHabitacion = @IdHabitacion 
                                      AND LOWER(LTRIM(RTRIM(Estado))) = 'mantenimiento'", connection))
                                {
                                    updateHabitacion.Parameters.AddWithValue("@IdHabitacion", idHabitacion.Value);
                                    int rowsHabitacion = updateHabitacion.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"Habitación {idHabitacion.Value} actualizada a Disponible: {rowsHabitacion} filas afectadas");
                                }
                            }
                            catch (Exception exHabitacion)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al actualizar estado de habitación: {exHabitacion.Message}");
                                // Continuar aunque falle
                            }
                        }
                        // Si está asociado a un departamento (idDepartamento no es NULL y idHabitacion es NULL)
                        else if (idDepartamento.HasValue && idDepartamento.Value > 0)
                        {
                            try
                            {
                                using (var updateDepartamento = new System.Data.SqlClient.SqlCommand(@"
                                    UPDATE Departamento 
                                    SET Estado = 'Disponible'
                                    WHERE IdDepartamento = @IdDepartamento 
                                      AND LOWER(LTRIM(RTRIM(Estado))) = 'mantenimiento'", connection))
                                {
                                    updateDepartamento.Parameters.AddWithValue("@IdDepartamento", idDepartamento.Value);
                                    int rowsDepartamento = updateDepartamento.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"Departamento {idDepartamento.Value} actualizado a Disponible: {rowsDepartamento} filas afectadas");
                                }
                            }
                            catch (Exception exDepartamento)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al actualizar estado de departamento: {exDepartamento.Message}");
                                // Continuar aunque falle
                            }
                        }
                    }
                    
                    // Registrar en bitácora usando SQL directo
                    try
                    {
                        string datosJson = $@"{{
                            ""IdMantenimiento"": {IdMantenimiento},
                            ""Descripcion"": ""{descripcion.Replace("\"", "\\\"")}"",
                            ""Estado"": ""{Estado}"",
                            ""idDepartamento"": {idDepartamento},
                            ""DepartamentoNombre"": ""{departamentoNombre.Replace("\"", "\\\"")}"",
                            ""idHabitacion"": {idHabitacion}
                        }}";
                        
                        using (var insertBitacora = new System.Data.SqlClient.SqlCommand(@"
                            INSERT INTO bitacoraEventos (TablaDeEvento, TipoDeEvento, FechaDeEvento, DescripcionDeEvento, StackTrace, DatosAnteriores, DatosPosteriores, Usuario)
                            VALUES (@TablaDeEvento, @TipoDeEvento, @FechaDeEvento, @DescripcionDeEvento, @StackTrace, @DatosAnteriores, @DatosPosteriores, @Usuario)", connection))
                        {
                            insertBitacora.Parameters.AddWithValue("@TablaDeEvento", "ModuloMantenimiento");
                            insertBitacora.Parameters.AddWithValue("@TipoDeEvento", "Actualizar");
                            insertBitacora.Parameters.AddWithValue("@FechaDeEvento", DateTime.Now);
                            insertBitacora.Parameters.AddWithValue("@DescripcionDeEvento", "Se actualizó el estado de un Mantenimiento.");
                            insertBitacora.Parameters.AddWithValue("@StackTrace", "Sin errores");
                            insertBitacora.Parameters.AddWithValue("@DatosAnteriores", datosJson);
                            insertBitacora.Parameters.AddWithValue("@DatosPosteriores", datosJson);
                            insertBitacora.Parameters.AddWithValue("@Usuario", User?.Identity?.Name ?? "Sistema");
                            insertBitacora.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exBitacora)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al registrar en bitácora: {exBitacora.Message}");
                        // Continuar aunque falle la bitácora
                    }
                    
                    // Retornar respuesta según el tipo de solicitud
                    if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Estado actualizado correctamente" });
                    }
                    return RedirectToAction("IndexMantenimiento");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ToggleEstado: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Retornar JSON para solicitudes AJAX
                if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Error al actualizar el estado: " + ex.Message });
                }
                return RedirectToAction("IndexMantenimiento");
            }
        }
    }
}
