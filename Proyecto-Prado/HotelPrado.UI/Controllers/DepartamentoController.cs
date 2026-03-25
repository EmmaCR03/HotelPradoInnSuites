using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.TipoDeDepartamentos;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.ImagenesDepartamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.AccesoADatos;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Departamentos.Editar;
using HotelPrado.LN.Departamentos.Listar;
using HotelPrado.LN.Departamentos.ObtenerPorId;
using HotelPrado.LN.Departamentos.Registrar;
using HotelPrado.LN.TipoDeDepartamento.Listar;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using HotelPrado.LN.SolicitudLimpieza.Registrar;
using HotelPrado.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace HotelPrado.UI.Controllers
{
    public class DepartamentoController : Controller
    {
        IListarDepartamentosLN _listarDepartamentosLN;
        IListarTipoDeDepartamenoLN _listarTipoDeDepartamentosLN;
        IRegistrarDepartamentoLN _registrarDepartamentosLN;
        Contexto _contexto;
        IEditarDepartamentosLN _editarDepartamentoLN;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IObtenerDepartamentoPorIdLN _obtenerDepartamentoPorId;
        private ApplicationUserManager _userManager;

        public DepartamentoController()
        {
            _listarDepartamentosLN = new ListarDepartamentoLN();
            _listarTipoDeDepartamentosLN = new ListarTipoDeDepartamenoLN();
            _registrarDepartamentosLN = new RegistrarDepartamentoLN();
            _contexto = new Contexto();
            _obtenerDepartamentoPorId = new ObtenerDepartamentoPorIdLN();
            _editarDepartamentoLN = new EditarDepartamentosLN();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Verifica si una excepción está relacionada con DisposableHttpContextWrapper
        /// </summary>
        private bool IsHttpContextWrapperException(Exception ex)
        {
            if (ex == null) return false;

            var exceptionType = ex.GetType().FullName ?? string.Empty;
            var exceptionMessage = ex.Message ?? string.Empty;
            var stackTrace = ex.StackTrace ?? string.Empty;

            // Verificar si es el error específico de DisposableHttpContextWrapper
            if (exceptionType.Contains("DisposableHttpContextWrapper") ||
                exceptionMessage.Contains("DisposableHttpContextWrapper") ||
                exceptionMessage.Contains("SwitchContext") ||
                stackTrace.Contains("DisposableHttpContextWrapper") ||
                stackTrace.Contains("SwitchContext"))
            {
                return true;
            }

            // Verificar excepciones internas
            if (ex.InnerException != null)
            {
                return IsHttpContextWrapperException(ex.InnerException);
            }

            return false;
        }

        // GET: Departamento
        [Authorize(Roles = "Administrador, Colaborador")]
        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult IndexDepartamentos()
        {
            var laListaDeDepartamentos = _listarDepartamentosLN.Listar();
            ViewBag.Title = "El Departamento";
            return View(laListaDeDepartamentos);
        }

        [AllowAnonymous]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult IndexDepartamentosClientes()
        {
            ViewBag.Title = "Explora Nuestros Departamentos";

            try
            {
                // Optimización máxima: SQL directo con NOLOCK para mejor rendimiento
                using (var contexto = new Contexto())
                {
                    try
                    {
                        // Cargar configuración de hero banner - Forzar recarga
                        contexto.Configuration.AutoDetectChangesEnabled = true;
                        var heroBanner = contexto.ConfiguracionHeroBannerTabla
                            .Where(c => c.Pagina == "Departamentos")
                            .OrderByDescending(c => c.FechaActualizacion)
                            .FirstOrDefault();
                        ViewBag.HeroBannerUrl = heroBanner?.UrlImagen ?? "/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg";
                    }
                    catch (Exception ex)
                    {
                        // Si la tabla no existe, usar valor por defecto
                        ViewBag.HeroBannerUrl = "/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg";
                        // Log del error para debug
                        System.Diagnostics.Debug.WriteLine("Error cargando hero banner: " + ex.Message);
                    }

                    try
                    {
                        // Cargar configuración de precios - Forzar recarga
                        contexto.Configuration.AutoDetectChangesEnabled = true;
                        var precioConfig = contexto.ConfiguracionPreciosDepartamentosTabla
                            .OrderByDescending(c => c.FechaActualizacion)
                            .FirstOrDefault();
                            
                        if (precioConfig != null)
                        {
                            ViewBag.PrecioBase = precioConfig.PrecioBase;
                            ViewBag.TextoPrecio = precioConfig.TextoPrecio ?? "Por mes";
                            // Mostrar precio siempre a menos que explícitamente esté desactivado
                            ViewBag.MostrarPrecio = precioConfig.MostrarPrecio;
                        }
                        else
                        {
                            // Por defecto, siempre mostrar precios
                            ViewBag.PrecioBase = 275000m;
                            ViewBag.TextoPrecio = "Por mes";
                            ViewBag.MostrarPrecio = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Si la tabla no existe, usar valores por defecto
                        ViewBag.PrecioBase = 275000m;
                        ViewBag.TextoPrecio = "Por mes";
                        ViewBag.MostrarPrecio = true;
                        // Log del error para debug
                        System.Diagnostics.Debug.WriteLine("Error cargando precios: " + ex.Message);
                    }
                    // Solo 1 o 2 habitaciones sin amueblar: una tarjeta por tipo (máximo 2 tarjetas)
                    var todos = contexto.Database.SqlQuery<DepartamentoDTO>(@"
                        SELECT 
                            d.IdDepartamento,
                            d.Nombre,
                            d.NumeroDepartamento,
                            d.Descripcion,
                            d.Precio,
                            d.Estado,
                            ISNULL(d.UrlImagenes, '') AS UrlImagenes,
                            d.IdTipoDepartamento,
                            ISNULL(t.NumeroHabitaciones, 0) AS NumeroHabitaciones,
                            ISNULL(t.Amueblado, 0) AS Amueblado,
                            ISNULL(d.NumeroEmpresa, '+50685406105') AS NumeroEmpresa,
                            ISNULL(d.CorreoEmpresa, 'info@pradoinn.com') AS CorreoEmpresa
                        FROM Departamento d WITH (NOLOCK)
                        LEFT JOIN TipoDepartamento t WITH (NOLOCK) ON d.IdTipoDepartamento = t.IdTipoDepartamento
                        WHERE d.Estado = 'Disponible'
                          AND ISNULL(t.NumeroHabitaciones, 0) IN (1, 2)
                          AND ISNULL(t.Amueblado, 0) = 0
                        ORDER BY t.NumeroHabitaciones, d.NumeroDepartamento
                    ").ToList();
                    // Una tarjeta por tipo: 1 habitación y 2 habitaciones
                    var departamentosDisponibles = todos
                        .GroupBy(d => d.NumeroHabitaciones)
                        .OrderBy(g => g.Key)
                        .Select(g => g.First())
                        .ToList();

                    // Galería para usuarios (Apartamentos, Instalaciones, Habitaciones): desde ConfiguracionHeroBanner, UrlImagen = "url|leyenda"
                    try
                    {
                        var listApartamentos = contexto.ConfiguracionHeroBannerTabla.Where(c => c.Pagina != null && c.Pagina.StartsWith("GaleriaApartamentos_")).ToList();
                        var galeriaApartamentos = listApartamentos.OrderBy(c => { var s = (c.Pagina ?? "").Replace("GaleriaApartamentos_", ""); int n; return int.TryParse(s, out n) ? n : 0; }).Select(c => { var u = c.UrlImagen ?? ""; var bar = u.IndexOf('|'); return Tuple.Create(bar >= 0 ? u.Substring(0, bar).Trim() : u, bar >= 0 ? u.Substring(bar + 1).Trim() : ""); }).ToList();
                        var listInstalaciones = contexto.ConfiguracionHeroBannerTabla.Where(c => c.Pagina != null && c.Pagina.StartsWith("GaleriaInstalaciones_")).ToList();
                        var galeriaInstalaciones = listInstalaciones.OrderBy(c => { var s = (c.Pagina ?? "").Replace("GaleriaInstalaciones_", ""); int n; return int.TryParse(s, out n) ? n : 0; }).Select(c => { var u = c.UrlImagen ?? ""; var bar = u.IndexOf('|'); return Tuple.Create(bar >= 0 ? u.Substring(0, bar).Trim() : u, bar >= 0 ? u.Substring(bar + 1).Trim() : ""); }).ToList();
                        var listHabitaciones = contexto.ConfiguracionHeroBannerTabla.Where(c => c.Pagina != null && c.Pagina.StartsWith("GaleriaHabitaciones_")).ToList();
                        var galeriaHabitaciones = listHabitaciones.OrderBy(c => { var s = (c.Pagina ?? "").Replace("GaleriaHabitaciones_", ""); int n; return int.TryParse(s, out n) ? n : 0; }).Select(c => { var u = c.UrlImagen ?? ""; var bar = u.IndexOf('|'); return Tuple.Create(bar >= 0 ? u.Substring(0, bar).Trim() : u, bar >= 0 ? u.Substring(bar + 1).Trim() : ""); }).ToList();
                        ViewBag.GaleriaApartamentos = galeriaApartamentos;
                        ViewBag.GaleriaInstalaciones = galeriaInstalaciones;
                        ViewBag.GaleriaHabitaciones = galeriaHabitaciones;
                    }
                    catch { ViewBag.GaleriaApartamentos = null; ViewBag.GaleriaInstalaciones = null; ViewBag.GaleriaHabitaciones = null; }

                    return View(departamentosDisponibles);
                }
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                System.Diagnostics.Debug.WriteLine($"Error en IndexDepartamentosClientes: {ex.Message}");
                
                // Fallback al método original si hay error
                try
                {
                    var laListaDeDepartamentos = _listarDepartamentosLN.Listar();
                    var filtrados = laListaDeDepartamentos
                        .Where(d => d.Estado == "Disponible"
                            && (d.NumeroHabitaciones == 1 || d.NumeroHabitaciones == 2)
                            && !d.Amueblado)
                        .Select(d => {
                            if (string.IsNullOrEmpty(d.NumeroEmpresa))
                                d.NumeroEmpresa = "+50685406105";
                            if (string.IsNullOrEmpty(d.CorreoEmpresa))
                                d.CorreoEmpresa = "info@pradoinn.com";
                            return d;
                        })
                        .ToList();
                    // Una tarjeta por tipo (máximo 2)
                    var departamentosDisponibles = filtrados
                        .GroupBy(d => d.NumeroHabitaciones)
                        .OrderBy(g => g.Key)
                        .Select(g => g.First())
                        .ToList();

                    return View(departamentosDisponibles);
                }
                catch
                {
                    // Si también falla el fallback, retornar lista vacía
                    return View(new List<DepartamentoDTO>());
                }
            }
        }


        // GET: Departamento/Details/5 (Para clientes)
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            DepartamentoDTO depto = _obtenerDepartamentoPorId.Obtener(id);
            if (depto == null)
            {
                return HttpNotFound();
            }

            depto.NumeroEmpresa = !string.IsNullOrEmpty(depto.NumeroEmpresa) ? depto.NumeroEmpresa : "+50685406105";
            depto.CorreoEmpresa = !string.IsNullOrEmpty(depto.CorreoEmpresa) ? depto.CorreoEmpresa : "info@pradoinn.com";

            // Asegurar que UrlImagenes no sea null ni vacío
            if (string.IsNullOrWhiteSpace(depto.UrlImagenes))
            {
                depto.UrlImagenes = ""; // Evita errores en la vista
            }

            return View(depto);
        }

        // GET: Departamento/DetallesAdmin/5 (Para administradores)
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult DetallesAdmin(int id)
        {
            DepartamentoDTO depto = _obtenerDepartamentoPorId.Obtener(id);
            if (depto == null)
            {
                return HttpNotFound();
            }

            // Optimizar: usar using para liberar recursos y AsNoTracking
            using (var contexto = new Contexto())
            {
                // Obtener información del cliente asignado si existe
                var departamentoTabla = contexto.DepartamentoTabla
                    .AsNoTracking()
                    .FirstOrDefault(d => d.IdDepartamento == id);
                    
                if (departamentoTabla != null && departamentoTabla.IdCliente.HasValue)
                {
                    try
                    {
                        // Optimizar: usar una sola consulta para obtener usuarios con rol Cliente
                        using (var dbContext = new ApplicationDbContext())
                        {
                            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
                            
                            if (roleManager.RoleExists("Cliente"))
                            {
                                var role = roleManager.FindByName("Cliente");
                                if (role != null)
                                {
                                    // Optimizar: obtener solo los IDs necesarios
                                    var userIdsInRole = dbContext.Set<IdentityUserRole>()
                                        .Where(ur => ur.RoleId == role.Id)
                                        .Select(ur => ur.UserId)
                                        .ToList();
                                    
                                    // Optimizar: buscar solo el cliente que coincide con el hash
                                    var usuariosConRolCliente = UserManager.Users
                                        .Where(u => userIdsInRole.Contains(u.Id))
                                        .ToList();
                                    
                                    // Buscar el cliente cuyo hash coincida con IdCliente
                                    foreach (var cliente in usuariosConRolCliente)
                                    {
                                        int hashId = Math.Abs(cliente.Id.GetHashCode());
                                        if (hashId == departamentoTabla.IdCliente.Value)
                                        {
                                            // Asignar el nombre completo del cliente
                                            depto.NombreCliente = !string.IsNullOrWhiteSpace(cliente.NombreCompleto) 
                                                ? cliente.NombreCompleto 
                                                : cliente.UserName;
                                            if (!string.IsNullOrWhiteSpace(cliente.Email))
                                            {
                                                depto.NombreCliente += " (" + cliente.Email + ")";
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Si hay un error al obtener el cliente, simplemente no mostrar el nombre
                        // No fallar toda la vista por esto
                        System.Diagnostics.Debug.WriteLine("Error al obtener nombre del cliente: " + ex.Message);
                    }
                }
            }

            // Asegurar que UrlImagenes no sea null ni vacío
            if (string.IsNullOrWhiteSpace(depto.UrlImagenes))
            {
                depto.UrlImagenes = "";
            }

            return View(depto);
        }


        public string GetUrlImagenesById(int id)
        {
            if (_contexto?.DepartamentoTabla == null)
            {
                return string.Empty;
            }

            var urlImagenes = _contexto.DepartamentoTabla
                .Where(d => d.IdDepartamento == id)
                .Select(d => d.UrlImagenes)
                .FirstOrDefault();

            return urlImagenes ?? string.Empty; // Devuelve string vacío si es null
        }


        // GET: Departamento/Create
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Create()
        {
            // Solo 1 o 2 habitaciones sin amueblar (opciones desde BD)
            var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                .Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && td.Amueblado == false)
                .Select(td => new
                {
                    Id = td.IdTipoDepartamento,
                    NumeroHabitaciones = td.NumeroHabitaciones,
                    Amueblado = td.Amueblado
                })
                .ToList();

            var tiposDepartamento = tiposDepartamentoDb
                .Select(td => new SelectListItem
                {
                    Value = td.Id.ToString(),
                    Text = $"{td.NumeroHabitaciones} habitación(es) - Sin amueblar"
                })
                .ToList();

            ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Value", "Text");

            // Crear una nueva instancia del modelo para evitar NullReferenceException
            var nuevoDepartamento = new DepartamentoDTO
            {
                Estado = "Disponible", // Valor por defecto
                UrlImagenes = "" // Inicializar como string vacío
            };

            return View(nuevoDepartamento);
        }
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartamentoDTO modeloDeDepartamento)
        {
            // Número de departamento: se acepta cualquier valor (sin restricción 11-20)
            if (ModelState.IsValid)
            {
                Console.WriteLine("IdTipoDepartamento recibido: " + modeloDeDepartamento.IdTipoDepartamento);

                // Si el valor es null o 0, el problema está en la vista
                if (modeloDeDepartamento.IdTipoDepartamento == null || modeloDeDepartamento.IdTipoDepartamento == 0)
                {
                    ModelState.AddModelError("IdTipoDepartamento", "Debe seleccionar un tipo de habitación.");
                    return View(modeloDeDepartamento);
                }



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
                    var listaTipo = _contexto.TipoDepartamentoTabla.Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && !td.Amueblado).Select(td => new SelectListItem { Value = td.IdTipoDepartamento.ToString(), Text = td.NumeroHabitaciones + " hab. - Sin amueblar" }).ToList();
                    ViewBag.TipoDepartamento = new SelectList(listaTipo, "Value", "Text");
                    return View(modeloDeDepartamento);
                }

                // Leer todos los archivos por índice (input multiple)
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
                            var listaTipo = _contexto.TipoDepartamentoTabla.Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && !td.Amueblado).Select(td => new SelectListItem { Value = td.IdTipoDepartamento.ToString(), Text = td.NumeroHabitaciones + " hab. - Sin amueblar" }).ToList();
                            ViewBag.TipoDepartamento = new SelectList(listaTipo, "Value", "Text");
                            return View(modeloDeDepartamento);
                        }
                    }
                }
                if (archivos.Count > 0)
                    modeloDeDepartamento.UrlImagenes = string.Join(",", archivos);

                // Guardar el departamento en la base de datos
                await _registrarDepartamentosLN.Guardar(modeloDeDepartamento);

                // Guardar las imágenes con un contexto nuevo (evita NullReference/HttpContext)
                if (archivos.Count > 0)
                {
                    using (var db = new Contexto())
                    {
                        var nuevoId = db.DepartamentoTabla.OrderByDescending(d => d.IdDepartamento).Select(d => d.IdDepartamento).FirstOrDefault();
                        foreach (var url in archivos)
                        {
                            db.ImagenesDepartamentoTabla.Add(new ImagenesDepartamentoTabla
                            {
                                IdDepartamento = nuevoId,
                                UrlImagen = url
                            });
                        }
                        await db.SaveChangesAsync();
                    }
                }

                return RedirectToAction("IndexDepartamentos");
            }

            // Si no es válido, regresamos el modelo para que se pueda corregir
            // Necesitamos recargar el ViewBag para que el dropdown funcione
            var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                .Select(td => new
                {
                    Id = td.IdTipoDepartamento,
                    NumeroHabitaciones = td.NumeroHabitaciones,
                    Amueblado = td.Amueblado
                })
                .ToList();

            var tiposDepartamento = tiposDepartamentoDb
                .Select(td => new SelectListItem
                {
                    Value = td.Id.ToString(),
                    Text = $"{td.NumeroHabitaciones} habitación(es) - {(td.Amueblado ? "Amueblado" : "No amueblado")}"
                })
                .ToList();

            ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Value", "Text");
            
            return View(modeloDeDepartamento);
        }


        // GET: Departamento/Edit/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Edit(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("IndexDepartamentos");
            var eldepartamento = _obtenerDepartamentoPorId.Obtener(id);
            if (eldepartamento == null)
                return HttpNotFound();

            // Solo 1 y 2 habitaciones sin amueblar (igual que Create)
            var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                .Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && td.Amueblado == false)
                .Select(td => new { Id = td.IdTipoDepartamento, NumeroHabitaciones = td.NumeroHabitaciones })
                .ToList();
            var tiposDepartamento = tiposDepartamentoDb
                .Select(td => new { Id = td.Id, Detalle = $"{td.NumeroHabitaciones} habitación(es) - Sin amueblar" })
                .ToList();
            ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Id", "Detalle");

            return View(eldepartamento);
        }


        // POST: Departamento/Edit/5
        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartamentoDTO eldepartamento, List<string> eliminarImagenes)
        {
            // Quitar validación de campos que se rellenan en servidor o no van en el formulario
            ModelState.Remove("UrlImagenes");
            ModelState.Remove("NombreCliente");
            ModelState.Remove("NumeroEmpresa");
            ModelState.Remove("CorreoEmpresa");

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtén el departamento actual desde la base de datos usando su ID
                    var departamentoActual = _obtenerDepartamentoPorId.Obtener(eldepartamento.IdDepartamento);
                    
                    // Si el departamento no existe, retornar error
                    if (departamentoActual == null)
                    {
                        ViewBag.mensaje = "El departamento no fue encontrado.";
                        return RedirectToAction("IndexDepartamentos");
                    }
                    
                    // Preservar valores que no se envían en el formulario
                    if (string.IsNullOrWhiteSpace(eldepartamento.Nombre))
                        eldepartamento.Nombre = departamentoActual.Nombre;
                    if (string.IsNullOrWhiteSpace(eldepartamento.Descripcion))
                        eldepartamento.Descripcion = departamentoActual.Descripcion;
                    if (eldepartamento.IdTipoDepartamento == 0)
                        eldepartamento.IdTipoDepartamento = departamentoActual.IdTipoDepartamento;
                    if (string.IsNullOrWhiteSpace(eldepartamento.Estado))
                        eldepartamento.Estado = departamentoActual.Estado;
                    if (eldepartamento.Precio == 0)
                        eldepartamento.Precio = departamentoActual.Precio;
                    if (eldepartamento.NumeroHabitaciones == 0)
                        eldepartamento.NumeroHabitaciones = departamentoActual.NumeroHabitaciones;
                    
                    // Preservar otros campos importantes (NumeroDepartamento se edita en el formulario)
                    eldepartamento.IdCliente = departamentoActual.IdCliente;
                    
                    // Obtén el departamento desde la base de datos usando su ID (para imágenes)
                    var departamento = _obtenerDepartamentoPorId.Obtener(eldepartamento.IdDepartamento);

                    // Eliminar imágenes seleccionadas
                    if (eliminarImagenes != null && eliminarImagenes.Count > 0)
                    {
                        // Eliminar las imágenes de la lista del departamento
                        departamento.ListaImagenes = departamento.ListaImagenes.Except(eliminarImagenes).ToList();
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
                                departamento.ListaImagenes.Add(ruta);
                            }
                        }
                    }

                    // Guardar cambios en la base de datos (las URLs de las imágenes)
                    departamento.UrlImagenes = string.Join(",", departamento.ListaImagenes);
                    eldepartamento.UrlImagenes = departamento.UrlImagenes;

                    // Actualizar el departamento en la base de datos
                    int cantidadDeDatosActualizados = await _editarDepartamentoLN.Actualizar(eldepartamento);

                    if (cantidadDeDatosActualizados == 0)
                    {
                        ViewBag.mensaje = "Ocurrió un error inesperado, favor intente nuevamente.";
                        var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                            .Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && !td.Amueblado)
                            .Select(td => new { Id = td.IdTipoDepartamento, NumeroHabitaciones = td.NumeroHabitaciones })
                            .ToList();
                        var tiposDepartamento = tiposDepartamentoDb.Select(td => new { Id = td.Id, Detalle = $"{td.NumeroHabitaciones} habitación(es) - Sin amueblar" }).ToList();
                        ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Id", "Detalle");
                        return View(eldepartamento);
                    }

                    return RedirectToAction("IndexDepartamentos");
                }
                catch (Exception ex)
                {
                    var inner = ex;
                    while (inner.InnerException != null) inner = inner.InnerException;
                    var msg = ex.Message + (inner != ex ? " | Detalle: " + inner.Message : "");
                    ViewBag.mensaje = "Error al guardar: " + msg;
                    var tiposDepartamentoDb = _contexto.TipoDepartamentoTabla
                        .Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && !td.Amueblado)
                        .Select(td => new { Id = td.IdTipoDepartamento, NumeroHabitaciones = td.NumeroHabitaciones })
                        .ToList();
                    var tiposDepartamento = tiposDepartamentoDb.Select(td => new { Id = td.Id, Detalle = $"{td.NumeroHabitaciones} habitación(es) - Sin amueblar" }).ToList();
                    ViewBag.TipoDepartamento = new SelectList(tiposDepartamento, "Id", "Detalle");
                    return View(eldepartamento);
                }
            }

            var tiposDeptoDb = _contexto.TipoDepartamentoTabla
                .Where(td => (td.NumeroHabitaciones == 1 || td.NumeroHabitaciones == 2) && !td.Amueblado)
                .Select(td => new { Id = td.IdTipoDepartamento, NumeroHabitaciones = td.NumeroHabitaciones })
                .ToList();
            var tiposDepto = tiposDeptoDb.Select(td => new { Id = td.Id, Detalle = $"{td.NumeroHabitaciones} habitación(es) - Sin amueblar" }).ToList();
            ViewBag.TipoDepartamento = new SelectList(tiposDepto, "Id", "Detalle");
            return View(eldepartamento);
        }



        // GET: Departamento/Delete/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult Delete(int id)
        {
            var departamento = _obtenerDepartamentoPorId.Obtener(id);
            if (departamento == null)
                return HttpNotFound();
            return View(departamento);
        }

        // POST: Departamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var departamento = _obtenerDepartamentoPorId.Obtener(id);
            if (departamento == null)
                return HttpNotFound();

            try
            {
                var entidad = _contexto.DepartamentoTabla.Find(id);
                if (entidad != null)
                {
                    _contexto.DepartamentoTabla.Remove(entidad);
                    await _contexto.SaveChangesAsync();

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloDepartamento",
                        TipoDeEvento = "Eliminar departamento",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = $"Se eliminó el departamento {entidad.Nombre} (Id: {id}).",
                        StackTrace = "Sin errores",
                        DatosAnteriores = $@"{{ ""IdDepartamento"": {id}, ""Nombre"": ""{entidad.Nombre}"", ""Estado"": ""{entidad.Estado}"" }}",
                        DatosPosteriores = "{}",
                        Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
                    };
                    await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                }
                TempData["MensajeExito"] = "Departamento eliminado correctamente.";
                try { Response.RemoveOutputCacheItem(Url.Action("IndexDepartamentos", "Departamento")); } catch { }
                return RedirectToAction("IndexDepartamentos");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
            {
                ModelState.AddModelError("", "No se puede eliminar este departamento porque tiene reservas, mantenimientos, citas o otros registros relacionados.");
                return View(departamento);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(departamento);
            }
        }
        public ActionResult EditarImagenes(int id)
        {
            var departamento = _obtenerDepartamentoPorId.Obtener(id);

            if (departamento == null)
            {
                return HttpNotFound();
            }

            return View(departamento);
        }

        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarImagenes(int id, IEnumerable<HttpPostedFileBase> imagenes)
        {
            // Ruta Content/Imagenes: ruta física de la aplicación (carpeta del proyecto)
            string rutaCarpeta = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes");
            if (string.IsNullOrEmpty(rutaCarpeta))
                rutaCarpeta = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, "Content", "Imagenes");

            DepartamentoTabla departamento;
            var imagenUrls = new List<string>();
            string datosAnteriores = "";

            using (var db = new Contexto())
            {
                departamento = db.DepartamentoTabla.Find(id);
                if (departamento == null)
                    return HttpNotFound();

                if (!string.IsNullOrWhiteSpace(departamento.UrlImagenes))
                    imagenUrls.AddRange(departamento.UrlImagenes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                datosAnteriores = string.Join(",", imagenUrls);

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
                    var fileName = Path.GetFileName(imagen.FileName);
                    try
                    {
                        imagen.SaveAs(Path.Combine(rutaCarpeta, fileName));
                        imagenUrls.Add("/Content/Imagenes/" + fileName);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorImagenes"] = "Error al guardar '" + fileName + "': " + ex.Message + " (ruta: " + rutaCarpeta + ")";
                        return RedirectToAction("EditarImagenes", new { id = id });
                    }
                }
                departamento.UrlImagenes = string.Join(",", imagenUrls);
                db.Entry(departamento).Property(d => d.UrlImagenes).IsModified = true;
                db.SaveChanges();
            }

            var datosPosteriores = string.Join(",", imagenUrls);
            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloDepartamento",
                TipoDeEvento = "Actualizar imágenes",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se actualizaron las imágenes de un departamento.",
                StackTrace = "no hubo error",
                DatosAnteriores = $@"{{ ""IdDepartamento"": {departamento.IdDepartamento}, ""Nombre"": ""{departamento.Nombre}"", ""UrlImagenes"": ""{datosAnteriores}"" }}",
                DatosPosteriores = $@"{{ ""IdDepartamento"": {departamento.IdDepartamento}, ""Nombre"": ""{departamento.Nombre}"", ""UrlImagenes"": ""{datosPosteriores}"" }}",
                Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
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

            DepartamentoTabla departamento;
            List<string> imagenesList;
            string datosAnteriores, datosPosteriores;
            bool eliminada = false;

            using (var db = new Contexto())
            {
                departamento = db.DepartamentoTabla.Find(id);
                if (departamento == null)
                {
                    TempData["ErrorImagenes"] = "Departamento no encontrado.";
                    return RedirectToAction("EditarImagenes", new { id = id });
                }

                imagenesList = !string.IsNullOrWhiteSpace(departamento.UrlImagenes)
                    ? departamento.UrlImagenes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string>();
                datosAnteriores = string.Join(",", imagenesList);

                var indice = imagenesList.FindIndex(x => normalizar(x).Equals(imagenUrlNorm, StringComparison.OrdinalIgnoreCase));
                if (indice < 0 && !string.IsNullOrEmpty(nombreArchivo))
                    indice = imagenesList.FindIndex(x => string.Equals(Path.GetFileName(normalizar(x).Replace("/", "\\")), nombreArchivo, StringComparison.OrdinalIgnoreCase));
                if (indice >= 0)
                {
                    imagenesList.RemoveAt(indice);
                    departamento.UrlImagenes = string.Join(",", imagenesList);
                    db.Entry(departamento).Property(d => d.UrlImagenes).IsModified = true;
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
                var urlParaArchivo = imagenUrl.Contains("|") ? imagenUrl.Split(new[] { '|' }, 2)[0].Trim() : imagenUrl;
                var virtualPath = urlParaArchivo.StartsWith("~") ? urlParaArchivo : "~" + urlParaArchivo.TrimStart('/');
                var rutaImagen = Server != null ? Server.MapPath(virtualPath) : Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory, urlParaArchivo.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
                if (!string.IsNullOrEmpty(rutaImagen) && System.IO.File.Exists(rutaImagen))
                    System.IO.File.Delete(rutaImagen);
            }
            catch { /* ignorar fallo al borrar archivo físico */ }

            var bitacora = new BitacoraEventosDTO
            {
                IdEvento = 0,
                TablaDeEvento = "ModuloDepartamento",
                TipoDeEvento = "Eliminar imagen",
                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                DescripcionDeEvento = "Se eliminó una imagen de un departamento.",
                StackTrace = "no hubo error",
                DatosAnteriores = $@"{{ ""IdDepartamento"": {departamento.IdDepartamento}, ""Nombre"": ""{departamento.Nombre}"", ""UrlImagenes"": ""{datosAnteriores}"" }}",
                DatosPosteriores = $@"{{ ""IdDepartamento"": {departamento.IdDepartamento}, ""Nombre"": ""{departamento.Nombre}"", ""UrlImagenes"": ""{datosPosteriores}"" }}",
                Usuario = User?.Identity?.IsAuthenticated == true ? User.Identity.GetUserName() : "Sistema"
            };
            await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

            TempData["MensajeImagenes"] = "Imagen eliminada.";
            return RedirectToAction("EditarImagenes", new { id = id });
        }

        [Authorize(Roles = "Administrador, Colaborador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActualizarLeyendas(int id, string[] urls, string[] leyendas)
        {
            if (urls == null || urls.Length == 0)
            {
                TempData["MensajeImagenes"] = "Descripciones guardadas.";
                return RedirectToAction("EditarImagenes", new { id = id });
            }
            using (var db = new Contexto())
            {
                var departamento = db.DepartamentoTabla.Find(id);
                if (departamento == null)
                    return HttpNotFound();
                var partes = new List<string>();
                for (int i = 0; i < urls.Length; i++)
                {
                    var url = (urls[i] ?? "").Trim();
                    if (string.IsNullOrEmpty(url)) continue;
                    var leyenda = (leyendas != null && i < leyendas.Length) ? (leyendas[i] ?? "").Trim() : "";
                    partes.Add(string.IsNullOrEmpty(leyenda) ? url : url + "|" + leyenda);
                }
                departamento.UrlImagenes = string.Join(",", partes);
                db.Entry(departamento).Property(d => d.UrlImagenes).IsModified = true;
                db.SaveChanges();
            }
            TempData["MensajeImagenes"] = "Descripciones de las imágenes guardadas.";
            return RedirectToAction("EditarImagenes", new { id = id });
        }

        [HttpPost]
        public async Task<ActionResult> ToggleEstado(int IdDepartamento, string Estado)
        {
            try
            {
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == IdDepartamento);
                if (departamento != null)
                {
                    // Guardar el estado anterior
                    string estadoAnterior = departamento.Estado;
                    
                    // Actualiza el estado con el valor recibido del formulario
                    departamento.Estado = Estado;
                    
                    // Si se cambia a "Disponible", limpiar el IdCliente
                    if (Estado == "Disponible")
                    {
                        departamento.IdCliente = null;
                    }
                    
                    _contexto.SaveChanges();

                    // Si se cambió de "Ocupado" a "Disponible", crear automáticamente solicitud de limpieza
                    if (estadoAnterior == "Ocupado" && Estado == "Disponible")
                    {
                        try
                        {
                            var solicitudLimpieza = new HotelPrado.Abstracciones.Modelos.SolicitudLimpieza.SolicitudLimpiezaDTO
                            {
                                Descripcion = "Limpieza automática: Departamento recién desalojado. Requiere limpieza general.",
                                Estado = "Pendiente",
                                idDepartamento = IdDepartamento,
                                DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString()),
                                idHabitacion = 0,
                                FechaSolicitud = DateTime.Now
                            };

                            var registrarSolicitudLimpiezaLN = new HotelPrado.LN.SolicitudLimpieza.Registrar.RegistrarSolicitudLimpiezaLN();
                            await registrarSolicitudLimpiezaLN.Guardar(solicitudLimpieza).ConfigureAwait(false);
                        }
                        catch (Exception exLimpieza)
                        {
                            // Si falla la creación automática, solo registrar en bitácora pero no fallar el cambio de estado
                            var bitacoraError = new BitacoraEventosDTO
                            {
                                IdEvento = 0,
                                TablaDeEvento = "ModuloDepartamento",
                                TipoDeEvento = "Error",
                                FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                                DescripcionDeEvento = "Error al crear solicitud de limpieza automática al desocupar departamento.",
                                StackTrace = exLimpieza.StackTrace ?? "No disponible",
                                DatosAnteriores = "NA",
                                DatosPosteriores = "NA"
                            };
                            _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);
                        }
                    }

                    string datosJson = $@"
            {{
                ""IdDepartamento"": {departamento.IdDepartamento},
                ""Nombre"": ""{departamento.Nombre}"",
                ""IdTipoDepartamento"": {departamento.IdTipoDepartamento},
                ""Precio"": ""{departamento.Precio}"",
                ""Estado"": ""{departamento.Estado}"",
                ""IdCliente"": ""{departamento.IdCliente}""
            }}";

                    var bitacora = new BitacoraEventosDTO
                    {
                        IdEvento = 0,
                        TablaDeEvento = "ModuloDepartamento",
                        TipoDeEvento = "Cambiar Estado",
                        FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                        DescripcionDeEvento = "Se actualizó el estado de un departamento.",
                        StackTrace = "no hubo error",
                        DatosAnteriores = datosJson,
                        DatosPosteriores = datosJson
                    };

                    _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                    return RedirectToAction("IndexDepartamentos");
                }

                // Si no se encuentra el departamento, redirigir a IndexDepartamentos
                return RedirectToAction("IndexDepartamentos");
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al actualizar el estado del departamento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Departamento/AsignarCliente/5
        [Authorize(Roles = "Administrador, Colaborador")]
        public ActionResult AsignarCliente(int id)
        {
            var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == id);
            if (departamento == null)
            {
                return HttpNotFound();
            }

            // Obtener lista de usuarios con rol "Cliente" para el dropdown
            var usuariosConRolCliente = new List<ApplicationUser>();
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                if (roleManager.RoleExists("Cliente"))
                {
                    var role = roleManager.FindByName("Cliente");
                    if (role != null)
                    {
                        var dbContext = new ApplicationDbContext();
                        var userIdsInRole = dbContext.Set<IdentityUserRole>()
                            .Where(ur => ur.RoleId == role.Id)
                            .Select(ur => ur.UserId)
                            .ToList();
                        
                        usuariosConRolCliente = UserManager.Users
                            .Where(u => userIdsInRole.Contains(u.Id))
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener usuarios con rol Cliente: " + ex.Message);
            }

            // Ordenar clientes por nombre
            usuariosConRolCliente = usuariosConRolCliente
                .OrderBy(u => !string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.NombreCompleto : u.UserName)
                .ToList();

            // Pasar la lista completa de clientes a la vista para mostrar en cards
            ViewBag.Clientes = usuariosConRolCliente;
            ViewBag.Departamento = departamento;
            ViewBag.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());

            // Obtener información del cliente actual si existe
            // Buscar el cliente actual usando el hash almacenado en IdCliente
            ViewBag.ClienteActual = null;
            if (departamento.IdCliente.HasValue)
            {
                // Buscar el cliente cuyo hash coincida con IdCliente
                // Nota: Esto es una solución temporal. Lo ideal sería cambiar IdCliente a string en la BD.
                foreach (var cliente in usuariosConRolCliente)
                {
                    int hashId = Math.Abs(cliente.Id.GetHashCode());
                    if (hashId == departamento.IdCliente.Value)
                    {
                        ViewBag.ClienteActual = cliente;
                        break;
                    }
                }
            }

            return View();
        }

        // POST: Departamento/AsignarCliente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Colaborador")]
        public async Task<ActionResult> AsignarCliente(int id, string IdCliente)
        {
            try
            {
                var departamento = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == id);
                if (departamento == null)
                {
                    return HttpNotFound();
                }

                // Guardar estado anterior para bitácora
                int? idClienteAnterior = departamento.IdCliente;
                string estadoAnterior = departamento.Estado;

                // Asignar o desasignar cliente
                if (!string.IsNullOrEmpty(IdCliente) && IdCliente != "0")
                {
                    // Verificar que el usuario con rol "Cliente" existe
                    var usuarioCliente = UserManager.FindById(IdCliente);
                    if (usuarioCliente == null)
                    {
                        ModelState.AddModelError("", "El cliente seleccionado no existe en el sistema.");
                        RecargarClientesEnViewBag(departamento, IdCliente);
                        return View();
                    }

                    // Verificar que el usuario tiene el rol "Cliente"
                    var rolesDelUsuario = UserManager.GetRoles(IdCliente);
                    if (!rolesDelUsuario.Contains("Cliente"))
                    {
                        ModelState.AddModelError("", "El usuario seleccionado no tiene el rol 'Cliente'.");
                        RecargarClientesEnViewBag(departamento, IdCliente);
                        return View();
                    }

                    // NOTA: IdCliente en DepartamentoTabla es int?, pero ApplicationUser.Id es string
                    // Por ahora, vamos a usar un hash del Id o almacenarlo de otra manera
                    // Opción 1: Usar un hash del Id del usuario (no ideal pero funcional)
                    // Opción 2: Cambiar el esquema para que IdCliente sea string (requiere cambios en BD)
                    // Por ahora, usaremos un valor temporal. En producción, deberías considerar cambiar el esquema.
                    
                    // Convertir el string Id a un hash numérico simple (solo para compatibilidad temporal)
                    // ADVERTENCIA: Esto es una solución temporal. Lo ideal sería cambiar IdCliente a string en la BD.
                    int hashId = Math.Abs(IdCliente.GetHashCode());
                    departamento.IdCliente = hashId; // Solución temporal
                    departamento.Estado = "Ocupado";
                    
                    // Almacenar el Id real del usuario en un campo adicional si existe, o usar otro método
                    // Por ahora, guardamos el hash. Para recuperar el usuario, necesitarías buscar por hash.
                }
                else
                {
                    // Desocupar departamento
                    departamento.IdCliente = null;
                    departamento.Estado = "Disponible";
                }

                // Intentar guardar cambios
                try
                {
                    // Forzar detección de cambios (por si AutoDetectChanges está deshabilitado en el contexto)
                    _contexto.ChangeTracker.DetectChanges();
                    var entry = _contexto.Entry(departamento);
                    entry.State = System.Data.Entity.EntityState.Modified;
                    entry.Property("IdCliente").IsModified = true;
                    entry.Property("Estado").IsModified = true;
                    
                    _contexto.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    // Si hay error de foreign key, mostrar mensaje claro
                    string errorMessage = "Error al guardar los cambios.";
                    if (ex.InnerException != null)
                    {
                        errorMessage += " " + ex.InnerException.Message;
                        // Si es error de foreign key, dar mensaje más específico
                        if (ex.InnerException.Message.Contains("FOREIGN KEY") || ex.InnerException.Message.Contains("FK_"))
                        {
                            errorMessage = "La base de datos tiene una restricción (FK_Departamento_Cliente) que impide guardar. " +
                                         "Ejecute en la base de datos el script: DB/dbo/Tables/EliminarFK_Departamento_Cliente.sql " +
                                         "y vuelva a intentar asignar el cliente.";
                        }
                    }
                    
                    ModelState.AddModelError("", errorMessage);
                    
                    // Recargar ViewBag y retornar vista con error
                    RecargarClientesEnViewBag(departamento, IdCliente ?? "0");
                    return View();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error inesperado al guardar: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine("Error completo: " + ex.ToString());
                    RecargarClientesEnViewBag(departamento, IdCliente ?? "0");
                    return View();
                }

                // Si se desocupó, crear automáticamente solicitud de limpieza
                if (estadoAnterior == "Ocupado" && departamento.Estado == "Disponible")
                {
                    try
                    {
                        var solicitudLimpieza = new HotelPrado.Abstracciones.Modelos.SolicitudLimpieza.SolicitudLimpiezaDTO
                        {
                            Descripcion = "Limpieza automática: Departamento recién desalojado. Requiere limpieza general.",
                            Estado = "Pendiente",
                            idDepartamento = id,
                            DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString()),
                            idHabitacion = 0,
                            FechaSolicitud = DateTime.Now
                        };

                        var registrarSolicitudLimpiezaLN = new HotelPrado.LN.SolicitudLimpieza.Registrar.RegistrarSolicitudLimpiezaLN();
                        await registrarSolicitudLimpiezaLN.Guardar(solicitudLimpieza);
                    }
                    catch (Exception exLimpieza)
                    {
                        // Registrar error pero no fallar
                        var bitacoraError = new BitacoraEventosDTO
                        {
                            IdEvento = 0,
                            TablaDeEvento = "ModuloDepartamento",
                            TipoDeEvento = "Error",
                            FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                            DescripcionDeEvento = "Error al crear solicitud de limpieza automática al desocupar departamento.",
                            StackTrace = exLimpieza.StackTrace ?? "No disponible",
                            DatosAnteriores = "NA",
                            DatosPosteriores = "NA"
                        };
                        _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);
                    }
                }

                // Registrar en bitácora
                string datosJson = $@"
            {{
                ""IdDepartamento"": {departamento.IdDepartamento},
                ""Nombre"": ""{departamento.Nombre}"",
                ""Estado"": ""{departamento.Estado}"",
                ""IdClienteAnterior"": ""{idClienteAnterior}"",
                ""IdClienteNuevo"": ""{departamento.IdCliente}""
            }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamento",
                    TipoDeEvento = !string.IsNullOrEmpty(IdCliente) && IdCliente != "0" ? "Asignar Cliente" : "Desocupar Departamento",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = !string.IsNullOrEmpty(IdCliente) && IdCliente != "0"
                        ? "Se asignó un cliente al departamento." 
                        : "Se desocupó el departamento.",
                    StackTrace = "no hubo error",
                    DatosAnteriores = datosJson,
                    DatosPosteriores = datosJson
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return RedirectToAction("DetallesAdmin", new { id = id });
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al asignar cliente al departamento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                ModelState.AddModelError("", "Error al asignar el cliente: " + ex.Message);
                
                var departamentoError = _contexto.DepartamentoTabla.FirstOrDefault(d => d.IdDepartamento == id);
                if (departamentoError != null)
                {
                    RecargarClientesEnViewBag(departamentoError, IdCliente ?? "0");
                }
                
                return View();
            }
        }

        /// <summary>
        /// Método auxiliar para recargar los clientes en ViewBag
        /// </summary>
        private void RecargarClientesEnViewBag(HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento.DepartamentoTabla departamento, string idClienteSeleccionado)
        {
            // Obtener lista de usuarios con rol "Cliente"
            var usuariosConRolCliente = new List<ApplicationUser>();
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                if (roleManager.RoleExists("Cliente"))
                {
                    var role = roleManager.FindByName("Cliente");
                    if (role != null)
                    {
                        var dbContext = new ApplicationDbContext();
                        var userIdsInRole = dbContext.Set<IdentityUserRole>()
                            .Where(ur => ur.RoleId == role.Id)
                            .Select(ur => ur.UserId)
                            .ToList();
                        
                        usuariosConRolCliente = UserManager.Users
                            .Where(u => userIdsInRole.Contains(u.Id))
                            .OrderBy(u => !string.IsNullOrWhiteSpace(u.NombreCompleto) ? u.NombreCompleto : u.UserName)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener usuarios con rol Cliente: " + ex.Message);
            }

            // Pasar la lista completa de clientes a la vista para mostrar en cards
            ViewBag.Clientes = usuariosConRolCliente;
            ViewBag.Departamento = departamento;
            ViewBag.DepartamentoNombre = "Departamento " + (departamento.NumeroDepartamento == 0 ? "Sin número" : departamento.NumeroDepartamento.ToString());

            // Obtener información del cliente actual si existe
            ViewBag.ClienteActual = null;
            if (departamento.IdCliente.HasValue)
            {
                // Buscar el cliente cuyo hash coincida con IdCliente
                foreach (var cliente in usuariosConRolCliente)
                {
                    int hashId = Math.Abs(cliente.Id.GetHashCode());
                    if (hashId == departamento.IdCliente.Value)
                    {
                        ViewBag.ClienteActual = cliente;
                        break;
                    }
                }
            }
        }

    }
}
