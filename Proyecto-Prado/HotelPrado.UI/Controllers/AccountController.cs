using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HotelPrado.UI.Models;
using HotelPrado.AccesoADatos;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;

namespace HotelPrado.UI.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        /// <summary>Obtiene la URL de imagen de fondo para Login/Registro desde ConfiguracionHeroBanner (con caché).</summary>
        private string ObtenerImagenFondo(string pagina)
        {
            var cacheKey = "HeroBanner_" + pagina;
            var cached = System.Web.HttpRuntime.Cache[cacheKey] as string;
            if (cached != null) return cached;
            try
            {
                using (var contexto = new Contexto())
                {
                    var url = contexto.ConfiguracionHeroBannerTabla
                        .AsNoTracking()
                        .Where(c => c.Pagina == pagina)
                        .OrderByDescending(c => c.FechaActualizacion)
                        .Select(c => c.UrlImagen)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(url))
                        url = "/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg";
                    System.Web.HttpRuntime.Cache.Insert(cacheKey, url, null,
                        DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    return url;
                }
            }
            catch { return "/Img/images/login/WhatsApp Image 2025-12-11 at 3.27.01 PM.jpeg"; }
        }

        /// <summary>Indica si el correo es la excepción del admin (no requiere verificación).</summary>
        private static bool EsEmailAdminException(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var excepcion = ConfigurationManager.AppSettings["EmailAdminException"];
            return !string.IsNullOrWhiteSpace(excepcion) && string.Equals(email.Trim(), excepcion.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Evitar bucles: si ya estamos siendo redirigidos desde Login, ignorar returnUrl
                returnUrl = SanitizeReturnUrl(returnUrl);
                
                // Verificar si hay un bucle de redirección detectado
                var redirectCount = Session?["RedirectCount"] as int? ?? 0;
                if (redirectCount > 3)
                {
                    Session["RedirectCount"] = 0; // Resetear contador
                    // Mostrar página de error en lugar de redirigir
                    return Content(
                        "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error de Redirección</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>" +
                        "<h1 style='color:#c00'>Error de Redirección Detectado</h1>" +
                        "<p>Se detectó un bucle de redirección. Por favor, limpie las cookies del navegador e intente nuevamente.</p>" +
                        "<p><a href='/'>Ir al inicio</a> | <a href='/Account/Login'>Reintentar login</a></p>" +
                        "</body></html>",
                        "text/html");
                }
                
                ViewBag.ReturnUrl = returnUrl;
                
                // Verificar que OWIN esté inicializado correctamente
                try
                {
                    var owinContext = this.HttpContext.GetOwinContext();
                    if (owinContext == null)
                    {
                        throw new InvalidOperationException("OWIN context no está disponible. Verifique que OWIN esté configurado correctamente.");
                    }
                }
                catch (Exception owinEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error OWIN en Login: {owinEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {owinEx.StackTrace}");
                    // Continuar con la carga de la vista aunque haya un problema con OWIN
                }
                
                ViewBag.ImagenFondo = ObtenerImagenFondo("Login");
                return View(new LoginViewModel());
            }
            catch (Exception ex)
            {
                // Log detallado del error
                System.Diagnostics.Debug.WriteLine($"Error en Account/Login GET: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Tipo de excepción: {ex.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                // Si algo falla (ej. BD no disponible), mostrar mensaje en vez de error genérico
                // NO redirigir para evitar bucles
                return Content(
                    "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>" +
                    "<h1 style='color:#c00'>No se pudo cargar la página de inicio de sesión</h1>" +
                    "<p><strong>Error:</strong> " + HttpUtility.HtmlEncode(ex.Message) + "</p>" +
                    "<p><strong>Tipo:</strong> " + HttpUtility.HtmlEncode(ex.GetType().FullName) + "</p>" +
                    (ex.InnerException != null ? "<p><strong>Error interno:</strong> " + HttpUtility.HtmlEncode(ex.InnerException.Message) + "</p>" : "") +
                    "<p><strong>Compruebe:</strong></p>" +
                    "<ul><li>Que publicó en modo <b>Release</b> (para que use la cadena de conexión del host)</li>" +
                    "<li>Que en el servidor el archivo Web.config tiene la cadena de conexión correcta (Data Source=... del hosting)</li>" +
                    "<li>Que la base de datos del host tiene las tablas de Identity (AspNetUsers, AspNetRoles, etc.)</li>" +
                    "<li>Que OWIN está configurado correctamente en Startup.cs</li></ul>" +
                    "<p><a href='/'>Ir al inicio</a> | <a href='/Home/Ping'>Probar si la app responde (Ping)</a> | <a href='/Home/Diagnostico'>Diagnóstico</a></p>" +
                    "</body></html>",
                    "text/html");
            }
        }

        //
        // POST: /Account/Login
        // Sin ValidateAntiForgeryToken: en móvil la cookie antifalsificación a veces no se envía con el POST,
        // lo que provoca Error 500 "cookie no está presente". El login sigue protegido por credenciales.
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            returnUrl = SanitizeReturnUrl(returnUrl);
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ImagenFondo = ObtenerImagenFondo("Login");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Por favor complete correo y contraseña.");
                return View(model);
            }

            try
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                
                switch (result)
                {
                    case SignInStatus.Success:
                        // Respaldo en Session para que el host muestre "Hola" aunque la cookie falle al leer (machineKey, proxy, etc.).
                        var userLogin = await UserManager.FindByEmailAsync(model.Email);
                        if (userLogin != null)
                        {
                            Session["UserId"] = userLogin.Id;
                            Session["UserName"] = userLogin.UserName ?? userLogin.Email ?? "";
                        }
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "La cuenta está bloqueada temporalmente.");
                        return View(model);
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        var usuarioPorEmail = await UserManager.FindByEmailAsync(model.Email);
                        if (usuarioPorEmail != null && !await UserManager.IsEmailConfirmedAsync(usuarioPorEmail.Id))
                        {
                            // Excepción: el admin no tiene correo real, puede iniciar sesión sin confirmar
                            if (!EsEmailAdminException(usuarioPorEmail.Email))
                            {
                                ModelState.AddModelError("", "Debe confirmar su correo electrónico antes de iniciar sesión. Revise su bandeja o el enlace que le enviamos al registrarse.");
                                return View(model);
                            }
                        }
                        ModelState.AddModelError("", "Correo o contraseña incorrectos. Intente de nuevo.");
                        return View(model);
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                // Manejar errores específicos de SQL Server con más detalle
                System.Diagnostics.Debug.WriteLine($"Error SQL en login: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Número de error: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"Servidor: {sqlEx.Server}");
                
                string mensajeError = "Error al conectar con la base de datos. ";
                
                // Mensajes específicos según el tipo de error
                if (sqlEx.Number == -1 || sqlEx.Number == 2)
                {
                    mensajeError += "No se puede conectar al servidor SQL Server. Verifique que el servicio esté ejecutándose.";
                }
                else if (sqlEx.Number == 4060)
                {
                    mensajeError += "No se puede abrir la base de datos 'HotelPrado'. Verifique que la base de datos exista.";
                }
                else if (sqlEx.Number == 18456)
                {
                    mensajeError += "Error de autenticación. Verifique las credenciales de SQL Server.";
                }
                else
                {
                    mensajeError += $"Error SQL ({sqlEx.Number}): {sqlEx.Message}";
                }
                
                ModelState.AddModelError("", mensajeError);
                return View(model);
            }
            catch (System.InvalidOperationException ioEx)
            {
                // Manejar errores de configuración de Entity Framework
                System.Diagnostics.Debug.WriteLine($"Error de configuración en login: {ioEx.Message}");
                ModelState.AddModelError("", "Error de configuración de la base de datos. Verifique la cadena de conexión en Web.config.");
                return View(model);
            }
            catch (Exception ex)
            {
                // Manejar otros errores de conexión
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Error al iniciar sesión. Por favor, intente nuevamente.");
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            try
            {
                // Verificar que OWIN esté inicializado correctamente (sin lanzar excepción si falla)
                try
                {
                    var owinContext = this.HttpContext.GetOwinContext();
                    if (owinContext == null)
                    {
                        // Si OWIN no está disponible, intentar cargar la vista de todas formas
                        System.Diagnostics.Debug.WriteLine("Advertencia: OWIN context no está disponible en Register GET");
                    }
                }
                catch (Exception owinEx)
                {
                    // Log pero continuar
                    System.Diagnostics.Debug.WriteLine($"Advertencia OWIN en Register: {owinEx.Message}");
                }
                
                // Crear ViewModel de forma segura
                ViewBag.ImagenFondo = ObtenerImagenFondo("Registro");
                var model = new RegisterViewModel();
                return View(model);
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                // Error de base de datos - mostrar mensaje amigable
                return Content(
                    "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error de Base de Datos</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>" +
                    "<h1 style='color:#c00'>Error de Conexión a Base de Datos</h1>" +
                    "<p><strong>Error SQL:</strong> " + HttpUtility.HtmlEncode(sqlEx.Message) + "</p>" +
                    "<p><strong>Número:</strong> " + sqlEx.Number + "</p>" +
                    "<p><strong>Servidor:</strong> " + HttpUtility.HtmlEncode(sqlEx.Server ?? "N/A") + "</p>" +
                    "<p><strong>Verifique:</strong></p>" +
                    "<ul><li>Que la cadena de conexión en Web.config sea correcta</li>" +
                    "<li>Que el servidor SQL esté accesible</li>" +
                    "<li>Que las credenciales sean correctas</li></ul>" +
                    "<p><a href='/'>Ir al inicio</a> | <a href='/Account/Login'>Iniciar sesión</a></p>" +
                    "</body></html>",
                    "text/html");
            }
            catch (Exception ex)
            {
                // Log detallado del error
                System.Diagnostics.Debug.WriteLine($"Error en Account/Register GET: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Tipo de excepción: {ex.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                // Mostrar error detallado en lugar de lanzar excepción
                return Content(
                    "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Error</title></head><body style='font-family:sans-serif;padding:2rem;max-width:600px;margin:0 auto;'>" +
                    "<h1 style='color:#c00'>Error al cargar página de registro</h1>" +
                    "<p><strong>Error:</strong> " + HttpUtility.HtmlEncode(ex.Message) + "</p>" +
                    "<p><strong>Tipo:</strong> " + HttpUtility.HtmlEncode(ex.GetType().FullName) + "</p>" +
                    (ex.InnerException != null ? "<p><strong>Error interno:</strong> " + HttpUtility.HtmlEncode(ex.InnerException.Message) + "</p>" : "") +
                    "<p><strong>Compruebe:</strong></p>" +
                    "<ul><li>Que publicó en modo <b>Release</b></li>" +
                    "<li>Que el archivo Web.config tiene la cadena de conexión correcta</li>" +
                    "<li>Que la base de datos tiene las tablas de Identity</li>" +
                    "<li>Que OWIN está configurado correctamente</li></ul>" +
                    "<p><a href='/'>Ir al inicio</a> | <a href='/Account/Login'>Iniciar sesión</a> | <a href='/Home/Ping'>Probar Ping</a> | <a href='/Home/Diagnostico'>Diagnóstico</a></p>" +
                    "</body></html>",
                    "text/html");
            }
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.Email, 
                    Email = model.Email,
                    NombreCompleto=model.NombreCompleto,
                    cedula=model.cedula,
                    Telefono=model.Telefono
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // ==========================================
                    // VINCULAR CLIENTE EXISTENTE O CREAR NUEVO
                    // ==========================================
                    try
                    {
                        using (var contexto = new Contexto())
                        {
                            // Buscar cliente existente por email o cédula - OPTIMIZADO
                            var emailParam = new SqlParameter("@email", model.Email?.ToLower().Trim() ?? "");
                            var cedulaParam = new SqlParameter("@cedula", model.cedula?.Trim() ?? "");
                            
                            // Query optimizada con índices sugeridos
                            var clienteExistente = await contexto.Database.SqlQuery<ClienteVinculacion>(
                                "SELECT TOP 1 IdCliente, EmailCliente, CedulaCliente FROM Cliente WITH (NOLOCK) WHERE " +
                                "(LOWER(LTRIM(RTRIM(EmailCliente))) = @email OR " +
                                "LTRIM(RTRIM(CedulaCliente)) = @cedula) AND (IdUsuario IS NULL OR IdUsuario = '')",
                                emailParam, cedulaParam
                            ).FirstOrDefaultAsync();

                            if (clienteExistente != null)
                            {
                                // Vincular cliente existente con el nuevo usuario
                                var userIdParam = new SqlParameter("@userId", user.Id);
                                var clienteIdParam = new SqlParameter("@clienteId", clienteExistente.IdCliente);
                                
                                await contexto.Database.ExecuteSqlCommandAsync(
                                    "UPDATE Cliente SET IdUsuario = @userId WHERE IdCliente = @clienteId",
                                    userIdParam, clienteIdParam
                                );
                            }
                            else
                            {
                                // Crear nuevo cliente si no existe
                                var nombreParam = new SqlParameter("@nombre", model.NombreCompleto?.Trim() ?? "");
                                var emailParam2 = new SqlParameter("@email", model.Email?.Trim() ?? "");
                                var cedulaParam2 = new SqlParameter("@cedula", model.cedula?.Trim() ?? "");
                                var telefonoParam = new SqlParameter("@telefono", model.Telefono?.Trim() ?? "");
                                var userIdParam2 = new SqlParameter("@userId", user.Id);
                                
                                await contexto.Database.ExecuteSqlCommandAsync(
                                    "INSERT INTO Cliente (NombreCliente, EmailCliente, CedulaCliente, TelefonoCliente, IdUsuario) " +
                                    "VALUES (@nombre, @email, @cedula, @telefono, @userId)",
                                    nombreParam, emailParam2, cedulaParam2, telefonoParam, userIdParam2
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log del error pero no fallar el registro
                        System.Diagnostics.Debug.WriteLine("Error al vincular cliente: " + ex.Message);
                    }
                    // ==========================================

                    // Asignar automáticamente el rol "Cliente" al nuevo usuario
                    var roleManager = new Microsoft.AspNet.Identity.RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(
                        new Microsoft.AspNet.Identity.EntityFramework.RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new ApplicationDbContext()));
                    
                    // Verificar que el rol "Cliente" existe, si no, crearlo
                    if (!roleManager.RoleExists("Cliente"))
                    {
                        roleManager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Cliente"));
                    }
                    
                    // Asignar el rol "Cliente" al usuario
                    var addRoleResult = await UserManager.AddToRoleAsync(user.Id, "Cliente");
                    if (!addRoleResult.Succeeded)
                    {
                        System.Diagnostics.Debug.WriteLine("Error al asignar rol Cliente: " + string.Join(", ", addRoleResult.Errors));
                    }

                    // Verificación de correo: generar token y enviar enlace (no iniciar sesión hasta confirmar)
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = token }, Request.Url.Scheme);
                    try
                    {
                        await UserManager.SendEmailAsync(user.Id, "Active su cuenta - Hotel Prado",
                            "Hola,<br/><br/>Para activar su cuenta haga clic en el siguiente enlace:<br/><br/><a href=\"" + callbackUrl + "\">Activar cuenta</a><br/><br/>Si no solicitó este registro, ignore este correo.");
                    }
                    catch
                    {
                        // Si falla el envío (ej. SMTP no configurado), el enlace se muestra en ReviseCorreo
                    }
                    return RedirectToAction("ReviseCorreo", "Account", new { email = model.Email, linkEnlace = callbackUrl });
                }
                AddErrors(result);
            }
            return View(model);
        }

        /// <summary>
        /// Página tras registro: indica que revise su correo para activar la cuenta. Si no recibió el correo, se muestra el enlace.
        /// </summary>
        [AllowAnonymous]
        public ActionResult ReviseCorreo(string email, string linkEnlace)
        {
            ViewBag.Email = email ?? "";
            ViewBag.LinkEnlace = linkEnlace ?? "";
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar un correo electrónico con este vínculo
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        Session["UserId"] = user.Id;
                        Session["UserName"] = user.UserName ?? user.Email ?? "";
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        // Sin ValidateAntiForgeryToken: evita error "token diseñado para un usuario distinto" al cerrar sesión
        // (p. ej. con SameSite=Strict o cuando la identidad ya cambió). Cerrar sesión no requiere protección CSRF.
        public ActionResult LogOff()
        {
            Session["UserId"] = null;
            Session["UserName"] = null;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Evita bucles de redirección: si ReturnUrl apunta a Login o está mal formado, se ignora.
        /// </summary>
        private static string SanitizeReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl)) return null;
            if (returnUrl.Length > 500) return null; // ReturnUrl codificado muchas veces
            
            // Lista de rutas que NO deben ser returnUrl para evitar bucles
            var problematicPaths = new[] 
            { 
                "Account/Login", 
                "Account/Register", 
                "Entrar", 
                "Registro",
                "Error",
                "Login",
                "Register"
            };
            
            var lowerReturnUrl = returnUrl.ToLowerInvariant();
            foreach (var path in problematicPaths)
            {
                if (lowerReturnUrl.Contains(path.ToLowerInvariant()))
                {
                    return null; // Ignorar returnUrl problemático
                }
            }
            
            // Si contiene ReturnUrl codificado múltiples veces, es probablemente un bucle
            if (returnUrl.IndexOf("ReturnUrl", StringComparison.OrdinalIgnoreCase) >= 0 && 
                returnUrl.Split(new[] { "ReturnUrl" }, StringSplitOptions.None).Length > 3)
            {
                return null;
            }
            
            return returnUrl;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            returnUrl = SanitizeReturnUrl(returnUrl);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        // Clase auxiliar para la vinculación de clientes
        private class ClienteVinculacion
        {
            public int IdCliente { get; set; }
            public string EmailCliente { get; set; }
            public string CedulaCliente { get; set; }
        }
    }
}