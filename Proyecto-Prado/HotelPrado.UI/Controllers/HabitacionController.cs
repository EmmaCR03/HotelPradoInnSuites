using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.HabDisponibles;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoHabitacion.Listar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesHabitacion;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Habitaciones.Editar;
using HotelPrado.LN.Habitaciones.HabDisponibles;
using HotelPrado.LN.Habitaciones.Listar;
using HotelPrado.LN.Habitaciones.ObtenerPorId;
using HotelPrado.LN.Habitaciones.Registrar;
using HotelPrado.LN.TipoHabitacion.Listar;
using System;
using System.Collections.Generic;
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
        }

        // GET: Habitacion
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult IndexHabitaciones(int? capacidad, string estado)
        {
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitaciones = _listarHabitacionesLN.Listar(capacidad, estado);
            return View(laListaDeHabitaciones);
        }

        public ActionResult habitacionesInfo()
        {
            return View();
        }

        // GET: Habitacion/IndexHabitacionesUsuario
        [AllowAnonymous]
        public ActionResult IndexHabitacionesUsuario(DateTime check_in, DateTime check_out, int capacidad)
        {
            ViewBag.CheckIn = check_in;
            ViewBag.CheckOut = check_out;
            ViewBag.Title = "La Habitacion";
            var laListaDeHabitacionesDisponibles = _habDisponibles.ListarDisponibles(check_in, check_out, capacidad);

            // Validar que las fechas sean actuales o futuras
            if (check_in.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("check_in", "La fecha de Check-In debe ser actual o futura.");
            }

            if (check_out.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("check_out", "La fecha de Check-Out debe ser actual o futura.");
            }

            // Validar que Check-In sea menor que Check-Out
            if (check_in >= check_out)
            {
                ModelState.AddModelError("check_out", "La fecha de Check-Out debe ser mayor que la fecha de Check-In.");
            }

            // Si hay errores, regresar a la vista con los mensajes
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            // Si hay habitaciones disponibles, solo mostrar una
            if (laListaDeHabitacionesDisponibles.Any())
            {
                var habitacionDisponible = laListaDeHabitacionesDisponibles.First();
                return View(new List<HabitacionesDTO> { habitacionDisponible });
            }

            // Si no hay habitaciones disponibles, devolver la lista vacía
            return View(new List<HabitacionesDTO>());
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

            return View(hab);
        }


        public string GetUrlImagenesById(int id)
        {
            var urlImagenes = _contexto.HabitacionesTabla
                .Where(d => d.IdHabitacion == id)
                .Select(d => d.UrlImagenes)
                .FirstOrDefault();

            return urlImagenes; // Devuelve solo el campo UrlImagenes
        }



        // GET: Habitacion/Create
        public ActionResult Create()
        {


            return View();
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

                // Verificar si la carpeta "Uploads" existe, si no, crearla
                string uploadDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Verificar si hay archivos cargados
                if (Image != null && Image.Any())
                {
                    foreach (var file in Image)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            // Guardar el archivo en un directorio específico
                            string path = Path.Combine(uploadDirectory, Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                            // Agregar la URL del archivo a la lista
                            archivos.Add("/Uploads/" + Path.GetFileName(file.FileName));
                        }
                    }

                    // Aquí puedes agregar la lista de imágenes a tu modelo
                    modeloDeHabitaciones.UrlImagenes = string.Join(",", archivos);
                }

                // Guardar el departamento en la base de datos
                int cantidadDeDatosGuardados = await _registrarHabitacionesLN.Guardar(modeloDeHabitaciones);
                await _contexto.SaveChangesAsync();

                // Guardar las imágenes asociadas a esta habitacion
                foreach (var url in archivos) // Usa "archivos" para recorrer las URLs
                {
                    var imagen = new ImagenesHabitacionTabla
                    {
                        IdHabitacion = modeloDeHabitaciones.IdHabitacion,
                        UrlImagen = url
                    };

                    _contexto.ImagenesHabitacionesTabla.Add(imagen);
                }

                // Guardar cambios en la base de datos
                await _contexto.SaveChangesAsync();

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
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Disponible", Value = "Disponible" },
                new SelectListItem { Text = "En Mantenimiento", Value = "En Mantenimiento" },
                new SelectListItem { Text = "Ocupado", Value = "Ocupado" }
            };
            return View(datosHabitacion);
        }

        // POST: Habitacion/Edit/5
        [Authorize(Roles = "Administrador, Colaborador")]

        [HttpPost]
        public async Task<ActionResult> Edit(HabitacionesDTO lahabitacion, List<string> eliminarImagenes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtén la habitacion desde la base de datos usando su ID
                    var habitacion = _obtenerHabitacionesPorId.Obtener(lahabitacion.IdHabitacion);

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
                                // Asigna una ruta para almacenar la imagen
                                var ruta = $"/imagenes/{imagen.FileName}";

                                // Guarda la imagen en el servidor (en la carpeta wwwroot)
                                var filePath = Path.Combine(Server.MapPath("~"), "wwwroot", "imagenes", imagen.FileName);
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
                    ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                    return View(lahabitacion);
                }
            }

            return View(lahabitacion);
        }

        // GET: Departamento/Delete/5
        [Authorize(Roles = "Administrador, Colaborador")]

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Departamento/Delete/5
        [Authorize(Roles = "Administrador, Colaborador")]

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult EditarImagenes(int id)
        {
            var habitaciones = _listarHabitacionesLN.Listar().FirstOrDefault(d => d.IdHabitacion == id);

            if (habitaciones == null)
            {
                return HttpNotFound();
            }

            return View(habitaciones);
        }

        [Authorize(Roles = "Administrador, Colaborador")]

        [HttpPost]
        public ActionResult ActualizarImagenes(int id, IEnumerable<HttpPostedFileBase> imagenes)
        {
            var habitaciones = _contexto.HabitacionesTabla.Find(id);

            if (habitaciones == null)
            {
                return HttpNotFound();
            }

            var imagenUrls = new List<string>();

            // Mantener las imágenes existentes en la habitacion
            if (!string.IsNullOrEmpty(habitaciones.UrlImagenes))
            {
                imagenUrls.AddRange(habitaciones.UrlImagenes.Split(','));
            }

            var datosAnteriores = string.Join(",", imagenUrls); // Guardamos las URLs antes de la actualización

            if (imagenes != null)
            {
                foreach (var imagen in imagenes)
                {
                    if (imagen != null && imagen.ContentLength > 0)
                    {
                        var rutaCarpeta = Server.MapPath("~/Content/Imagenes");
                        if (!Directory.Exists(rutaCarpeta))
                        {
                            Directory.CreateDirectory(rutaCarpeta);
                        }

                        var filePath = Path.Combine(rutaCarpeta, Path.GetFileName(imagen.FileName));

                        // Guardar archivo
                        imagen.SaveAs(filePath);

                        // Guardar la URL relativa
                        imagenUrls.Add("/Content/Imagenes/" + Path.GetFileName(imagen.FileName));
                    }
                }

                // Actualiza el campo de UrlImagenes en el departamento
                habitaciones.UrlImagenes = string.Join(",", imagenUrls);
                _contexto.SaveChanges();
            }
            var datosPosteriores = string.Join(",", imagenUrls); // Guardamos las URLs después de la actualización

            // Registrar en la bitácora
            string datosJsonAnteriores = $@"
    {{
        ""IdDepartamento"": {habitaciones.IdHabitacion},
        ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"",
        ""UrlImagenes"": ""{datosAnteriores}""
    }}";

            string datosJsonPosteriores = $@"
    {{
        ""IdDepartamento"": {habitaciones.IdHabitacion},
        ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"",
        ""UrlImagenes"": ""{datosPosteriores}""
    }}";

            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloHabitaciones",
                TipoDeEvento = "Actualizar imágenes",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se actualizaron las imágenes de una habitacion.",
                StackTrace = "no hubo error",
                DatosAnteriores = datosJsonAnteriores,
                DatosPosteriores = datosJsonPosteriores
            };

            _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

            return RedirectToAction("IndexHabitaciones");
        }


        [HttpPost]
        public ActionResult EliminarImagen(int id, string imagenUrl)
        {
            var habitaciones = _contexto.HabitacionesTabla.Find(id);

            if (habitaciones == null || string.IsNullOrEmpty(imagenUrl))
            {
                return HttpNotFound();
            }

            // Convertir la lista de imágenes en un List<string>
            var imagenesList = habitaciones.UrlImagenes.Split(',').ToList();

            var datosAnteriores = string.Join(",", imagenesList); // Guardamos las URLs antes de la eliminación

            // Eliminar la imagen seleccionada
            imagenesList.Remove(imagenUrl);

            // Actualizar la cadena de imágenes en la base de datos
            habitaciones.UrlImagenes = string.Join(",", imagenesList);
            _contexto.SaveChanges();

            // Eliminar físicamente la imagen del servidor
            var rutaImagen = Server.MapPath(imagenUrl);
            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }

            var datosPosteriores = string.Join(",", imagenesList); // Guardamos las URLs después de la eliminación

            // Registrar en la bitácora
            string datosJsonAnteriores = $@"
    {{
        ""IdHabitacion"": {habitaciones.IdHabitacion},
        ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"",
        ""UrlImagenes"": ""{datosAnteriores}""
    }}";

            string datosJsonPosteriores = $@"
    {{
        ""IdHabitacion"": {habitaciones.IdHabitacion},
        ""NumeroHabitacion"": ""{habitaciones.NumeroHabitacion}"",
        ""UrlImagenes"": ""{datosPosteriores}""
    }}";

            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloHabitaciones",
                TipoDeEvento = "Eliminar imagen",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se eliminó una imagen de una imagen.",
                StackTrace = "no hubo error",
                DatosAnteriores = datosJsonAnteriores,
                DatosPosteriores = datosJsonPosteriores
            };

            _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

            return RedirectToAction("EditarImagenes", new { id = id });
        }


        [HttpPost]
        public ActionResult ToggleEstado(int IdHabitacion, string Estado)
        {
            try
            {
                var habitaciones = _contexto.HabitacionesTabla.FirstOrDefault(d => d.IdHabitacion == IdHabitacion);
                if (habitaciones != null)
                {
                    // Actualiza el estado con el valor recibido del formulario
                    habitaciones.Estado = Estado;
                    _contexto.SaveChanges();

                    string datosJson = $@"
           {{
                    ""IdHabitacion"": {habitaciones.IdHabitacion},
                    ""NumeroHabitacion"": {habitaciones.NumeroHabitacion},
                    ""PrecioPorNoche1P"": ""{habitaciones.PrecioPorNoche1P}"",
                    ""PrecioPorNoche2P"": ""{habitaciones.PrecioPorNoche2P}"",
                    ""PrecioPorNoche3P"": ""{habitaciones.PrecioPorNoche3P}"",
                    ""PrecioPorNoche4P"": ""{habitaciones.PrecioPorNoche4P}"",
                    ""CapacidadMax"": ""{habitaciones.CapacidadMax}"",
                    ""CapacidadMin"": ""{habitaciones.CapacidadMin}"",
                    ""Estado"": ""{habitaciones.Estado}""
                }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloHabitaciones",
                        TipoDeEvento = "Cambiar Estado",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de la habitacion.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexHabitaciones");
                }

                // Si no se encuentra el departamento, redirigir a IndexHabitaciones
                return RedirectToAction("IndexHabitaciones");
            }
            catch (Exception ex)
            {
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

    }
}
