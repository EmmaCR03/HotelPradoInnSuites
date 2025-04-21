
using HotelPrado.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsuarioController()
        {
        }

        public UsuarioController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // GET: Usuario
        public ActionResult Index()
        {
            var users = UserManager.Users.ToList();

            var usuarioConRoles = users.Select(user => new UsuarioConRoles
            {
                id = user.Id,
                UserName = user.UserName,
                NombreCompleto = user.NombreCompleto,
                cedula = user.cedula,
                Email = user.Email,
                Telefono = user.Telefono,
                Roles = UserManager.GetRoles(user.Id).ToList()
            }).ToList();

            return View(usuarioConRoles);
        }
        public ActionResult Create()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var roles = roleManager.Roles.ToList();

            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            });

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var existingUser = userManager.FindByEmail(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Ya existe un usuario con ese correo electrónico.");
                }

                if (ModelState.IsValid)
                {

                    model.UserName = model.UserName?.Trim();
                    model.NombreCompleto = model.NombreCompleto?.Trim();
                    model.cedula = model.cedula?.Trim();
                    model.Email = model.Email?.Trim();
                    model.Telefono = model.Telefono?.Trim();

                    var user = new ApplicationUser
                    {

                        UserName = model.Email,
                        NombreCompleto = model.NombreCompleto,
                        cedula = model.cedula,
                        Email = model.Email,
                        Telefono = model.Telefono
                    };

                    

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        var addRoleResult = userManager.AddToRole(user.Id, model.Rol);
                        if (addRoleResult.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            ViewBag.Roles = roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound("Se requiere un ID de usuario para editar.");
            }

            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound("El usuario no existe.");
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var roles = roleManager.Roles.ToList();

            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            });

            var model = new ApplicationUser
            {
                Id = user.Id,
                cedula = user.cedula,
                UserName = user.UserName,
                NombreCompleto = user.NombreCompleto,
                Telefono = user.Telefono,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser model, string Rol, string Password)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.Id);
                if (user == null)
                {
                    return HttpNotFound("El usuario no existe.");
                }
                user.cedula = model.cedula;
                user.UserName = model.UserName;
                user.NombreCompleto = model.NombreCompleto;
                user.Telefono = model.Telefono;

                if (!string.IsNullOrEmpty(Password))
                {
                    var passwordChangeResult = UserManager.RemovePassword(user.Id);
                    if (passwordChangeResult.Succeeded)
                    {
                        var newPasswordResult = UserManager.AddPassword(user.Id, Password);
                        if (!newPasswordResult.Succeeded)
                        {
                            foreach (var error in newPasswordResult.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            CargarRoles();
                            return View(model);
                        }
                    }
                    else
                    {
                        foreach (var error in passwordChangeResult.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        CargarRoles();
                        return View(model);
                    }
                }

                if (!string.IsNullOrEmpty(Rol))
                {
                    var currentRoles = UserManager.GetRoles(user.Id);
                    if (!currentRoles.Contains(Rol))
                    {
                        foreach (var role in currentRoles)
                        {
                            UserManager.RemoveFromRole(user.Id, role);
                        }

                        var addRoleResult = UserManager.AddToRole(user.Id, Rol);
                        if (!addRoleResult.Succeeded)
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            CargarRoles();
                            return View(model);
                        }
                    }
                }

                var updateResult = UserManager.Update(user);
                if (updateResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            CargarRoles();
            return View(model);
        }

        private void CargarRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var roles = roleManager.Roles.ToList();
            ViewBag.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            });
        }
        public ActionResult Delete(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = UserManager.FindById(id);
            if (user != null)
            {
                var result = UserManager.Delete(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


    }
}