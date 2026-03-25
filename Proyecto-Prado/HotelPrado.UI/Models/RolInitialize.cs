using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelPrado.UI.Models
{
    public class RolInitialize
    {
        public static void Inicializar()
        {
            var context = new ApplicationDbContext();
            var rolManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Lista de roles
            List<string> roles = new List<string>
            {
                "Administrador",
                "Usuarios",
                "Colaborador",
                "Cliente"
            };

            
            foreach (var rol in roles)
            {
                if (!rolManager.RoleExists(rol))
                {
                    var roleResult = rolManager.Create(new IdentityRole(rol));
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Error al crear el rol: {rol}");
                    }
                }
            }

            // Crear el usuario admin si no existe
            var adminUser = new ApplicationUser
            {
                UserName = "adminHotel@hotelPrado.com",
                Email = "adminHotel@hotelPrado.com",
                Telefono="809-5555",
                NombreCompleto = "Admin Hotel",
                cedula = "001234569"
            };

            string password = "Admin123456!"; // Contraseña inicial

            // Verificar si el usuario ya existe
            var existingUser = userManager.FindByEmail(adminUser.Email);
            if (existingUser == null)
            {
                // Intentar crear el usuario admin
                var result = userManager.Create(adminUser, password);
                if (result.Succeeded)
                {
                    // Admin es excepción: no tiene correo real, marcar como confirmado para que pueda iniciar sesión
                    var adminCreado = userManager.FindById(adminUser.Id);
                    if (adminCreado != null)
                    {
                        adminCreado.EmailConfirmed = true;
                        userManager.UpdateAsync(adminCreado).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    // Asignar rol al usuario admin
                    // Usar ConfigureAwait(false) para evitar deadlocks en Application_Start
                    var addRoleResult = userManager.AddToRoleAsync(adminUser.Id, "Administrador").ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!addRoleResult.Succeeded)
                    {
                        Console.WriteLine($"Error al asignar el rol al usuario admin: {string.Join(", ", addRoleResult.Errors)}");
                    }
                    else
                    {
                        Console.WriteLine("Usuario admin creado correctamente y rol asignado.");
                    }
                }
                else
                {
                    // Mostrar los errores de la creación del usuario
                    Console.WriteLine("Error al crear el usuario:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                }
            }
            else
            {
                Console.WriteLine("El usuario con ese correo ya existe.");
            }

            // Asignar automáticamente el rol "Cliente" a usuarios sin rol - Solo si hay usuarios
            // Comentado temporalmente para mejorar rendimiento
            // AsignarRolClienteAUsuariosSinRol();
        }

        /// <summary>
        /// Asigna automáticamente el rol "Cliente" a todos los usuarios que no tienen ningún rol asignado
        /// </summary>
        public static void AsignarRolClienteAUsuariosSinRol()
        {
            var context = new ApplicationDbContext();
            var rolManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Asegurar que el rol "Cliente" existe
            if (!rolManager.RoleExists("Cliente"))
            {
                rolManager.Create(new IdentityRole("Cliente"));
            }

            // Obtener todos los usuarios
            var todosLosUsuarios = userManager.Users.ToList();
            int usuariosActualizados = 0;

            foreach (var usuario in todosLosUsuarios)
            {
                var rolesDelUsuario = userManager.GetRoles(usuario.Id);
                
                // Si el usuario no tiene ningún rol, asignarle "Cliente"
                if (rolesDelUsuario == null || rolesDelUsuario.Count == 0)
                {
                    var resultado = userManager.AddToRole(usuario.Id, "Cliente");
                    if (resultado.Succeeded)
                    {
                        usuariosActualizados++;
                    }
                    else
                    {
                        Console.WriteLine($"Error al asignar rol Cliente al usuario {usuario.Email}: {string.Join(", ", resultado.Errors)}");
                    }
                }
            }

            if (usuariosActualizados > 0)
            {
                Console.WriteLine($"Se asignó el rol 'Cliente' a {usuariosActualizados} usuario(s) sin rol.");
            }
        }
    }
}
