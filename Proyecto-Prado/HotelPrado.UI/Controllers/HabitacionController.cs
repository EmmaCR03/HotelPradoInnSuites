using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoHabitacion.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Listar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesHabitacion;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Habitaciones.Editar;
using HotelPrado.LN.Habitaciones.HabDisponibles;
using HotelPrado.LN.Habitaciones.Listar;
using HotelPrado.LN.Habitaciones.ObtenerPorId;
using HotelPrado.LN.Habitaciones.Registrar;
using HotelPrado.LN.TipoHabitacion.Listar;
using HotelPrado.LN.Reservas;
using HotelPrado.LN.Reservas.Listar;
using HotelPrado.UI.Models;
using HotelPrado.UI.Services;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.LN.SolicitudLimpieza.Registrar;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    public class HabitacionController : Controller
    {
        IListarHabitacionesLN _listarHabitacionesLN;
        IHabDisponiblesLN _habDisponiblesLN;
        IListarTipoHabitacionLN _listarTipoHabitacionLN;
        IRegistrarHabitacionesLN _registrarHabitacionesLN;
        Contexto _contexto;
        IEditarHabitacionesLN _editarHabitacionesLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerHabitacionesPorIdLN _obtenerHabitacionesPorId;
        IHabDisponiblesLN _habDisponibles;
        IReservasLN _reservasLN;
        IListarReservasLN _listarReservasLN;
        HabitacionEstadoService _habitacionEstadoService;

        public HabitacionController()
        {
            _listarHabitacionesLN = new ListarHabitacionesLN();
            _listarTipoHabitacionLN = new ListarTipoHabitacionLN();
            _registrarHabitacionesLN = new RegistrarHabitacionesLN();
            _contexto = new Contexto();
            _obtenerHabitacionesPorId = new ObtenerHabitacionesPorIdLN();
            _editarHabitacionesLN = new EditarHabitacionesLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _habDisponibles = new HabDisponiblesLN();
            _reservasLN = new ReservasLN();
            _listarReservasLN = new ListarReservasLN();
            _habitacionEstadoService = new HabitacionEstadoService(_reservasLN, _listarHabitacionesLN, _listarReservasLN);
        }

        // GET: Habitacion (caché 60 s para que la primera carga y siguientes sean más fluidas)
        [OutputCache(Duration = 60, VaryByParam = "capacidad;estado", Location = System.Web.UI.OutputCacheLocation.Server)]
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult IndexHabitaciones(int? capacidad, string estado)
        {
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitaciones = _listarHabitacionesLN.Listar(capacidad, estado);
            return View(laListaDeHabitaciones);
        }

        [AllowAnonymous]
        public ActionResult habitacionesInfo()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    contexto.Configuration.AutoDetectChangesEnabled = true;
                    var heroBanner = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina == "Habitaciones")
                        .OrderByDescending(c => c.FechaActualizacion)
                        .FirstOrDefault();
                    ViewBag.HeroBannerUrl = heroBanner?.UrlImagen ?? "/Img/images/IMG_2.JPG";

                    // Galería de habitaciones: misma configuración que Admin → Galería Habitaciones (GaleriaHabitaciones_*)
                    var listHabitaciones = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith("GaleriaHabitaciones_"))
                        .ToList();
                    var galeriaHabitaciones = listHabitaciones
                        .OrderBy(c =>
                        {
                            var s = (c.Pagina ?? "").Replace("GaleriaHabitaciones_", "");
                            int n;
                            return int.TryParse(s, out n) ? n : 0;
                        })
                        .Select(c =>
                        {
                            var u = c.UrlImagen ?? "";
                            var bar = u.IndexOf('|');
                            return Tuple.Create(bar >= 0 ? u.Substring(0, bar).Trim() : u, bar >= 0 ? u.Substring(bar + 1).Trim() : "");
                        })
                        .ToList();
                    ViewBag.GaleriaHabitaciones = galeriaHabitaciones;
                }
            }
            catch (Exception ex)
            {
                ViewBag.HeroBannerUrl = "/Img/images/IMG_2.JPG";
                ViewBag.GaleriaHabitaciones = null;
                System.Diagnostics.Debug.WriteLine("Error habitacionesInfo: " + ex.Message);
            }
            return View();
        }

        // GET: Habitacion/IndexHabitacionesUsuario
        [AllowAnonymous]
        public ActionResult IndexHabitacionesUsuario(DateTime? check_in, DateTime? check_out, int? capacidad)
        {
            // Validar parámetros
            if (!check_in.HasValue || !check_out.HasValue || !capacidad.HasValue)
            {
                ViewBag.ErrorMessage = "Por favor, complete todos los campos de búsqueda.";
                return View(new List<HabitacionesDTO>());
            }

            // Validar que las fechas sean válidas
            if (check_out.Value <= check_in.Value)
            {
                ViewBag.ErrorMessage = "La fecha de salida debe ser posterior a la fecha de entrada.";
                return View(new List<HabitacionesDTO>());
            }

            // Validar que la fecha de entrada no sea en el pasado (permitir hoy y fechas futuras)
            var hoy = DateTime.Today;
            if (check_in.Value.Date < hoy)
            {
                ViewBag.ErrorMessage = "La fecha de entrada no puede ser anterior a hoy.";
                return View(new List<HabitacionesDTO>());
            }

            // Calcular días y validar que sea al menos 1
            var dias = (check_out.Value - check_in.Value).Days;
            if (dias < 1)
            {
                ViewBag.ErrorMessage = "La estadía debe ser de al menos 1 noche.";
                return View(new List<HabitacionesDTO>());
            }

            // Validar capacidad
            if (capacidad.Value < 1 || capacidad.Value > 10)
            {
                ViewBag.ErrorMessage = "El número de personas debe estar entre 1 y 10.";
                return View(new List<HabitacionesDTO>());
            }

            ViewBag.CheckIn = check_in.Value;
            ViewBag.CheckOut = check_out.Value;
            ViewBag.Title = "Habitaciones Disponibles";
            
            try
            {
                var laListaDeHabitacionesDisponibles = _habDisponibles.ListarDisponibles(check_in.Value, check_out.Value, capacidad.Value);

                // Si hay habitaciones disponibles, solo mostrar una
                if (laListaDeHabitacionesDisponibles.Any())
                {
                    var habitacionDisponible = laListaDeHabitacionesDisponibles.First();
                    return View(new List<HabitacionesDTO> { habitacionDisponible });
                }

                // Si no hay habitaciones disponibles, devolver la lista vacía
                return View(new List<HabitacionesDTO>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar habitaciones: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Error al buscar habitaciones disponibles. Por favor, inténtelo de nuevo.";
                return View(new List<HabitacionesDTO>());
            }
        }

        // GET: Habitacion/Details/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Details(int id)
        {
            HabitacionesDTO hab = _obtenerHabitacionesPorId.Obtener(id);
            if (hab == null)
            {
                return HttpNotFound();
            }
            // Asegurar que UrlImagenes no sea null ni vacío
            if (string.IsNullOrWhiteSpace(hab.UrlImagenes))
            {
                hab.UrlImagenes = ""; // Evita errores en la vista
            }

            // Obtener información de reserva activa (buscar siempre, no solo si está ocupada)
            var hoy = DateTime.Now.Date;
            var reservaActiva = _listarReservasLN.Listar()
                .Where(r => r.IdHabitacion == id &&
                           r.FechaInicio.HasValue &&
                           r.FechaFinal.HasValue &&
                           (r.EstadoReserva == "Confirmada" || r.EstadoReserva == "En Proceso") &&
                           hoy >= r.FechaInicio.Value.Date &&
                           hoy <= r.FechaFinal.Value.Date)
                .OrderByDescending(r => r.FechaInicio)
                .FirstOrDefault();

            if (reservaActiva != null)
            {
                ViewBag.ReservaActiva = reservaActiva;
                ViewBag.IdReserva = reservaActiva.IdReserva;
                ViewBag.ClienteActual = reservaActiva.NombreCliente;
                ViewBag.EstadoReserva = reservaActiva.EstadoReserva;
                ViewBag.FechaCheckIn = reservaActiva.FechaInicio;
                ViewBag.FechaCheckOut = reservaActiva.FechaFinal;
            }

            return View(hab);
        }


        public string GetUrlImagenesById(int id)
        {
            if (_contexto?.HabitacionesTabla == null)
            {
                return string.Empty;
            }

            var urlImagenes = _contexto.HabitacionesTabla
                .Where(d => d.IdHabitacion == id)
                .Select(d => d.UrlImagenes)
                .FirstOrDefault();

            return urlImagenes ?? string.Empty; // Devuelve string vacío si es null
        }



        // GET: Habitacion/Create
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Create()
        {
            // Inicializar un nuevo modelo para la vista
            var nuevoModelo = new HabitacionesDTO
            {
                Estado = "Disponible", // Valor por defecto
                CapacidadMin = 1, // Valor por defecto
                CapacidadMax = 2 // Valor por defecto
            };

            return View(nuevoModelo);
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HabitacionesDTO modeloDeHabitaciones, IEnumerable<HttpPostedFileBase> Image)
        {
            if (ModelState.IsValid)
            {


                // Lista de archivos a almacenar
                var archivos = new List<string>();

                // Carpeta Content/Imagenes (misma que Editar Imágenes para que las URLs carguen bien)
                string uploadDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes");
                if (string.IsNullOrEmpty(uploadDirectory))
                    uploadDirectory = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, "Content", "Imagenes");
                try
                {
                    if (!Directory.Exists(uploadDirectory))
                        Directory.CreateDirectory(uploadDirectory);
                }
                catch (Exception exDir)
                {
                    ModelState.AddModelError("", "No se pudo crear la carpeta de imágenes. Ruta: " + uploadDirectory + " Error: " + exDir.Message);
                    return View(modeloDeHabitaciones);
                }

                // Leer archivos por índice (input multiple)
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(uploadDirectory, fileName);
                        try
                        {
                            file.SaveAs(path);
                            archivos.Add("/Content/Imagenes/" + fileName);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Error al guardar imagen '" + fileName + "': " + ex.Message + " (ruta: " + uploadDirectory + ")");
                            return View(modeloDeHabitaciones);
                        }
                    }
                }
                if (archivos.Count == 0 && Image != null)
                {
                    foreach (var file in Image)
                    {
                        if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(uploadDirectory, fileName);
                            try
                            {
                                file.SaveAs(path);
                                archivos.Add("/Content/Imagenes/" + fileName);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", "Error al guardar imagen '" + file.FileName + "': " + ex.Message);
                                return View(modeloDeHabitaciones);
                            }
                        }
                    }
                }
                if (archivos.Count > 0)
                    modeloDeHabitaciones.UrlImagenes = string.Join(",", archivos);

                // Guardar la habitación en la base de datos
                await _registrarHabitacionesLN.Guardar(modeloDeHabitaciones);

                // Guardar las imágenes con un contexto nuevo (evita NullReference/HttpContext)
                if (archivos.Count > 0)
                {
                    using (var db = new Contexto())
                    {
                        var nuevoId = db.HabitacionesTabla.OrderByDescending(h => h.IdHabitacion).Select(h => h.IdHabitacion).FirstOrDefault();
                        foreach (var url in archivos)
                        {
                            db.ImagenesHabitacionesTabla.Add(new ImagenesHabitacionTabla
                            {
                                IdHabitacion = nuevoId,
                                UrlImagen = url
                            });
                        }
                        await db.SaveChangesAsync();
                    }
                }

                return RedirectToAction("IndexHabitaciones");
            }

            // Si no es válido, regresamos el modelo para que se pueda corregir
            return View(modeloDeHabitaciones);
        }

        // GET: Habitacion/Edit/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Edit(int id)
        {
            var datosHabitacion = _obtenerHabitacionesPorId.Obtener(id);
            
            if (datosHabitacion == null)
            {
                return HttpNotFound();
            }

            // Asegurar que UrlImagenes no sea null
            if (string.IsNullOrWhiteSpace(datosHabitacion.UrlImagenes))
            {
                datosHabitacion.UrlImagenes = string.Empty;
            }

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Disponible", Value = "Disponible" },
                new SelectListItem { Text = "En Mantenimiento", Value = "En Mantenimiento" },
                new SelectListItem { Text = "Ocupado", Value = "Ocupado" }
            };
            return View(datosHabitacion);
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        // POST: Habitacion/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(HabitacionesDTO lahabitacion, List<string> eliminarImagenes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtén la habitacion desde la base de datos usando su ID
                    var habitacion = _obtenerHabitacionesPorId.Obtener(lahabitacion.IdHabitacion);

                    if (habitacion == null)
                    {
                        ViewBag.mensaje = "Habitación no encontrada.";
                        return View(lahabitacion);
                    }

                    // Asegurar que ListaImagenes no sea null
                    if (habitacion.ListaImagenes == null)
                    {
                        habitacion.ListaImagenes = new List<string>();
                    }

                    // Eliminar imágenes seleccionadas
                    if (eliminarImagenes != null && eliminarImagenes.Count > 0)
                    {
                        // Eliminar las imágenes de la lista de la habitacion
                        habitacion.ListaImagenes = habitacion.ListaImagenes.Except(eliminarImagenes).ToList();
                    }

                    // Guardar nuevas imágenes
                    if (Request.Files.Count > 0)
                    {
                        foreach (string file in Request.Files)
                        {
                            var imagen = Request.Files[file];
                            if (imagen.ContentLength > 0)
                            {
                                // Misma carpeta que Editar Imágenes para que las URLs se sirvan correctamente
                                var rutaCarpetaImagenes = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes");
                                if (string.IsNullOrEmpty(rutaCarpetaImagenes))
                                    rutaCarpetaImagenes = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, "Content", "Imagenes");
                                if (!Directory.Exists(rutaCarpetaImagenes))
                                    Directory.CreateDirectory(rutaCarpetaImagenes);
                                var ruta = "/Content/Imagenes/" + imagen.FileName;
                                var filePath = Path.Combine(rutaCarpetaImagenes, imagen.FileName);
                                imagen.SaveAs(filePath);

                                // Añadir la URL de la imagen a la lista de imágenes
                                habitacion.ListaImagenes.Add(ruta);
                            }
                        }
                    }

                    // Guardar cambios en la base de datos (las URLs de las imágenes)
                    habitacion.UrlImagenes = string.Join(",", habitacion.ListaImagenes) ?? "";

                    // Actualizar la habitacion en la base de datos
                    int cantidadDeDatosActualizados = await _editarHabitacionesLN.Actualizar(lahabitacion);

                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        return View(lahabitacion);
                    }

                    return RedirectToAction("IndexHabitaciones");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error inesperado: {ex.Message}");
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(lahabitacion);
                }
            }

            return View(lahabitacion);
        }

        // GET: Habitacion/Delete/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Delete(int id)
        {
            var habitacion = _obtenerHabitacionesPorId.Obtener(id);
            
            if (habitacion == null)
            {
                return HttpNotFound();
            }

            return View(habitacion);
        }
        // POST: Habitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var habitacion = _obtenerHabitacionesPorId.Obtener(id);
            if (habitacion == null)
                return HttpNotFound();

            try
            {
                var entidad = _contexto.HabitacionesTabla.Find(id);
                if (entidad != null)
                {
                    _contexto.HabitacionesTabla.Remove(entidad);
                    await _contexto.SaveChangesAsync();

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloHabitaciones",
                        TipoDeEvento = "Eliminar habitación",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = $"Se eliminó la habitación {entidad.NumeroHabitacion} (Id: {id}).",
                        StackTrace = "Sin errores",
                        DatosAnteriores = $@"{{ ""IdHabitacion"": {id}, ""NumeroHabitacion"": ""{entidad.NumeroHabitacion}"", ""Estado"": ""{entidad.Estado}"" }}",
                        DatosPosteriores = "{}",
                        Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
                    };
                    await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                TempData["MensajeExito"] = "Habitación eliminada correctamente.";
                try { Response.RemoveOutputCacheItem(Url.Action("IndexHabitaciones", "Habitacion")); } catch { }
                return RedirectToAction("IndexHabitaciones");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
            {
                ModelState.AddModelError("", "No se puede eliminar esta habitación porque tiene reservas, mantenimientos o otros registros relacionados.");
                return View(habitacion);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(habitacion);
            }
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult EditarImagenes(int id)
        {
            var habitaciones = _obtenerHabitacionesPorId.Obtener(id);

            if (habitaciones == null)
            {
                return HttpNotFound();
            }

            return View(habitaciones);
        }

        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarImagenes(int id, IEnumerable<HttpPostedFileBase> imagenes)
        {
            // Capturar usuario al inicio (mismo hilo/request) para evitar NullReference en HttpContext
            var usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema";

            HabitacionesTabla habitaciones;
            var imagenUrls = new List<string>();
            string datosAnteriores = "";

            using (var db = new Contexto())
            {
                habitaciones = db.HabitacionesTabla.Find(id);
                if (habitaciones == null)
                    return HttpNotFound();

                if (!string.IsNullOrWhiteSpace(habitaciones.UrlImagenes))
                    imagenUrls.AddRange(habitaciones.UrlImagenes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                datosAnteriores = string.Join(",", imagenUrls);

                var rutaCarpeta = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes");
                if (string.IsNullOrEmpty(rutaCarpeta))
                    rutaCarpeta = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, "Content", "Imagenes");
                var archivosSubidos = imagenes != null ? imagenes.Where(i => i != null && i.ContentLength > 0).ToList() : new List<HttpPostedFileBase>();
                if (archivosSubidos.Count == 0 && Request.Files != null && Request.Files.Count > 0)
                {
                    for (int idx = 0; idx < Request.Files.Count; idx++)
                    {
                        var f = Request.Files[idx];
                        if (f != null && f.ContentLength > 0)
                            archivosSubidos.Add(f);
                    }
                    if (archivosSubidos.Count == 0)
                    {
                        foreach (string key in Request.Files.AllKeys)
                        {
                            var f = Request.Files[key];
                            if (f != null && f.ContentLength > 0)
                                archivosSubidos.Add(f);
                        }
                    }
                }
                if (archivosSubidos.Count == 0)
                {
                    TempData["ErrorImagenes"] = "Seleccione al menos una imagen para agregar.";
                    return RedirectToAction("EditarImagenes", new { id = id });
                }
                try
                {
                    if (!Directory.Exists(rutaCarpeta))
                        Directory.CreateDirectory(rutaCarpeta);
                }
                catch (Exception exDir)
                {
                    TempData["ErrorImagenes"] = "No se pudo crear/usar la carpeta de imágenes. Ruta: " + rutaCarpeta + " Error: " + exDir.Message;
                    return RedirectToAction("EditarImagenes", new { id = id });
                }
                foreach (var imagen in archivosSubidos)
                {
                    var filePath = Path.Combine(rutaCarpeta, Path.GetFileName(imagen.FileName));
                    try
                    {
                        imagen.SaveAs(filePath);
                        imagenUrls.Add("/Content/Imagenes/" + Path.GetFileName(imagen.FileName));
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorImagenes"] = "Error al guardar '" + imagen.FileName + "': " + ex.Message + " (ruta: " + rutaCarpeta + ")";
                        return RedirectToAction("EditarImagenes", new { id = id });
                    }
                }
                habitaciones.UrlImagenes = string.Join(",", imagenUrls);
                db.Entry(habitaciones).Property(h => h.UrlImagenes).IsModified = true;
                db.SaveChanges();
            }

            var datosPosteriores = string.Join(",", imagenUrls);
            string datosJsonAnteriores = $@"{{ ""IdHabitacion"": {habitaciones.IdHabitacion}, ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"", ""UrlImagenes"": ""{datosAnteriores}"" }}";
            string datosJsonPosteriores = $@"{{ ""IdHabitacion"": {habitaciones.IdHabitacion}, ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"", ""UrlImagenes"": ""{datosPosteriores}"" }}";

            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloHabitaciones",
                TipoDeEvento = "Actualizar imágenes",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se actualizaron las imágenes de una habitacion.",
                StackTrace = "no hubo error",
                DatosAnteriores = datosJsonAnteriores,
                DatosPosteriores = datosJsonPosteriores,
                Usuario = usuario
            };

            await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

            TempData["MensajeImagenes"] = "Imágenes agregadas correctamente.";
            return RedirectToAction("EditarImagenes", new { id = id });
        }


        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarImagen(int id, string imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(imagenUrl))
            {
                TempData["ErrorImagenes"] = "No se indicó la imagen a eliminar.";
                return RedirectToAction("EditarImagenes", new { id = id });
            }

            imagenUrl = imagenUrl.Trim();
            var normalizar = new Func<string, string>(s =>
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                return s.Trim().Replace("~/", "/").Replace("\\", "/").TrimStart('/');
            });
            var imagenUrlNorm = normalizar(imagenUrl);
            var nombreArchivo = Path.GetFileName(imagenUrlNorm.Replace("/", "\\"));

            HabitacionesTabla habitaciones;
            List<string> imagenesList;
            string datosAnteriores, datosPosteriores;
            bool eliminada = false;

            using (var db = new Contexto())
            {
                habitaciones = db.HabitacionesTabla.Find(id);
                if (habitaciones == null)
                {
                    TempData["ErrorImagenes"] = "Habitación no encontrada.";
                    return RedirectToAction("EditarImagenes", new { id = id });
                }

                imagenesList = !string.IsNullOrWhiteSpace(habitaciones.UrlImagenes)
                    ? habitaciones.UrlImagenes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string>();

                datosAnteriores = string.Join(",", imagenesList);
                var indice = imagenesList.FindIndex(x => normalizar(x).Equals(imagenUrlNorm, StringComparison.OrdinalIgnoreCase));
                if (indice < 0 && !string.IsNullOrEmpty(nombreArchivo))
                    indice = imagenesList.FindIndex(x => string.Equals(Path.GetFileName(normalizar(x).Replace("/", "\\")), nombreArchivo, StringComparison.OrdinalIgnoreCase));
                if (indice >= 0)
                {
                    imagenesList.RemoveAt(indice);
                    habitaciones.UrlImagenes = string.Join(",", imagenesList);
                    db.Entry(habitaciones).Property(h => h.UrlImagenes).IsModified = true;
                    db.SaveChanges();
                    eliminada = true;
                }
                datosPosteriores = string.Join(",", imagenesList);
            }

            if (!eliminada)
            {
                TempData["ErrorImagenes"] = "No se encontró la imagen en la lista. Pruebe de nuevo.";
                return RedirectToAction("EditarImagenes", new { id = id });
            }

            try
            {
                var virtualPath = imagenUrl.StartsWith("~") ? imagenUrl : "~" + imagenUrl.TrimStart('/');
                var rutaImagen = Server != null ? Server.MapPath(virtualPath) : Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, imagenUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
                if (!string.IsNullOrEmpty(rutaImagen) && System.IO.File.Exists(rutaImagen))
                    System.IO.File.Delete(rutaImagen);
            }
            catch { /* ignorar */ }

            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloHabitaciones",
                TipoDeEvento = "Eliminar imagen",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se eliminó una imagen de una habitación.",
                StackTrace = "no hubo error",
                DatosAnteriores = $@"{{ ""IdHabitacion"": {habitaciones.IdHabitacion}, ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"", ""UrlImagenes"": ""{datosAnteriores}"" }}",
                DatosPosteriores = $@"{{ ""IdHabitacion"": {habitaciones.IdHabitacion}, ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"", ""UrlImagenes"": ""{datosPosteriores}"" }}",
                Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
            };

            await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

            TempData["MensajeImagenes"] = "Imagen eliminada.";
            return RedirectToAction("EditarImagenes", new { id = id });
        }


        [HttpPost]
        public ActionResult ToggleEstado(int IdHabitacion, string Estado, string DescripcionMantenimiento = null, string Tipo = "habitacion")
        {
            string estadoAnterior = "";
            string numeroHabitacion = "";
            
            try
            {
                // Obtener datos de la habitación usando SQL directo
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    // Obtener datos de la habitación
                    using (var selectCommand = new System.Data.SqlClient.SqlCommand(@"
                        SELECT IdHabitacion, Estado, NumeroHabitacion
                        FROM Habitaciones 
                        WHERE IdHabitacion = @IdHabitacion", connection))
                    {
                        selectCommand.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                                {
                                    return Json(new { success = false, message = "Habitación no encontrada" });
                                }
                                return RedirectToAction("IndexHabitaciones");
                            }
                            
                            estadoAnterior = reader.IsDBNull(reader.GetOrdinal("Estado")) ? "" : reader.GetString(reader.GetOrdinal("Estado"));
                            numeroHabitacion = reader.IsDBNull(reader.GetOrdinal("NumeroHabitacion")) ? IdHabitacion.ToString() : reader.GetString(reader.GetOrdinal("NumeroHabitacion"));
                        }
                    }
                    
                    // Actualizar estado usando SQL directo en transacción
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Actualizar estado de la habitación usando SQL directo
                            using (var updateHabitacion = new System.Data.SqlClient.SqlCommand(@"
                                UPDATE Habitaciones 
                                SET Estado = @Estado
                                WHERE IdHabitacion = @IdHabitacion", connection, transaction))
                            {
                                updateHabitacion.Parameters.AddWithValue("@Estado", Estado);
                                updateHabitacion.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                                int rowsHabitacion = updateHabitacion.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"=== ACTUALIZACIÓN DE ESTADO ===");
                                System.Diagnostics.Debug.WriteLine($"Habitación ID: {IdHabitacion}");
                                System.Diagnostics.Debug.WriteLine($"Estado anterior: {estadoAnterior}");
                                System.Diagnostics.Debug.WriteLine($"Estado nuevo: {Estado}");
                                System.Diagnostics.Debug.WriteLine($"Filas afectadas: {rowsHabitacion}");
                                
                                if (rowsHabitacion == 0)
                                {
                                    throw new Exception("No se pudo actualizar la habitación");
                                }
                            }
                            
                            // Si se cambió a Mantenimiento, eliminar cualquier solicitud de limpieza activa para esa habitación
                            if (Estado == "Mantenimiento")
                            {
                                try
                                {
                                    using (var deleteLimpieza = new System.Data.SqlClient.SqlCommand(@"
                                        UPDATE SolicitudesLimpieza 
                                        SET Estado = 'Completado'
                                        WHERE idHabitacion = @IdHabitacion 
                                          AND LOWER(LTRIM(RTRIM(Estado))) IN ('pendiente', 'en proceso')", connection, transaction))
                                    {
                                        deleteLimpieza.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                                        int rowsLimpieza = deleteLimpieza.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine($"Solicitudes de limpieza completadas para habitación {IdHabitacion}: {rowsLimpieza}");
                                    }
                                }
                                catch (Exception exLimpieza)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al completar solicitudes de limpieza: {exLimpieza.Message}");
                                    // Continuar aunque falle
                                }
                            }
                            
                            // Si se cambió a Disponible, completar cualquier mantenimiento activo para esa habitación
                            if (Estado == "Disponible")
                            {
                                try
                                {
                                    using (var completarMantenimiento = new System.Data.SqlClient.SqlCommand(@"
                                        UPDATE Mantenimiento 
                                        SET Estado = 'Completado'
                                        WHERE idHabitacion = @IdHabitacion 
                                          AND LOWER(LTRIM(RTRIM(Estado))) IN ('pendiente', 'en proceso')", connection, transaction))
                                    {
                                        completarMantenimiento.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                                        int rowsMantenimiento = completarMantenimiento.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine($"Mantenimientos completados para habitación {IdHabitacion}: {rowsMantenimiento}");
                                    }
                                }
                                catch (Exception exMantenimiento)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al completar mantenimientos: {exMantenimiento.Message}");
                                    // Continuar aunque falle
                                }
                            }
                            
                            // Si se cambió de "Ocupado" a "Disponible", crear automáticamente solicitud de limpieza usando SQL directo
                            if (estadoAnterior == "Ocupado" && Estado == "Disponible")
                            {
                                try
                                {
                                    // Crear solicitud de limpieza usando SQL directo para evitar problemas de contexto asíncrono
                                    using (var insertLimpieza = new System.Data.SqlClient.SqlCommand(@"
                                        INSERT INTO SolicitudesLimpieza (Descripcion, Estado, idHabitacion, idDepartamento, DepartamentoNombre, FechaSolicitud)
                                        VALUES (@Descripcion, @Estado, @IdHabitacion, 0, NULL, @FechaSolicitud)", connection, transaction))
                                    {
                                        insertLimpieza.Parameters.AddWithValue("@Descripcion", "Limpieza automática: Habitación recién desalojada. Requiere limpieza general.");
                                        insertLimpieza.Parameters.AddWithValue("@Estado", "Pendiente");
                                        insertLimpieza.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                                        insertLimpieza.Parameters.AddWithValue("@FechaSolicitud", DateTime.Now);
                                        insertLimpieza.ExecuteNonQuery();
                                        System.Diagnostics.Debug.WriteLine("Solicitud de limpieza creada automáticamente usando SQL directo");
                                    }
                                }
                                catch (Exception exLimpieza)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al crear solicitud de limpieza: {exLimpieza.Message}");
                                    // Continuar aunque falle la creación de limpieza
                                }
                            }
                            
                            // Si se cambió a "Mantenimiento" y hay descripción, crear registro de mantenimiento
                            if (Estado == "Mantenimiento" && !string.IsNullOrWhiteSpace(DescripcionMantenimiento))
                            {
                                try
                                {
                                    // Verificar si la columna idHabitacion existe antes de insertar
                                    bool columnaExiste = false;
                                    using (var checkColumn = new System.Data.SqlClient.SqlCommand(@"
                                        SELECT COUNT(*) 
                                        FROM INFORMATION_SCHEMA.COLUMNS 
                                        WHERE TABLE_NAME = 'Mantenimiento' 
                                        AND COLUMN_NAME = 'idHabitacion'", connection, transaction))
                                    {
                                        columnaExiste = ((int)checkColumn.ExecuteScalar()) > 0;
                                    }
                                    
                                    if (Tipo.ToLower() == "departamento")
                                    {
                                        // Crear registro de mantenimiento para departamento
                                        using (var insertMantenimiento = new System.Data.SqlClient.SqlCommand(@"
                                            INSERT INTO Mantenimiento (Descripcion, Estado, idDepartamento, DepartamentoNombre" + 
                                            (columnaExiste ? ", idHabitacion" : "") + @")
                                            VALUES (@Descripcion, @Estado, @IdDepartamento, @DepartamentoNombre" + 
                                            (columnaExiste ? ", NULL" : "") + @")", connection, transaction))
                                        {
                                            insertMantenimiento.Parameters.AddWithValue("@Descripcion", DescripcionMantenimiento);
                                            insertMantenimiento.Parameters.AddWithValue("@Estado", "Pendiente");
                                            insertMantenimiento.Parameters.AddWithValue("@IdDepartamento", IdHabitacion);
                                            insertMantenimiento.Parameters.AddWithValue("@DepartamentoNombre", numeroHabitacion);
                                            insertMantenimiento.ExecuteNonQuery();
                                            System.Diagnostics.Debug.WriteLine($"Registro de mantenimiento creado para departamento {IdHabitacion} ({numeroHabitacion})");
                                        }
                                    }
                                    else
                                    {
                                        // Crear registro de mantenimiento para habitación
                                        if (columnaExiste)
                                        {
                                            using (var insertMantenimiento = new System.Data.SqlClient.SqlCommand(@"
                                                INSERT INTO Mantenimiento (Descripcion, Estado, idHabitacion, idDepartamento, DepartamentoNombre)
                                                VALUES (@Descripcion, @Estado, @IdHabitacion, NULL, NULL)", connection, transaction))
                                            {
                                                insertMantenimiento.Parameters.AddWithValue("@Descripcion", DescripcionMantenimiento);
                                                insertMantenimiento.Parameters.AddWithValue("@Estado", "Pendiente");
                                                insertMantenimiento.Parameters.AddWithValue("@IdHabitacion", IdHabitacion);
                                                insertMantenimiento.ExecuteNonQuery();
                                                System.Diagnostics.Debug.WriteLine($"Registro de mantenimiento creado para habitación {IdHabitacion}");
                                            }
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine("ADVERTENCIA: La columna idHabitacion no existe en la tabla Mantenimiento. Ejecute el script SQL para agregarla.");
                                        }
                                    }
                                }
                                catch (Exception exMantenimiento)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al crear registro de mantenimiento: {exMantenimiento.Message}");
                                    // Continuar aunque falle la creación de mantenimiento
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
                
                // Registrar en bitácora usando SQL directo para evitar problemas de contexto asíncrono
                try
                {
                    using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var insertBitacora = new System.Data.SqlClient.SqlCommand(@"
                            INSERT INTO bitacoraEventos (TablaDeEvento, TipoDeEvento, FechaDeEvento, DescripcionDeEvento, StackTrace, DatosAnteriores, DatosPosteriores, Usuario)
                            VALUES (@TablaDeEvento, @TipoDeEvento, @FechaDeEvento, @DescripcionDeEvento, @StackTrace, @DatosAnteriores, @DatosPosteriores, @Usuario)", connection))
                        {
                            string datosJson = $@"{{
                                ""IdHabitacion"": {IdHabitacion},
                                ""NumeroHabitacion"": ""{numeroHabitacion}"",
                                ""EstadoAnterior"": ""{estadoAnterior}"",
                                ""EstadoNuevo"": ""{Estado}""
                            }}";
                            
                            insertBitacora.Parameters.AddWithValue("@TablaDeEvento", "ModuloHabitaciones");
                            insertBitacora.Parameters.AddWithValue("@TipoDeEvento", "Cambiar Estado");
                            insertBitacora.Parameters.AddWithValue("@FechaDeEvento", DateTime.Now);
                            insertBitacora.Parameters.AddWithValue("@DescripcionDeEvento", $"Se actualizó el estado de la habitación {numeroHabitacion} de {estadoAnterior} a {Estado}.");
                            insertBitacora.Parameters.AddWithValue("@StackTrace", "Sin errores");
                            insertBitacora.Parameters.AddWithValue("@DatosAnteriores", datosJson);
                            insertBitacora.Parameters.AddWithValue("@DatosPosteriores", datosJson);
                            insertBitacora.Parameters.AddWithValue("@Usuario", User?.Identity?.Name ?? "Sistema");
                            insertBitacora.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception exBitacora)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al registrar en bitácora: {exBitacora.Message}");
                    // Continuar aunque falle la bitácora
                }

                // Si es una petición AJAX, devolver JSON
                if (Request.IsAjaxRequest() || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = $"Estado de habitación actualizado a {Estado}" });
                }
                
                return RedirectToAction("IndexHabitaciones");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ToggleEstado: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado de la habitacion.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Realiza el check-in de una habitación
        /// Actualiza el estado de la reserva y la habitación
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador, Colaborador")]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIn(int IdReserva)
        {
            try
            {
                var reserva = _contexto.ReservasTabla.FirstOrDefault(r => r.IdReserva == IdReserva);
                if (reserva == null)
                {
                    return Json(new { success = false, message = "Reserva no encontrada." });
                }

                // Validar que la reserva esté confirmada
                if (reserva.EstadoReserva != "Confirmada")
                {
                    return Json(new { success = false, message = "Solo se puede hacer check-in de reservas confirmadas." });
                }

                // Validar que la fecha de check-in sea hoy o anterior
                if (reserva.FechaInicio.HasValue && reserva.FechaInicio.Value.Date > DateTime.Now.Date)
                {
                    return Json(new { success = false, message = "No se puede hacer check-in antes de la fecha de inicio de la reserva." });
                }

                // Actualizar estado de la reserva
                string estadoAnterior = reserva.EstadoReserva;
                reserva.EstadoReserva = "En Proceso"; // O "Check-in Realizado"
                
                // Actualizar estado de la habitación
                var habitacion = _contexto.HabitacionesTabla.FirstOrDefault(h => h.IdHabitacion == reserva.IdHabitacion);
                if (habitacion != null)
                {
                    habitacion.Estado = "Ocupado";
                }

                // Usar la misma conexión del contexto para evitar "Login failed" (segunda conexión rechazada en hosting o local)
                var connection = _contexto.Database.Connection;
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                // Verificar si ya existe un check-in para esta reserva
                bool existeCheckIn = false;
                using (var checkCommand = connection.CreateCommand())
                {
                    checkCommand.CommandText = "SELECT COUNT(*) FROM CheckIn WHERE IdReserva = @IdReserva";
                    var pId = checkCommand.CreateParameter();
                    pId.ParameterName = "@IdReserva";
                    pId.Value = IdReserva;
                    checkCommand.Parameters.Add(pId);
                    existeCheckIn = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                }

                // Si no existe check-in, crearlo automáticamente
                if (!existeCheckIn)
                {
                    string cedulaCliente = "";
                    string telefonoCliente = "";
                    string direccionCliente = "";
                    string emailCliente = "";
                    int? idCliente = null;

                    if (!string.IsNullOrEmpty(reserva.IdCliente))
                    {
                        using (var clienteCommand = connection.CreateCommand())
                        {
                            clienteCommand.CommandText = @"
                                SELECT IdCliente, CedulaCliente, TelefonoCliente, DireccionCliente, EmailCliente
                                FROM Cliente
                                WHERE IdUsuario = @IdUsuario";
                            var pUsuario = clienteCommand.CreateParameter();
                            pUsuario.ParameterName = "@IdUsuario";
                            pUsuario.Value = reserva.IdCliente;
                            clienteCommand.Parameters.Add(pUsuario);
                            using (var clienteReader = clienteCommand.ExecuteReader())
                            {
                                if (clienteReader.Read())
                                {
                                    idCliente = clienteReader.GetInt32(0);
                                    cedulaCliente = clienteReader.IsDBNull(1) ? "" : clienteReader.GetString(1);
                                    telefonoCliente = clienteReader.IsDBNull(2) ? "" : clienteReader.GetString(2);
                                    direccionCliente = clienteReader.IsDBNull(3) ? "" : clienteReader.GetString(3);
                                    emailCliente = clienteReader.IsDBNull(4) ? "" : clienteReader.GetString(4);
                                }
                            }
                        }
                    }

                    using (var insertCommand = connection.CreateCommand())
                    {
                        insertCommand.CommandText = @"
                            INSERT INTO CheckIn (
                                IdReserva, IdCliente, IdHabitacion,
                                NombreCliente, CedulaCliente, TelefonoCliente, DireccionCliente,
                                FechaCheckIn, FechaCheckOut, NumeroAdultos, NumeroNinos,
                                Total, Estado, CodigoFolio
                            )
                            VALUES (
                                @IdReserva, @IdCliente, @IdHabitacion,
                                @NombreCliente, @CedulaCliente, @TelefonoCliente, @DireccionCliente,
                                @FechaCheckIn, @FechaCheckOut, @NumeroAdultos, @NumeroNinos,
                                @Total, @Estado, @CodigoFolio
                            )";
                        var param = insertCommand.CreateParameter(); param.ParameterName = "@IdReserva"; param.Value = IdReserva; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@IdCliente"; param.Value = (object)idCliente ?? DBNull.Value; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@IdHabitacion"; param.Value = reserva.IdHabitacion; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@NombreCliente"; param.Value = reserva.NombreCliente ?? ""; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@CedulaCliente"; param.Value = cedulaCliente; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@TelefonoCliente"; param.Value = telefonoCliente; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@DireccionCliente"; param.Value = direccionCliente; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@FechaCheckIn"; param.Value = reserva.FechaInicio ?? DateTime.Now; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@FechaCheckOut"; param.Value = reserva.FechaFinal ?? DateTime.Now.AddDays(1); insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@NumeroAdultos"; param.Value = reserva.cantidadPersonas > 0 ? reserva.cantidadPersonas : 1; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@NumeroNinos"; param.Value = 0; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@Total"; param.Value = reserva.MontoTotal; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@Estado"; param.Value = "Check-In"; insertCommand.Parameters.Add(param);
                        param = insertCommand.CreateParameter(); param.ParameterName = "@CodigoFolio"; param.Value = IdReserva; insertCommand.Parameters.Add(param);
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Guardar cambios de forma síncrona para evitar problemas de contexto asíncrono
                try
                {
                    _contexto.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    var errorMessages = dbEx.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    throw new Exception("Error de validación: " + fullErrorMessage, dbEx);
                }

                // Registrar en bitácora
                string datosJson = $@"{{
                    ""IdReserva"": {reserva.IdReserva},
                    ""IdHabitacion"": {reserva.IdHabitacion},
                    ""EstadoAnterior"": ""{estadoAnterior}"",
                    ""EstadoNuevo"": ""En Proceso"",
                    ""Cliente"": ""{reserva.NombreCliente}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Check-in",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Check-in realizado para la reserva {IdReserva} - Cliente: {reserva.NombreCliente}",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = true, message = "Check-in realizado exitosamente." });
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al realizar check-in.",
                    StackTrace = ex.StackTrace ?? "No disponible",
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = false, message = "Error al realizar el check-in: " + ex.Message });
            }
        }

        /// <summary>
        /// Realiza el check-out de una habitación
        /// Actualiza el estado de la reserva y la habitación, y crea solicitud de limpieza
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador, Colaborador")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CheckOut(int IdReserva)
        {
            int idHabitacion = 0;
            string estadoAnteriorReserva = "";
            string nombreCliente = "";
            string numeroHabitacion = "";
            
            try
            {
                // Obtener datos de la reserva usando SQL directo para evitar problemas de contexto
                using (var connection = new System.Data.SqlClient.SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    
                    // Obtener datos de la reserva
                    using (var selectCommand = new System.Data.SqlClient.SqlCommand(@"
                        SELECT IdReserva, IdHabitacion, EstadoReserva, NombreCliente
                        FROM Reservas 
                        WHERE IdReserva = @IdReserva", connection))
                    {
                        selectCommand.Parameters.AddWithValue("@IdReserva", IdReserva);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return Json(new { success = false, message = "Reserva no encontrada." });
                            }
                            
                            idHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion"));
                            estadoAnteriorReserva = reader.GetString(reader.GetOrdinal("EstadoReserva"));
                            nombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? "" : reader.GetString(reader.GetOrdinal("NombreCliente"));
                            
                            // Validar que la reserva esté en proceso o confirmada
                            if (estadoAnteriorReserva != "En Proceso" && estadoAnteriorReserva != "Confirmada")
                            {
                                return Json(new { success = false, message = "Solo se puede hacer check-out de reservas en proceso o confirmadas." });
                            }
                        }
                    }
                    
                    // Obtener número de habitación
                    using (var selectHabitacion = new System.Data.SqlClient.SqlCommand(@"
                        SELECT NumeroHabitacion 
                        FROM Habitaciones 
                        WHERE IdHabitacion = @IdHabitacion", connection))
                    {
                        selectHabitacion.Parameters.AddWithValue("@IdHabitacion", idHabitacion);
                        var result = selectHabitacion.ExecuteScalar();
                        numeroHabitacion = result != null && result != DBNull.Value ? result.ToString() : idHabitacion.ToString();
                    }
                    
                    // Crear solicitud de limpieza automática PRIMERO
                    try
                    {
                        var solicitudLimpieza = new HotelPrado.Abstracciones.Modelos.SolicitudLimpieza.SolicitudLimpiezaDTO
                        {
                            Descripcion = $"Limpieza automática: Habitación {numeroHabitacion} recién desalojada después del check-out. Requiere limpieza general.",
                            Estado = "Pendiente",
                            idHabitacion = idHabitacion,
                            HabitacionNombre = "Habitación " + numeroHabitacion,
                            idDepartamento = 0,
                            FechaSolicitud = DateTime.Now
                        };

                        var registrarSolicitudLimpiezaLN = new HotelPrado.LN.SolicitudLimpieza.Registrar.RegistrarSolicitudLimpiezaLN();
                        await registrarSolicitudLimpiezaLN.Guardar(solicitudLimpieza).ConfigureAwait(false);
                        System.Diagnostics.Debug.WriteLine("Solicitud de limpieza creada exitosamente");
                    }
                    catch (Exception exLimpieza)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al crear solicitud de limpieza: {exLimpieza.Message}");
                        // Continuar con el check-out aunque falle la creación de limpieza
                    }
                    
                    // Actualizar TODOS los cambios en una sola transacción SQL
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Actualizar CheckIn con FechaCheckOut
                            using (var updateCheckIn = new System.Data.SqlClient.SqlCommand(@"
                                UPDATE CheckIn 
                                SET FechaCheckOut = @FechaCheckOut,
                                    Estado = 'Check-Out'
                                WHERE IdReserva = @IdReserva 
                                  AND (FechaCheckOut IS NULL OR Estado = 'Check-In')", connection, transaction))
                            {
                                updateCheckIn.Parameters.AddWithValue("@FechaCheckOut", DateTime.Now);
                                updateCheckIn.Parameters.AddWithValue("@IdReserva", IdReserva);
                                int rowsCheckIn = updateCheckIn.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"CheckIn actualizado: {rowsCheckIn} filas afectadas");
                            }
                            
                            // 2. Actualizar estado de la reserva a "Completada"
                            using (var updateReserva = new System.Data.SqlClient.SqlCommand(@"
                                UPDATE Reservas 
                                SET EstadoReserva = @EstadoReserva
                                WHERE IdReserva = @IdReserva", connection, transaction))
                            {
                                updateReserva.Parameters.AddWithValue("@EstadoReserva", "Completada");
                                updateReserva.Parameters.AddWithValue("@IdReserva", IdReserva);
                                int rowsReserva = updateReserva.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"Reserva actualizada: {rowsReserva} filas afectadas");
                                
                                if (rowsReserva == 0)
                                {
                                    throw new Exception("No se pudo actualizar la reserva");
                                }
                            }
                            
                            // 3. Actualizar estado de la habitación a "Limpieza"
                            using (var updateHabitacion = new System.Data.SqlClient.SqlCommand(@"
                                UPDATE Habitaciones 
                                SET Estado = @Estado
                                WHERE IdHabitacion = @IdHabitacion", connection, transaction))
                            {
                                updateHabitacion.Parameters.AddWithValue("@Estado", "Limpieza");
                                updateHabitacion.Parameters.AddWithValue("@IdHabitacion", idHabitacion);
                                int rowsHabitacion = updateHabitacion.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine($"Habitación actualizada: {rowsHabitacion} filas afectadas");
                                
                                if (rowsHabitacion == 0)
                                {
                                    throw new Exception("No se pudo actualizar la habitación");
                                }
                            }
                            
                            // Confirmar transacción
                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine("=== CHECK-OUT EXITOSO - TRANSACCIÓN COMMITEADA ===");
                        }
                        catch (Exception exTrans)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"Error en transacción, rollback ejecutado: {exTrans.Message}");
                            throw;
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"=== CHECK-OUT EXITOSO ===");
                System.Diagnostics.Debug.WriteLine($"Reserva ID: {IdReserva}");
                System.Diagnostics.Debug.WriteLine($"Habitación ID: {idHabitacion}");
                System.Diagnostics.Debug.WriteLine($"Estado reserva: {estadoAnteriorReserva} -> Completada");
                System.Diagnostics.Debug.WriteLine($"Estado habitación: -> Limpieza");

                // Registrar en bitácora
                string datosJson = $@"{{
                    ""IdReserva"": {IdReserva},
                    ""IdHabitacion"": {idHabitacion},
                    ""EstadoAnterior"": ""{estadoAnteriorReserva}"",
                    ""EstadoNuevo"": ""Completada"",
                    ""Cliente"": ""{nombreCliente}""
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Check-out",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Check-out realizado para la reserva {IdReserva} - Cliente: {nombreCliente}",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = true, message = "Check-out realizado exitosamente. Se creó automáticamente una solicitud de limpieza." });
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al realizar check-out.",
                    StackTrace = ex.StackTrace ?? "No disponible",
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return Json(new { success = false, message = "Error al realizar el check-out: " + ex.Message });
            }
        }

        /// <summary>
        /// Vista de calendario de habitaciones - Muestra todas las reservas en un calendario
        /// </summary>
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpGet]
        public ActionResult CalendarioHabitaciones()
        {
            try
            {
                ViewBag.Title = "Calendario de Habitaciones";
                var viewModel = _habitacionEstadoService.ObtenerEventosCalendario();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Registrar error en bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al obtener eventos del calendario: " + ex.Message,
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };
                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                
                // Retornar vista con modelo vacío en caso de error
                var viewModelError = new CalendarioHabitacionesViewModel
                {
                    Eventos = new List<EventoCalendario>(),
                    Habitaciones = new List<HabitacionInfo>()
                };
                return View(viewModelError);
            }
        }

        /// <summary>
        /// Vista de estado de habitaciones en tiempo real - Dashboard
        /// </summary>
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpGet]
        public ActionResult EstadoHabitaciones(DateTime? fecha = null)
        {
            try
            {
                ViewBag.Title = "Estado de Habitaciones";
                ViewBag.FechaPlano = fecha ?? DateTime.Today;
                var viewModel = _habitacionEstadoService.ObtenerEstadoHabitaciones(fecha);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Registrar error en bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al obtener estado de habitaciones: " + ex.Message,
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };
                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                
                // Retornar vista con modelo vacío en caso de error
                var viewModelError = new EstadoHabitacionesViewModel
                {
                    Habitaciones = new List<EstadoHabitacion>(),
                    Departamentos = new List<EstadoDepartamento>(),
                    Estadisticas = new EstadisticasResumen()
                };
                return View(viewModelError);
            }
        }

        /// <summary>
        /// Endpoint API para actualizar el estado en tiempo real (AJAX)
        /// </summary>
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpGet]
        public JsonResult ObtenerEstadoHabitacionesJson(DateTime? fecha = null)
        {
            var viewModel = _habitacionEstadoService.ObtenerEstadoHabitaciones(fecha);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Próximos eventos (reservas, check-in, check-out) de una habitación o departamento para los siguientes días.
        /// </summary>
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpGet]
        public JsonResult GetProximosEventosUnidad(int? idHabitacion, int? idDepartamento, int dias = 14)
        {
            if (!idHabitacion.HasValue && !idDepartamento.HasValue)
                return Json(new { ok = false, eventos = new List<object>() }, JsonRequestBehavior.AllowGet);
            var eventos = new List<object>();
            try
            {
                var listado = _listarReservasLN != null ? _listarReservasLN.Listar() : null;
                var reservas = listado != null ? listado : new List<ReservasDTO>();
                var desde = DateTime.Today;
                var hasta = desde.AddDays(dias);
                var filtradas = reservas.Where(r => r.FechaInicio.HasValue && r.FechaFinal.HasValue &&
                    (r.EstadoReserva == "Confirmada" || r.EstadoReserva == "Solicitada" || r.EstadoReserva == "En Proceso")).ToList();
                if (idHabitacion.HasValue)
                    filtradas = filtradas.Where(r => r.IdHabitacion == idHabitacion.Value).ToList();
                // Las reservas son por habitación; idDepartamento se deja para futura extensión
                foreach (var r in filtradas.OrderBy(r => r.FechaInicio))
                {
                    if (r.FechaInicio.Value.Date <= hasta && r.FechaFinal.Value.Date >= desde)
                    {
                        eventos.Add(new { fecha = r.FechaInicio.Value.ToString("dd/MM/yyyy"), tipo = "Check-in", descripcion = r.NombreCliente ?? "Reserva" });
                        eventos.Add(new { fecha = r.FechaFinal.Value.ToString("dd/MM/yyyy"), tipo = "Check-out", descripcion = r.NombreCliente ?? "Reserva" });
                    }
                }
            }
            catch { }
            return Json(new { ok = true, eventos }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Resumen de ocupación y reservas de todas las unidades (habitaciones y departamentos) para los próximos N días.
        /// Permite ver en un solo lugar qué está ocupado, qué tiene reserva y hacer check-out anticipado.
        /// </summary>
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpGet]
        public JsonResult GetResumenOcupacionProximosDias(int dias = 90)
        {
            var unidades = new List<object>();
            try
            {
                var viewModel = _habitacionEstadoService?.ObtenerEstadoHabitaciones(null) as EstadoHabitacionesViewModel;
                var listado = _listarReservasLN != null ? _listarReservasLN.Listar() : null;
                var reservas = listado != null ? listado : new List<ReservasDTO>();
                var desde = DateTime.Today;
                var hasta = desde.AddDays(dias);
                var reservasActivas = reservas.Where(r => r.FechaInicio.HasValue && r.FechaFinal.HasValue &&
                    (r.EstadoReserva == "Confirmada" || r.EstadoReserva == "Solicitada" || r.EstadoReserva == "En Proceso")).ToList();

                if (viewModel?.Habitaciones != null)
                {
                    foreach (var h in viewModel.Habitaciones)
                    {
                        var eventos = new List<object>();
                        var filtradas = reservasActivas.Where(r => r.IdHabitacion == h.IdHabitacion).ToList();
                        foreach (var r in filtradas.OrderBy(r => r.FechaInicio))
                        {
                            if (r.FechaInicio.Value.Date <= hasta && r.FechaFinal.Value.Date >= desde)
                            {
                                eventos.Add(new { fecha = r.FechaInicio.Value.ToString("dd/MM/yyyy"), tipo = "Check-in", descripcion = r.NombreCliente ?? "Reserva", idReserva = r.IdReserva });
                                eventos.Add(new { fecha = r.FechaFinal.Value.ToString("dd/MM/yyyy"), tipo = "Check-out", descripcion = r.NombreCliente ?? "Reserva", idReserva = r.IdReserva });
                            }
                        }
                        unidades.Add(new
                        {
                            tipo = "habitacion",
                            id = h.IdHabitacion,
                            numero = h.NumeroHabitacion.ToString(),
                            estadoActual = h.Estado,
                            estadoLeyenda = h.EstadoLeyenda ?? h.Estado,
                            idReserva = h.IdReserva,
                            clienteActual = h.ClienteActual ?? "",
                            fechaCheckOut = h.FechaCheckOut?.ToString("dd/MM/yyyy"),
                            eventos = eventos
                        });
                    }
                }
                if (viewModel?.Departamentos != null)
                {
                    foreach (var d in viewModel.Departamentos)
                    {
                        var eventos = new List<object>();
                        unidades.Add(new
                        {
                            tipo = "departamento",
                            id = d.IdDepartamento,
                            numero = d.NumeroDepartamento?.ToString() ?? "",
                            estadoActual = d.Estado,
                            estadoLeyenda = d.EstadoLeyenda ?? d.Estado,
                            idReserva = 0,
                            clienteActual = d.ClienteActual ?? "",
                            fechaCheckOut = (string)null,
                            eventos = eventos
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, unidades = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = true, unidades }, JsonRequestBehavior.AllowGet);
        }
    }
}
