using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using HotelPrado.UI.Models;

namespace HotelPrado.UI
{
    public partial class Startup
    {
        // Para obtener más información sobre cómo configurar la autenticación, visite https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure el contexto de base de datos, el administrador de usuarios y el administrador de inicios de sesión para usar una única instancia por solicitud
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Permitir que la aplicación use una cookie para almacenar información para el usuario que inicia sesión
            // y una cookie para almacenar temporalmente información sobre un usuario que inicia sesión con un proveedor de inicio de sesión de terceros
            // Configurar cookie de inicio de sesión
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                // Never = cookie se envía en HTTP y HTTPS (hosting tras proxy a veces recibe HTTP y la cookie Secure no se guardaba).
                CookieSecure = CookieSecureOption.Never,
                CookieHttpOnly = true,
                CookieName = ".AspNet.ApplicationCookie",
                CookieSameSite = Microsoft.Owin.SameSiteMode.Lax,
                ExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = new CookieAuthenticationProvider
                {
                    // Validar identidad cada 24h (no cada 120 min) para que la sesión no se pierda por timeouts de BD
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromHours(24),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
                    // Evitar redirecciones infinitas: no redirigir a Login si la petición es a una ruta pública
                    OnApplyRedirect = context =>
                    {
                        var currentPath = context.Request.Path.Value?.ToLowerInvariant() ?? "";
                        // Rutas que deben ser accesibles sin sesión (igual que [AllowAnonymous] en controladores)
                        var rutasPublicas = new[] {
                            "/account/login", "/account/register", "/entrar", "/registro",
                            "/", "/home/index", "/home/about", "/home/contact", "/home/services", "/home/error", "/home/ping", "/home/diagnostico",
                            "/habitacion/habitacionesinfo", "/habitacion/indexhabitacionesusuario",
                            "/departamento/indexdepartamentosclientes",
                            "/citas/indexcitas", "/citas/create"
                        };
                        if (rutasPublicas.Any(r => currentPath.Equals(r, StringComparison.OrdinalIgnoreCase)))
                        {
                            return; // No redirigir a login en rutas públicas
                        }
                        context.Response.Redirect(context.RedirectUri);
                    }
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Permite que la aplicación almacene temporalmente la información del usuario cuando se verifica el segundo factor en el proceso de autenticación de dos factores.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Permite que la aplicación recuerde el segundo factor de verificación de inicio de sesión, como el teléfono o correo electrónico.
            // Cuando selecciona esta opción, el segundo paso de la verificación del proceso de inicio de sesión se recordará en el dispositivo desde el que ha iniciado sesión.
            // Es similar a la opción Recordarme al iniciar sesión.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Quitar los comentarios de las siguientes líneas para habilitar el inicio de sesión con proveedores de inicio de sesión de terceros
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}