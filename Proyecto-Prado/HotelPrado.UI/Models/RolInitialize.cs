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
                "Colaborador"
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
                UserName = "AdminHotel",
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
                    // Asignar rol al usuario admin
                    var addRoleResult = userManager.AddToRoleAsync(adminUser.Id, "Administrador").Result;
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
        }
    }
}
