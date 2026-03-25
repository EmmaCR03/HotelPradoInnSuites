using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HotelPrado.UI.Models;
using HotelPrado.UI.Helpers;

namespace HotelPrado.UI.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        /// <summary>Obtiene el Id del usuario (Identity, Session, claims o email en host cuando la cookie no se lee).</summary>
        private string GetCurrentUserId()
        {
            return UsuarioActualHelper.ObtenerId(this.HttpContext);
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña se ha cambiado."
                : message == ManageMessageId.SetPasswordSuccess ? "Su contraseña se ha establecido."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Su proveedor de autenticación de dos factores se ha establecido."
                : message == ManageMessageId.ChangeEmailSendConfirmation ? "Se envió un enlace de confirmación a su nuevo correo. Debe hacer clic en el enlace para activarlo."
                : message == ManageMessageId.Error ? "Se ha producido un error."
                : message == ManageMessageId.AddPhoneSuccess ? "Se ha agregado su número de teléfono."
                : message == ManageMessageId.RemovePhoneSuccess ? "Se ha quitado su número de teléfono."
                : "";

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),

                  UserName = user.UserName,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Cedula = user.cedula,
                Telefono = user.Telefono
            };
            return View(model);
        }

        // GET: /Manage/EditProfile
        public async Task<ActionResult> EditProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
            var user = await UserManager.FindByIdAsync(userId);
            
            var model = new EditProfileViewModel
            {
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                Telefono = user.Telefono,
                Cedula = user.cedula
            };
            
            return View(model);
        }

        // POST: /Manage/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
            var user = await UserManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            // Correo de excepción del admin: solo quien ya lo tiene puede usarlo (no se puede registrar ni asignar a otros)
            var emailAdminException = ConfigurationManager.AppSettings["EmailAdminException"]?.Trim();
            var nuevoEsAdmin = !string.IsNullOrEmpty(emailAdminException) && string.Equals(model.Email?.Trim(), emailAdminException, StringComparison.OrdinalIgnoreCase);
            var usuarioActualEsAdmin = !string.IsNullOrEmpty(emailAdminException) && string.Equals(user.Email?.Trim(), emailAdminException, StringComparison.OrdinalIgnoreCase);
            if (nuevoEsAdmin && !usuarioActualEsAdmin)
            {
                ModelState.AddModelError("Email", "Ese correo está reservado.");
                return View(model);
            }

            // Verificar si el email cambió y si ya existe
            if (user.Email != model.Email)
            {
                var existingUser = await UserManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != userId)
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está en uso.");
                    return View(model);
                }
            }

            var emailCambiado = user.Email != model.Email;
            // Actualizar información
            user.NombreCompleto = model.NombreCompleto;
            user.Email = model.Email;
            user.UserName = model.Email; // Actualizar también el UserName
            user.Telefono = model.Telefono;
            user.cedula = model.Cedula;

            // Si cambió a un correo real (no el del admin), exigir confirmación del nuevo correo
            if (emailCambiado && !nuevoEsAdmin)
                user.EmailConfirmed = false;

            if (emailCambiado && nuevoEsAdmin)
                user.EmailConfirmed = true; // Admin: no hay buzón real, dejar como confirmado

            var result = await UserManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                // Si el email cambió, actualizar el claim
                if (emailCambiado)
                {
                    var emailClaim = await UserManager.GetClaimsAsync(userId);
                    var existingEmailClaim = emailClaim.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                    if (existingEmailClaim != null)
                    {
                        await UserManager.RemoveClaimAsync(userId, existingEmailClaim);
                    }
                    await UserManager.AddClaimAsync(userId, new Claim(ClaimTypes.Email, model.Email));
                }

                // Enviar correo de confirmación al nuevo correo si no es el del admin
                if (emailCambiado && !nuevoEsAdmin)
                {
                    try
                    {
                        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = token }, Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirme su nuevo correo - Hotel Prado",
                            "Hola,<br/><br/>Para confirmar su nuevo correo haga clic en el siguiente enlace:<br/><br/><a href=\"" + callbackUrl + "\">Confirmar correo</a><br/><br/>Si no solicitó este cambio, ignore este mensaje.");
                    }
                    catch { /* Si falla el envío, el usuario ya tiene el perfil actualizado; puede usar "Reenviar confirmación" si existe */ }
                    return RedirectToAction("Index", new { message = ManageMessageId.ChangeEmailSendConfirmation });
                }

                return RedirectToAction("Index", new { message = ManageMessageId.ChangePasswordSuccess });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(GetCurrentUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(GetCurrentUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generar el token y enviarlo
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(GetCurrentUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Su código de seguridad es: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(GetCurrentUserId(), true);
            var user = await UserManager.FindByIdAsync(GetCurrentUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(GetCurrentUserId(), false);
            var user = await UserManager.FindByIdAsync(GetCurrentUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(GetCurrentUserId(), phoneNumber);
            // Enviar un SMS a través del proveedor de SMS para verificar el número de teléfono
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(GetCurrentUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(GetCurrentUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            ModelState.AddModelError("", "No se ha podido comprobar el teléfono");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(GetCurrentUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(GetCurrentUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(GetCurrentUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(GetCurrentUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(GetCurrentUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(GetCurrentUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Se ha quitado el inicio de sesión externo."
                : message == ManageMessageId.Error ? "Se ha producido un error."
                : "";
            var user = await UserManager.FindByIdAsync(GetCurrentUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(GetCurrentUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Solicitar la redirección al proveedor de inicio de sesión externo para vincular un inicio de sesión para el usuario actual
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), GetCurrentUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, GetCurrentUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(GetCurrentUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(GetCurrentUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(GetCurrentUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ChangeEmailSendConfirmation,
            Error
        }

#endregion
    }
}