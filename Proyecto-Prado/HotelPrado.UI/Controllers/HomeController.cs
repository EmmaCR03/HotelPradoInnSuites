using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HotelPrado.AccesoADatos;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Configuracion;
using HotelPrado.UI.Models;
using Microsoft.AspNet.Identity.Owin;

namespace HotelPrado.UI.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>Imagen de hero por defecto según la página (cuando no hay config en BD o falla la conexión).</summary>
        private static string HeroPorDefecto(string pagina)
        {
            switch (pagina)
            {
                case "Home": return "/Img/images/img_1.JPG";
                case "About": return "/Img/images/IMG_3.JPG";
                case "Contacto": return "/Img/images/Contactenos/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg";
                case "Services": return "/Img/images/Servicios/WhatsApp SIPEPE2025-12-11 at 3.27.00 PM.jpeg";
                default: return "/Img/images/img_1.JPG";
            }
        }

        private const int CACHE_HERO_MINUTOS = 5;
        private static string CacheKeyHero(string pagina) => "HeroBanner_" + pagina;

        private string ObtenerHeroBanner(string pagina)
        {
            var cacheKey = CacheKeyHero(pagina);
            var cached = HttpRuntime.Cache[cacheKey] as string;
            if (cached != null)
                return cached;

            try
            {
                using (var contexto = new Contexto())
                {
                    contexto.Configuration.AutoDetectChangesEnabled = false;
                    var config = contexto.ConfiguracionHeroBannerTabla
                        .AsNoTracking()
                        .Where(c => c.Pagina == pagina)
                        .OrderByDescending(c => c.FechaActualizacion)
                        .Select(c => c.UrlImagen)
                        .FirstOrDefault();
                    var url = config ?? HeroPorDefecto(pagina);
                    HttpRuntime.Cache.Insert(cacheKey, url, null,
                        DateTime.UtcNow.AddMinutes(CACHE_HERO_MINUTOS),
                        System.Web.Caching.Cache.NoSlidingExpiration);
                    return url;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cargando hero banner para " + pagina + ": " + ex.Message);
                return HeroPorDefecto(pagina);
            }
        }

        private const string CACHE_GALERIA = "GaleriaCarousel_Home";

        private List<Tuple<string, string>> ObtenerGaleriaCarousel()
        {
            var cached = HttpRuntime.Cache[CACHE_GALERIA] as List<Tuple<string, string>>;
            if (cached != null) return cached;

            var porDefecto = new List<Tuple<string, string>>
            {
                Tuple.Create("/Img/images/IMG_5.JPG", "Habitación Deluxe"),
                Tuple.Create("/Img/images/IMG_6.JPG", "Sala de estar"),
                Tuple.Create("/Img/images/img_1.JPG", "Vista desde la habitación")
            };

            try
            {
                using (var contexto = new Contexto())
                {
                    var items = contexto.ConfiguracionHeroBannerTabla
                        .AsNoTracking()
                        .Where(c => c.Pagina != null && c.Pagina.StartsWith("GaleriaHome_"))
                        .ToList()
                        .OrderBy(c => { var s = (c.Pagina ?? "").Replace("GaleriaHome_", ""); int n; return int.TryParse(s, out n) ? n : 0; })
                        .Take(20)
                        .Select(c => Tuple.Create(c.UrlImagen ?? "", c.Pagina ?? ""))
                        .ToList();
                    if (items.Any())
                    {
                        var list = items.Select(t => Tuple.Create(t.Item1, t.Item2.Replace("GaleriaHome_", "Slide "))).ToList();
                        HttpRuntime.Cache.Insert(CACHE_GALERIA, list, null, DateTime.UtcNow.AddMinutes(CACHE_HERO_MINUTOS), System.Web.Caching.Cache.NoSlidingExpiration);
                        return list;
                    }
                }
            }
            catch { }
            return porDefecto;
        }

        [AllowAnonymous]
        // Sin OutputCache: en hosting la barra mostraba "Iniciar sesión" aunque el usuario estuviera logueado
        public ActionResult Index()
        {
            ViewBag.HeroBannerUrl = ObtenerHeroBanner("Home");
            ViewBag.GaleriaCarousel = ObtenerGaleriaCarousel();
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.HeroBannerUrl = ObtenerHeroBanner("About");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.HeroBannerUrl = ObtenerHeroBanner("Contacto");
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Services()
        {
            ViewBag.HeroBannerUrl = ObtenerHeroBanner("Services");
            return View();
        }

        /// <summary>Vista de error para que /Error no devuelva 404 (p. ej. cuando customErrors redirige aquí). Siempre captura y pasa el error real para poder diagnosticar en hosting.</summary>
        [AllowAnonymous]
        public ActionResult Error()
        {
            try
            {
                var ctx = System.Web.HttpContext.Current;
                var app = ctx?.Application;
                // Primero intentar obtener la excepción del servidor (cuando customErrors redirige aquí)
                var ex = ctx?.Server?.GetLastError();
                if (ex != null)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    ViewBag.ErrorStackTrace = ex.StackTrace;
                    ViewBag.ErrorType = ex.GetType().FullName;
                    ViewBag.InnerError = ex.InnerException != null ? ex.InnerException.Message + (ex.InnerException.StackTrace != null ? "\n" + ex.InnerException.StackTrace : "") : null;
                    ctx.Server.ClearError();
                }
                else if (app != null)
                {
                    ViewBag.ErrorMessage = app["LastError"] as string;
                    ViewBag.ErrorStackTrace = app["LastErrorStackTrace"] as string;
                    ViewBag.ErrorType = app["LastErrorType"] as string;
                    ViewBag.InnerError = app["LastInnerError"] as string;
                }
                return View("~/Views/Shared/Error.cshtml");
            }
            catch
            {
                return Content(
                    "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>" +
                    "<h1 style='color:#c00'>Error</h1><p>Ocurrió un error al mostrar la página de detalle.</p>" +
                    "<p><a href='/'>Volver al inicio</a> | <a href='/Account/Login'>Iniciar sesión</a></p></body></html>",
                    "text/html");
            }
        }

        /// <summary>Prueba rápida: si la app corre, responde OK. No usa base de datos.</summary>
        [AllowAnonymous]
        public ActionResult Ping()
        {
            return Content("OK", "text/plain");
        }

        /// <summary>Diagnóstico hosteado: OWIN y BD. Solo para ver por qué falla Login/Register.</summary>
        [AllowAnonymous]
        public ActionResult Diagnostico()
        {
            var html = "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Diagnóstico</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>";
            html += "<h1>Diagnóstico</h1>";
            // OWIN
            try
            {
                var ctx = Request.GetOwinContext();
                html += "<p><strong>OWIN:</strong> <span style='color:green'>OK</span></p>";
            }
            catch (Exception ex)
            {
                html += "<p><strong>OWIN:</strong> <span style='color:red'>Error</span><br><code>" + System.Web.HttpUtility.HtmlEncode(ex.Message) + "</code></p>";
            }
            // BD (Identity)
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                }
                html += "<p><strong>Base de datos (Identity):</strong> <span style='color:green'>OK</span></p>";
            }
            catch (Exception ex)
            {
                html += "<p><strong>Base de datos (Identity):</strong> <span style='color:red'>Error</span><br><code>" + System.Web.HttpUtility.HtmlEncode(ex.Message) + "</code></p>";
            }
            html += "<p><a href='/Account/Login'>Ir a Login</a> | <a href='/'>Inicio</a></p></body></html>";
            return Content(html, "text/html");
        }

        /// <summary>Página de inicio de sesión mínima (sin layout ni Account). Si esta carga y /Account/Login no, el fallo está en Account o sus vistas.</summary>
        [AllowAnonymous]
        public ActionResult Entrar(string returnUrl, int? error)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/";
            // Mostrar mensaje según error devuelto por Account/Login POST
            if (error.HasValue)
            {
                switch (error.Value)
                {
                    case 1: ViewBag.ErrorMessage = "Correo o contraseña incorrectos. Intente de nuevo."; break;
                    case 2: ViewBag.ErrorMessage = "Por favor complete correo y contraseña."; break;
                    case 3: ViewBag.ErrorMessage = "La cuenta está bloqueada temporalmente."; break;
                    case 4: ViewBag.ErrorMessage = "Debe confirmar su correo electrónico antes de iniciar sesión. Revise su bandeja o el enlace que le enviamos al registrarse."; break;
                    default: ViewBag.ErrorMessage = "Error al iniciar sesión. Intente de nuevo."; break;
                }
            }
            return View();
        }

        /// <summary>Página de registro mínima (sin layout ni Account). Para probar si el fallo es solo en Account.</summary>
        [AllowAnonymous]
        public ActionResult Registro()
        {
            return View();
        }

        /// <summary>Muestra el error REAL que falla al cargar Register. Usar en el servidor para diagnosticar.</summary>
        [AllowAnonymous]
        public ActionResult VerErrorRegister()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== DIAGNÓSTICO REGISTER (paso a paso) ===");
            sb.AppendLine();

            // Paso 1: OWIN
            try
            {
                var ctx = Request.GetOwinContext();
                sb.AppendLine("1. OWIN: OK");
            }
            catch (Exception ex)
            {
                sb.AppendLine("1. OWIN: FALLO");
                sb.AppendLine("   Mensaje: " + ex.Message);
                sb.AppendLine("   Tipo: " + ex.GetType().FullName);
                sb.AppendLine("   Stack: " + (ex.StackTrace ?? ""));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("   Inner: " + ex.InnerException.Message);
                    sb.AppendLine("   Inner Stack: " + (ex.InnerException.StackTrace ?? ""));
                }
                return Content(sb.ToString(), "text/plain; charset=utf-8");
            }

            // Paso 2: Base de datos
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                }
                sb.AppendLine("2. Base de datos (Identity): OK");
            }
            catch (Exception ex)
            {
                sb.AppendLine("2. Base de datos: FALLO");
                sb.AppendLine("   Mensaje: " + ex.Message);
                sb.AppendLine("   Tipo: " + ex.GetType().FullName);
                sb.AppendLine("   Stack: " + (ex.StackTrace ?? ""));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("   Inner: " + ex.InnerException.Message);
                }
                return Content(sb.ToString(), "text/plain; charset=utf-8");
            }

            // Paso 3: UserManager (lo que usa Account/Register)
            try
            {
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                sb.AppendLine("3. UserManager: OK");
            }
            catch (Exception ex)
            {
                sb.AppendLine("3. UserManager: FALLO");
                sb.AppendLine("   Mensaje: " + ex.Message);
                sb.AppendLine("   Tipo: " + ex.GetType().FullName);
                sb.AppendLine("   Stack: " + (ex.StackTrace ?? ""));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("   Inner: " + ex.InnerException.Message);
                    sb.AppendLine("   Inner Stack: " + (ex.InnerException.StackTrace ?? ""));
                }
                return Content(sb.ToString(), "text/plain; charset=utf-8");
            }

            // Paso 4: Intentar cargar la vista Register
            try
            {
                var model = new RegisterViewModel();
                return View("~/Views/Account/Register.cshtml", model);
            }
            catch (Exception ex)
            {
                sb.AppendLine("4. Vista Register: FALLO");
                sb.AppendLine("   Mensaje: " + ex.Message);
                sb.AppendLine("   Tipo: " + ex.GetType().FullName);
                sb.AppendLine("   Stack: " + (ex.StackTrace ?? ""));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("   Inner: " + ex.InnerException.Message);
                    sb.AppendLine("   Inner Stack: " + (ex.InnerException.StackTrace ?? ""));
                }
                return Content(sb.ToString(), "text/plain; charset=utf-8");
            }
        }
    }
}
