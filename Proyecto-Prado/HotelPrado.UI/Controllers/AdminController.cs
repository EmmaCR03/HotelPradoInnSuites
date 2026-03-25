using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelPrado.AccesoADatos;
using HotelPrado.Abstracciones.Modelos.Configuracion;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Configuracion;
using HotelPrado.UI.Helpers;
using Microsoft.AspNet.Identity;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador, Colaborador")]
    public class AdminController : Controller
    {
        // GET: Admin/Configuraciones (caché 60 s para aligerar en host)
        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Configuraciones()
        {
            ViewBag.Title = "Panel de Configuraciones";
            return View();
        }

        // Páginas que deben poder configurarse siempre (si no existen en BD se insertan)
        private static readonly Dictionary<string, string> PaginasHeroPorDefecto = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Home", "/Img/images/img_3.jpg" },
            { "Habitaciones", "/Img/images/IMG_2.JPG" },
            { "Contacto", "/Img/images/Contactenos/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg" },
            { "About", "/Img/images/IMG_3.JPG" },
            { "Services", "/Img/images/Servicios/WhatsApp SIPEPE2025-12-11 at 3.27.00 PM.jpeg" },
            { "Departamentos", "/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg" },
            { "Login", "/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg" },
            { "Registro", "/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg" }
        };

        // GET: Admin/ConfigurarHeroBanners
        public ActionResult ConfigurarHeroBanners()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var existentes = contexto.ConfiguracionHeroBannerTabla.Select(c => c.Pagina).ToList();
                    foreach (var kv in PaginasHeroPorDefecto)
                    {
                        if (!existentes.Any(p => string.Equals(p, kv.Key, StringComparison.OrdinalIgnoreCase)))
                        {
                            contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                            {
                                Pagina = kv.Key,
                                UrlImagen = kv.Value,
                                FechaActualizacion = DateTime.Now
                            });
                            existentes.Add(kv.Key);
                        }
                    }
                    contexto.SaveChanges();

                    // Solo hero banners por página; la galería se configura en ConfigurarGaleriaUsuarios
                    var configuraciones = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && !c.Pagina.StartsWith("Galeria"))
                        .OrderBy(c => c.Pagina)
                        .ToList()
                        .Select(c => new ConfiguracionHeroBannerDTO
                        {
                            IdConfiguracion = c.IdConfiguracion,
                            Pagina = c.Pagina,
                            UrlImagen = c.UrlImagen,
                            FechaActualizacion = c.FechaActualizacion,
                            ActualizadoPor = c.ActualizadoPor
                        })
                        .ToList();

                    ViewBag.Title = "Configurar Hero Banners";
                    return View(configuraciones);
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Si la tabla no existe, mostrar mensaje
                if (ex.Message.Contains("no es válido") || ex.Message.Contains("Invalid object name"))
                {
                    ViewBag.ErrorMessage = "Las tablas de configuración no existen en la base de datos. Por favor, ejecute el script SQL: DB/dbo/Tables/CREAR_TABLAS_CONFIGURACION.sql";
                    ViewBag.Title = "Configurar Hero Banners";
                    return View(new List<ConfiguracionHeroBannerDTO>());
                }
                throw;
            }
        }

        // POST: Admin/SubirImagenHeroBanner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubirImagenHeroBanner(int id, HttpPostedFileBase imagen)
        {
            // En algunos navegadores el archivo no llega por el parámetro; intentar desde Request.Files
            if (imagen == null || imagen.ContentLength == 0)
            {
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    imagen = Request.Files["imagen"] ?? Request.Files[0];
                }
                if (imagen == null || imagen.ContentLength == 0)
                {
                    return Json(new { success = false, message = "Por favor seleccione una imagen. Si ya lo hizo, pruebe con otro navegador o recargue la página." });
                }
            }

            try
            {
                // Validar que sea una imagen
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".heic" };
                var originalFileName = imagen.FileName ?? "";
                var extension = System.IO.Path.GetExtension(originalFileName).ToLower();
                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    return Json(new { success = false, message = $"Formato de imagen no permitido. Use JPG, PNG, GIF, WEBP o HEIC. Extensión recibida: '{extension}'" });
                }

                // Crear carpeta si no existe
                string uploadDirectory = Server.MapPath("~/Img/hero-banners/");
                if (!System.IO.Directory.Exists(uploadDirectory))
                {
                    System.IO.Directory.CreateDirectory(uploadDirectory);
                }

                // Generar nombre único para evitar conflictos
                string fileName = $"hero_{id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                string filePath = System.IO.Path.Combine(uploadDirectory, fileName);
                
                // Guardar archivo
                imagen.SaveAs(filePath);

                // Obtener URL relativa
                string urlImagen = $"/Img/hero-banners/{fileName}";

                // Actualizar en base de datos (UPDATE directo para garantizar persistencia)
                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext) ?? "";
                var ahora = DateTime.Now;
                using (var contexto = new Contexto())
                {
                    var configuracion = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (configuracion == null)
                    {
                        return Json(new { success = false, message = "Configuración no encontrada" });
                    }
                    var pagina = configuracion.Pagina ?? "";

                    // Persistir con UPDATE directo para evitar fallos intermitentes del change tracker
                    var filas = contexto.Database.ExecuteSqlCommand(
                        "UPDATE ConfiguracionHeroBanner SET UrlImagen = @p0, FechaActualizacion = @p1, ActualizadoPor = @p2 WHERE IdConfiguracion = @p3",
                        urlImagen, ahora, userId, id);
                    if (filas == 0)
                    {
                        return Json(new { success = false, message = "No se pudo actualizar el registro. Intente de nuevo." });
                    }

                    // Invalidar caché del hero para que la página muestre la nueva imagen de inmediato
                    if (!string.IsNullOrEmpty(pagina))
                    {
                        System.Web.HttpRuntime.Cache.Remove("HeroBanner_" + pagina);
                        if (pagina == "Home" && Response != null)
                            Response.RemoveOutputCacheItem("/");
                    }
                }

                return Json(new { success = true, message = "Imagen subida y actualizada correctamente", urlImagen = urlImagen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al subir imagen: " + ex.Message });
            }
        }

        // POST: Admin/ActualizarHeroBanner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarHeroBanner(int id, string urlImagen)
        {
            if (string.IsNullOrWhiteSpace(urlImagen))
            {
                return Json(new { success = false, message = "La URL de la imagen es requerida" });
            }

            try
            {
                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext) ?? "";
                var ahora = DateTime.Now;
                using (var contexto = new Contexto())
                {
                    var configuracion = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (configuracion == null)
                    {
                        return Json(new { success = false, message = "Configuración no encontrada" });
                    }
                    var pagina = configuracion.Pagina ?? "";

                    // UPDATE directo para garantizar que siempre se persiste (evita fallos intermitentes)
                    var filas = contexto.Database.ExecuteSqlCommand(
                        "UPDATE ConfiguracionHeroBanner SET UrlImagen = @p0, FechaActualizacion = @p1, ActualizadoPor = @p2 WHERE IdConfiguracion = @p3",
                        urlImagen, ahora, userId, id);
                    if (filas == 0)
                    {
                        return Json(new { success = false, message = "No se pudo actualizar. Intente de nuevo." });
                    }

                    if (!string.IsNullOrEmpty(pagina))
                    {
                        System.Web.HttpRuntime.Cache.Remove("HeroBanner_" + pagina);
                        if (pagina == "Home" && Response != null)
                            Response.RemoveOutputCacheItem("/");
                    }
                    return Json(new { success = true, message = "Hero banner actualizado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }

        private const string GaleriaHomePrefijo = "GaleriaHome_";
        private const int GaleriaHomeMaxSlides = 20;
        private static readonly string[] GaleriaHomeInicial = { "GaleriaHome_1", "GaleriaHome_2", "GaleriaHome_3" };
        private static readonly string[] GaleriaHomeUrlsPorDefecto = {
            "/Img/images/IMG_5.JPG",
            "/Img/images/IMG_6.JPG",
            "/Img/images/img_1.JPG"
        };

        private static int ObtenerNumeroSlideGaleria(string pagina)
        {
            if (string.IsNullOrEmpty(pagina) || !pagina.StartsWith(GaleriaHomePrefijo)) return 0;
            int n;
            return int.TryParse(pagina.Substring(GaleriaHomePrefijo.Length), out n) ? n : 0;
        }

        // GET: Admin/ConfigurarGaleriaCarousel
        public ActionResult ConfigurarGaleriaCarousel()
        {
            ViewBag.Title = "Configurar galería de inicio";
            ViewBag.PuedeAgregar = true;
            try
            {
                using (var contexto = new Contexto())
                {
                    for (int i = 0; i < GaleriaHomeInicial.Length; i++)
                    {
                        var pagina = GaleriaHomeInicial[i];
                        if (!contexto.ConfiguracionHeroBannerTabla.Any(c => c.Pagina == pagina))
                        {
                            contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                            {
                                Pagina = pagina,
                                UrlImagen = GaleriaHomeUrlsPorDefecto[i],
                                FechaActualizacion = DateTime.Now
                            });
                        }
                    }
                    contexto.SaveChanges();

                    var list = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaHomePrefijo))
                        .ToList()
                        .OrderBy(c => ObtenerNumeroSlideGaleria(c.Pagina))
                        .Select(c => new ConfiguracionHeroBannerDTO
                        {
                            IdConfiguracion = c.IdConfiguracion,
                            Pagina = c.Pagina,
                            UrlImagen = c.UrlImagen,
                            FechaActualizacion = c.FechaActualizacion,
                            ActualizadoPor = c.ActualizadoPor
                        })
                        .ToList();

                    ViewBag.PuedeAgregar = list.Count < GaleriaHomeMaxSlides;
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                var msg = (ex.InnerException != null ? ex.InnerException.Message + " " : "") + ex.Message;
                // Quitar rutas del servidor del mensaje por seguridad
                var msgSeguro = msg;
                if (msgSeguro.Length > 400) msgSeguro = msgSeguro.Substring(0, 400) + "...";
                if (msg.IndexOf("Invalid object name", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("no es válido", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("does not exist", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("invalid object", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessage = "La tabla ConfiguracionHeroBanner no existe en la base de datos del servidor. Ejecute el script CREAR_TABLAS_CONFIGURACION.sql en la base de datos del hosting.";
                }
                else if (msg.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("connection", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("conexión", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("network", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessage = "No se pudo conectar a la base de datos. Verifique la cadena de conexión en el hosting y que el servidor de base de datos esté accesible.";
                }
                else if (msg.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("duplicat", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessage = "Conflicto de datos (valor duplicado en la tabla). Si la tabla ya tiene filas para la galería, puede ignorar este mensaje y recargar la página.";
                }
                else if (msg.IndexOf("NULL", StringComparison.OrdinalIgnoreCase) >= 0
                    || msg.IndexOf("Cannot insert", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ViewBag.ErrorMessage = "Error al guardar en la base de datos (falta un valor obligatorio). Asegúrese de que la tabla ConfiguracionHeroBanner tenga las columnas: IdConfiguracion, Pagina, UrlImagen, FechaActualizacion, ActualizadoPor (nullable). Detalle: " + msgSeguro;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se pudo cargar la galería. Detalle del servidor: " + msgSeguro;
                }
                return View(new List<ConfiguracionHeroBannerDTO>());
            }
        }

        // POST: Admin/AgregarSlideGaleriaCarousel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarSlideGaleriaCarousel()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var existentes = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaHomePrefijo))
                        .Select(c => c.Pagina)
                        .ToList();
                    int maxNum = 0;
                    foreach (var p in existentes)
                    {
                        int n = ObtenerNumeroSlideGaleria(p);
                        if (n > maxNum) maxNum = n;
                    }
                    if (existentes.Count >= GaleriaHomeMaxSlides)
                    {
                        TempData["ErrorMessage"] = "Se ha alcanzado el máximo de " + GaleriaHomeMaxSlides + " imágenes.";
                        return RedirectToAction("ConfigurarGaleriaCarousel");
                    }
                    var nuevaPagina = GaleriaHomePrefijo + (maxNum + 1);
                    contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                    {
                        Pagina = nuevaPagina,
                        UrlImagen = "/Img/images/img_1.JPG",
                        FechaActualizacion = DateTime.Now
                    });
                    contexto.SaveChanges();
                }
                System.Web.HttpRuntime.Cache.Remove("GaleriaCarousel_Home");
                TempData["SuccessMessage"] = "Se agregó una nueva imagen. Configúrala y guarda.";
                return RedirectToAction("ConfigurarGaleriaCarousel");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al agregar: " + ex.Message;
                return RedirectToAction("ConfigurarGaleriaCarousel");
            }
        }

        // POST: Admin/QuitarSlideGaleriaCarousel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarSlideGaleriaCarousel(int id)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var c = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (c != null && c.Pagina != null && c.Pagina.StartsWith(GaleriaHomePrefijo))
                    {
                        contexto.ConfiguracionHeroBannerTabla.Remove(c);
                        contexto.SaveChanges();
                    }
                }
                System.Web.HttpRuntime.Cache.Remove("GaleriaCarousel_Home");
                TempData["SuccessMessage"] = "Imagen quitada del carrusel.";
                return RedirectToAction("ConfigurarGaleriaCarousel");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al quitar: " + ex.Message;
                return RedirectToAction("ConfigurarGaleriaCarousel");
            }
        }

        // POST: Admin/SubirImagenGaleriaCarousel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubirImagenGaleriaCarousel(int id, HttpPostedFileBase imagen)
        {
            if (imagen == null || imagen.ContentLength == 0)
            {
                imagen = Request.Files["imagen"] ?? (Request.Files.Count > 0 ? Request.Files[0] : null);
            }
            if (imagen == null || imagen.ContentLength == 0)
                return Json(new { success = false, message = "Seleccione una imagen." });

            var extensiones = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".heic" };
            var ext = System.IO.Path.GetExtension(imagen.FileName ?? "").ToLower();
            if (string.IsNullOrEmpty(ext) || !extensiones.Contains(ext))
                return Json(new { success = false, message = "Formato no permitido. Use JPG, PNG, GIF, WEBP o HEIC." });

            try
            {
                string dir = Server.MapPath("~/Img/galeria-carousel/");
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                string fileName = $"galeria_{id}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                string path = System.IO.Path.Combine(dir, fileName);
                imagen.SaveAs(path);
                string urlImagen = "/Img/galeria-carousel/" + fileName;

                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext) ?? "";
                using (var contexto = new Contexto())
                {
                    var c = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (c == null) return Json(new { success = false, message = "Configuración no encontrada." });
                    contexto.Database.ExecuteSqlCommand(
                        "UPDATE ConfiguracionHeroBanner SET UrlImagen = @p0, FechaActualizacion = @p1, ActualizadoPor = @p2 WHERE IdConfiguracion = @p3",
                        urlImagen, DateTime.Now, userId, id);
                }
                System.Web.HttpRuntime.Cache.Remove("GaleriaCarousel_Home");
                if (Response != null) Response.RemoveOutputCacheItem("/");
                return Json(new { success = true, message = "Imagen actualizada.", urlImagen = urlImagen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: Admin/ActualizarGaleriaCarousel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarGaleriaCarousel(int id, string urlImagen)
        {
            if (string.IsNullOrWhiteSpace(urlImagen))
                return Json(new { success = false, message = "La URL es requerida." });
            try
            {
                var userId = UsuarioActualHelper.ObtenerId(this.HttpContext) ?? "";
                using (var contexto = new Contexto())
                {
                    var c = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (c == null) return Json(new { success = false, message = "Configuración no encontrada." });
                    contexto.Database.ExecuteSqlCommand(
                        "UPDATE ConfiguracionHeroBanner SET UrlImagen = @p0, FechaActualizacion = @p1, ActualizadoPor = @p2 WHERE IdConfiguracion = @p3",
                        urlImagen.Trim(), DateTime.Now, userId, id);
                }
                System.Web.HttpRuntime.Cache.Remove("GaleriaCarousel_Home");
                if (Response != null) Response.RemoveOutputCacheItem("/");
                return Json(new { success = true, message = "Galería actualizada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // --- Galería para usuarios (Apartamentos, Instalaciones, Habitaciones) en página Departamentos ---
        private const string GaleriaApartamentosPrefijo = "GaleriaApartamentos_";
        private const string GaleriaInstalacionesPrefijo = "GaleriaInstalaciones_";
        private const string GaleriaHabitacionesPrefijo = "GaleriaHabitaciones_";
        private static readonly string[] GaleriaApartamentosDefaults = { "Cocina", "Habitación", "Sala", "Baño", "Ambiente" };
        private static readonly string[] GaleriaApartamentosUrls = {
            "/Img/images/Apartamentos/WhatsApp cocinaapartamentos2025-12-17 at 10.33.45 AM.jpeg",
            "/Img/images/Apartamentos/WhatsApp Image 2025-12-17 at 10.33.41 AM.jpeg",
            "/Img/images/Apartamentos/WhatsApp Image 2xxxxx025-12-17 at 10.33.44 AM.jpeg",
            "/Img/images/Apartamentos/WhatsApp Image 2025-1ffffff.33.44 AM.jpeg",
            "/Img/images/Apartamentos/WhatsApp aaaaa2025-12-17 at 10.33.42 AM.jpeg"
        };
        private static readonly string[] GaleriaInstalacionesDefaults = { "Gimnasio", "Lavandería" };
        private static readonly string[] GaleriaInstalacionesUrls = { "/Img/images/Servicios/Gimnasio.jpeg", "/Img/images/Servicios/Lavandería.jpeg" };
        private static readonly string[] GaleriaHabitacionesDefaults = { "Habitación", "Cocina" };
        private static readonly string[] GaleriaHabitacionesUrls = { "/Img/images/Habitaciones/WhatsApp Image 2025-12-11 at 3.24.28 PM.jpeg", "/Img/images/Habitaciones/WhatsApp Image cocina por fuera-12-11 at 3.24.28 PM.jpeg" };

        [HttpGet]
        public ActionResult ConfigurarGaleriaUsuarios()
        {
            ViewBag.Title = "Configurar galería (Apartamentos, Instalaciones y Habitaciones)";
            try
            {
                using (var contexto = new Contexto())
                {
                    // Seed Apartamentos si no hay ninguno
                    if (!contexto.ConfiguracionHeroBannerTabla.Any(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaApartamentosPrefijo)))
                    {
                        for (int i = 0; i < GaleriaApartamentosDefaults.Length; i++)
                        {
                            contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                            {
                                Pagina = GaleriaApartamentosPrefijo + i,
                                UrlImagen = GaleriaApartamentosUrls[i] + "|" + GaleriaApartamentosDefaults[i],
                                FechaActualizacion = DateTime.Now
                            });
                        }
                        contexto.SaveChanges();
                    }
                    // Seed Instalaciones si no hay ninguna
                    if (!contexto.ConfiguracionHeroBannerTabla.Any(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaInstalacionesPrefijo)))
                    {
                        for (int i = 0; i < GaleriaInstalacionesDefaults.Length; i++)
                        {
                            contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                            {
                                Pagina = GaleriaInstalacionesPrefijo + i,
                                UrlImagen = GaleriaInstalacionesUrls[i] + "|" + GaleriaInstalacionesDefaults[i],
                                FechaActualizacion = DateTime.Now
                            });
                        }
                        contexto.SaveChanges();
                    }
                    // Seed Habitaciones si no hay ninguna
                    if (!contexto.ConfiguracionHeroBannerTabla.Any(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaHabitacionesPrefijo)))
                    {
                        for (int i = 0; i < GaleriaHabitacionesDefaults.Length; i++)
                        {
                            contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                            {
                                Pagina = GaleriaHabitacionesPrefijo + i,
                                UrlImagen = GaleriaHabitacionesUrls[i] + "|" + GaleriaHabitacionesDefaults[i],
                                FechaActualizacion = DateTime.Now
                            });
                        }
                        contexto.SaveChanges();
                    }
                    var apartamentosQuery = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaApartamentosPrefijo))
                        .Select(c => new ConfiguracionHeroBannerDTO { IdConfiguracion = c.IdConfiguracion, Pagina = c.Pagina, UrlImagen = c.UrlImagen, FechaActualizacion = c.FechaActualizacion, ActualizadoPor = c.ActualizadoPor });
                    var apartamentos = apartamentosQuery.ToList().OrderBy(c =>
                    {
                        var s = (c.Pagina ?? "").Replace(GaleriaApartamentosPrefijo, "");
                        int n;
                        return int.TryParse(s, out n) ? n : 0;
                    }).ToList();
                    var instalacionesQuery = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaInstalacionesPrefijo))
                        .Select(c => new ConfiguracionHeroBannerDTO { IdConfiguracion = c.IdConfiguracion, Pagina = c.Pagina, UrlImagen = c.UrlImagen, FechaActualizacion = c.FechaActualizacion, ActualizadoPor = c.ActualizadoPor });
                    var instalaciones = instalacionesQuery.ToList().OrderBy(c =>
                    {
                        var s = (c.Pagina ?? "").Replace(GaleriaInstalacionesPrefijo, "");
                        int n;
                        return int.TryParse(s, out n) ? n : 0;
                    }).ToList();
                    var habitacionesQuery = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(GaleriaHabitacionesPrefijo))
                        .Select(c => new ConfiguracionHeroBannerDTO { IdConfiguracion = c.IdConfiguracion, Pagina = c.Pagina, UrlImagen = c.UrlImagen, FechaActualizacion = c.FechaActualizacion, ActualizadoPor = c.ActualizadoPor });
                    var habitaciones = habitacionesQuery.ToList().OrderBy(c =>
                    {
                        var s = (c.Pagina ?? "").Replace(GaleriaHabitacionesPrefijo, "");
                        int n;
                        return int.TryParse(s, out n) ? n : 0;
                    }).ToList();
                    ViewBag.Apartamentos = apartamentos;
                    ViewBag.Instalaciones = instalaciones;
                    ViewBag.Habitaciones = habitaciones;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar: " + ex.Message;
                ViewBag.Apartamentos = new List<ConfiguracionHeroBannerDTO>();
                ViewBag.Instalaciones = new List<ConfiguracionHeroBannerDTO>();
                ViewBag.Habitaciones = new List<ConfiguracionHeroBannerDTO>();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarLeyendaGaleriaUsuarios(int id, string leyenda)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var c = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (c == null) { TempData["ErrorMessage"] = "Registro no encontrado."; return RedirectToAction("ConfigurarGaleriaUsuarios"); }
                    var u = c.UrlImagen ?? "";
                    var bar = u.IndexOf('|');
                    var urlPart = bar >= 0 ? u.Substring(0, bar).Trim() : u;
                    c.UrlImagen = urlPart + "|" + (leyenda ?? "").Trim();
                    c.FechaActualizacion = DateTime.Now;
                    c.ActualizadoPor = UsuarioActualHelper.ObtenerId(this.HttpContext);
                    contexto.SaveChanges();
                }
                TempData["SuccessMessage"] = "Leyenda guardada.";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("ConfigurarGaleriaUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubirImagenGaleriaUsuarios(string tipo, string leyenda, HttpPostedFileBase imagen)
        {
            var file = imagen ?? (Request.Files != null && Request.Files.Count > 0 ? Request.Files[0] : null);
            if (file == null || file.ContentLength == 0) { TempData["ErrorMessage"] = "Seleccione una imagen."; return RedirectToAction("ConfigurarGaleriaUsuarios"); }
            if (string.IsNullOrWhiteSpace(tipo) || (tipo != "Apartamentos" && tipo != "Instalaciones" && tipo != "Habitaciones")) { TempData["ErrorMessage"] = "Tipo inválido."; return RedirectToAction("ConfigurarGaleriaUsuarios"); }
            var prefijo = tipo == "Apartamentos" ? GaleriaApartamentosPrefijo : (tipo == "Habitaciones" ? GaleriaHabitacionesPrefijo : GaleriaInstalacionesPrefijo);
            var ext = System.IO.Path.GetExtension(file.FileName ?? "").ToLower();
            if (string.IsNullOrEmpty(ext) || !new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext)) { TempData["ErrorMessage"] = "Formato no permitido."; return RedirectToAction("ConfigurarGaleriaUsuarios"); }
            try
            {
                var dir = Server.MapPath("~/Content/Imagenes");
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                var fileName = "galeria_" + tipo.ToLower() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                var path = System.IO.Path.Combine(dir, fileName);
                file.SaveAs(path);
                var urlImagen = "/Content/Imagenes/" + fileName + "|" + (leyenda ?? "").Trim();
                using (var contexto = new Contexto())
                {
                    var maxNum = contexto.ConfiguracionHeroBannerTabla
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith(prefijo))
                        .ToList()
                        .Select(c => { var s = (c.Pagina ?? "").Replace(prefijo, ""); int n; return int.TryParse(s, out n) ? n : -1; })
                        .DefaultIfEmpty(-1).Max();
                    contexto.ConfiguracionHeroBannerTabla.Add(new ConfiguracionHeroBannerTabla
                    {
                        Pagina = prefijo + (maxNum + 1),
                        UrlImagen = urlImagen,
                        FechaActualizacion = DateTime.Now,
                        ActualizadoPor = UsuarioActualHelper.ObtenerId(this.HttpContext)
                    });
                    contexto.SaveChanges();
                }
                TempData["SuccessMessage"] = "Imagen agregada.";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("ConfigurarGaleriaUsuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarImagenGaleriaUsuarios(int id)
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var c = contexto.ConfiguracionHeroBannerTabla.Find(id);
                    if (c != null && c.Pagina != null && (c.Pagina.StartsWith(GaleriaApartamentosPrefijo) || c.Pagina.StartsWith(GaleriaInstalacionesPrefijo) || c.Pagina.StartsWith(GaleriaHabitacionesPrefijo)))
                    {
                        contexto.ConfiguracionHeroBannerTabla.Remove(c);
                        contexto.SaveChanges();
                        TempData["SuccessMessage"] = "Imagen eliminada.";
                    }
                }
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction("ConfigurarGaleriaUsuarios");
        }

        // GET: Admin/ConfigurarPreciosDepartamentos
        public ActionResult ConfigurarPreciosDepartamentos()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    // Verificar si la tabla existe antes de consultar
                    var configuracion = contexto.ConfiguracionPreciosDepartamentosTabla.FirstOrDefault();
                    
                    ConfiguracionPreciosDepartamentosDTO dto = null;
                    if (configuracion != null)
                    {
                        dto = new ConfiguracionPreciosDepartamentosDTO
                        {
                            IdConfiguracion = configuracion.IdConfiguracion,
                            PrecioBase = configuracion.PrecioBase,
                            TextoPrecio = configuracion.TextoPrecio,
                            MostrarPrecio = configuracion.MostrarPrecio,
                            FechaActualizacion = configuracion.FechaActualizacion,
                            ActualizadoPor = configuracion.ActualizadoPor
                        };
                    }
                    else
                    {
                        // Crear configuración por defecto si no existe
                        dto = new ConfiguracionPreciosDepartamentosDTO
                        {
                            PrecioBase = 275000,
                            TextoPrecio = "Por mes",
                            MostrarPrecio = true
                        };
                    }

                    ViewBag.Title = "Configurar Precios de Departamentos";
                    return View(dto);
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Si la tabla no existe, mostrar mensaje y crear configuración por defecto
                if (ex.Message.Contains("no es válido") || ex.Message.Contains("Invalid object name"))
                {
                    ViewBag.ErrorMessage = "Las tablas de configuración no existen en la base de datos. Por favor, ejecute el script SQL: DB/dbo/Tables/CREAR_TABLAS_CONFIGURACION.sql";
                    var dto = new ConfiguracionPreciosDepartamentosDTO
                    {
                        PrecioBase = 275000,
                        TextoPrecio = "Por mes",
                        MostrarPrecio = true
                    };
                    ViewBag.Title = "Configurar Precios de Departamentos";
                    return View(dto);
                }
                throw;
            }
        }

        // POST: Admin/ActualizarPreciosDepartamentos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarPreciosDepartamentos(ConfiguracionPreciosDepartamentosDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfigurarPreciosDepartamentos", model);
            }

            try
            {
                using (var contexto = new Contexto())
                {
                    // Forzar detección de cambios
                    contexto.Configuration.AutoDetectChangesEnabled = true;
                    
                    var configuracion = contexto.ConfiguracionPreciosDepartamentosTabla.FirstOrDefault();
                    
                    if (configuracion == null)
                    {
                        configuracion = new ConfiguracionPreciosDepartamentosTabla
                        {
                            PrecioBase = model.PrecioBase,
                            TextoPrecio = model.TextoPrecio ?? "Por mes",
                            MostrarPrecio = model.MostrarPrecio,
                            FechaActualizacion = DateTime.Now,
                            ActualizadoPor = UsuarioActualHelper.ObtenerId(this.HttpContext)
                        };
                        contexto.ConfiguracionPreciosDepartamentosTabla.Add(configuracion);
                    }
                    else
                    {
                        configuracion.PrecioBase = model.PrecioBase;
                        configuracion.TextoPrecio = model.TextoPrecio ?? "Por mes";
                        configuracion.MostrarPrecio = model.MostrarPrecio;
                        configuracion.FechaActualizacion = DateTime.Now;
                        configuracion.ActualizadoPor = UsuarioActualHelper.ObtenerId(this.HttpContext);
                    }

                    int cambios = contexto.SaveChanges();
                    
                    TempData["SuccessMessage"] = $"Precios actualizados correctamente. Cambios guardados: {cambios}";
                    return RedirectToAction("ConfigurarPreciosDepartamentos");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                TempData["ErrorMessage"] = "Error al guardar: " + ex.Message;
                return View("ConfigurarPreciosDepartamentos", model);
            }
        }
    }
}

