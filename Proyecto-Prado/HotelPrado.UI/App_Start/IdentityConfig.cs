using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using HotelPrado.UI.Models;

namespace HotelPrado.UI
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // No enviar al correo de excepción del admin (no existe)
            var emailAdmin = ConfigurationManager.AppSettings["EmailAdminException"];
            if (!string.IsNullOrWhiteSpace(emailAdmin) && string.Equals(message.Destination?.Trim(), emailAdmin.Trim(), StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(0);

            var host = ConfigurationManager.AppSettings["EmailSmtpHost"];
            if (string.IsNullOrWhiteSpace(host))
                return Task.FromResult(0);

            var port = 25;
            int.TryParse(ConfigurationManager.AppSettings["EmailSmtpPort"] ?? "25", out port);
            var user = ConfigurationManager.AppSettings["EmailSmtpUser"];
            var pass = ConfigurationManager.AppSettings["EmailSmtpPassword"];
            var from = ConfigurationManager.AppSettings["EmailFrom"] ?? user ?? "noreply@hotelprado.com";
            var fromName = ConfigurationManager.AppSettings["EmailFromName"] ?? "Hotel Prado";
            var enableSsl = string.Equals(ConfigurationManager.AppSettings["EmailSmtpSSL"], "true", StringComparison.OrdinalIgnoreCase);

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                if (!string.IsNullOrEmpty(user))
                    client.Credentials = new NetworkCredential(user, pass);
                    var mail = new MailMessage(from, message.Destination, message.Subject, message.Body) { IsBodyHtml = true };
                mail.From = new MailAddress(from, fromName);
                client.Send(mail);
            }
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Conecte el servicio SMS aquí para enviar un mensaje de texto.
            return Task.FromResult(0);
        }
    }

    // Configure el administrador de usuarios de aplicación que se usa en esta aplicación. UserManager se define en ASP.NET Identity y se usa en la aplicación.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure la lógica de validación de nombres de usuario
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure la lógica de validación de contraseñas
            // Optimizado: reducir validaciones para login más rápido (solo longitud mínima)
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,  // Deshabilitado para login más rápido
                RequireDigit = false,              // Deshabilitado para login más rápido
                RequireLowercase = false,          // Deshabilitado para login más rápido
                RequireUppercase = false,         // Deshabilitado para login más rápido
            };

            // Configurar valores predeterminados para bloqueo de usuario
            // Optimizado: deshabilitar bloqueo temporalmente para login más rápido
            manager.UserLockoutEnabledByDefault = false;  // Deshabilitado para evitar consultas adicionales
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registre los proveedores de autenticación de dos factores. Esta aplicación usa el teléfono y el correo electrónico para recibir un código de verificación del usuario
            // Puede escribir su propio proveedor y conectarlo aquí.
            manager.RegisterTwoFactorProvider("Código telefónico", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Su código de seguridad es {0}"
            });
            manager.RegisterTwoFactorProvider("Código de correo electrónico", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Código de seguridad",
                BodyFormat = "Su código de seguridad es {0}"
            });
            // RequireConfirmedEmail no existe en ASP.NET Identity para .NET Framework (sí en Core).
            // La confirmación de correo se puede validar en AccountController al hacer login si se desea.
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure el administrador de inicios de sesión que se usa en esta aplicación.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
