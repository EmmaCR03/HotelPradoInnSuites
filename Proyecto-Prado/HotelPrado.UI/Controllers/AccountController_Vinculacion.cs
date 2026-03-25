// =============================================
// CÓDIGO PARA AGREGAR AL AccountController.cs
// Modifica el método Register para vincular clientes migrados
// =============================================

// Agregar estos using al inicio del archivo:
using HotelPrado.AccesoADatos;
using System.Data.Entity;
using System.Data.SqlClient;

// Modificar el método Register (línea ~150) para incluir esta lógica:

[HttpPost]
[AllowAnonymous]
public async Task<ActionResult> Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = new ApplicationUser { 
            UserName = model.Email, 
            Email = model.Email,
            NombreCompleto = model.NombreCompleto,
            cedula = model.cedula,
            Telefono = model.Telefono
        };
        var result = await UserManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // ==========================================
            // NUEVO: Buscar y vincular cliente existente
            // ==========================================
            try
            {
                using (var contexto = new Contexto())
                {
                    // Buscar cliente por email
                    var clienteExistente = await contexto.Database.SqlQuery<ClienteInfo>(
                        "SELECT IdCliente, EmailCliente, CedulaCliente FROM Cliente WHERE " +
                        "(LOWER(LTRIM(RTRIM(EmailCliente))) = @email OR " +
                        "LTRIM(RTRIM(CedulaCliente)) = @cedula) AND IdUsuario IS NULL",
                        new SqlParameter("@email", model.Email?.ToLower() ?? ""),
                        new SqlParameter("@cedula", model.cedula?.Trim() ?? "")
                    ).FirstOrDefaultAsync();

                    if (clienteExistente != null)
                    {
                        // Vincular cliente existente con el nuevo usuario
                        await contexto.Database.ExecuteSqlCommandAsync(
                            "UPDATE Cliente SET IdUsuario = @userId WHERE IdCliente = @clienteId",
                            new SqlParameter("@userId", user.Id),
                            new SqlParameter("@clienteId", clienteExistente.IdCliente)
                        );
                    }
                    else
                    {
                        // Crear nuevo cliente si no existe
                        await contexto.Database.ExecuteSqlCommandAsync(
                            "INSERT INTO Cliente (NombreCliente, EmailCliente, CedulaCliente, TelefonoCliente, IdUsuario) " +
                            "VALUES (@nombre, @email, @cedula, @telefono, @userId)",
                            new SqlParameter("@nombre", model.NombreCompleto ?? ""),
                            new SqlParameter("@email", model.Email ?? ""),
                            new SqlParameter("@cedula", model.cedula ?? ""),
                            new SqlParameter("@telefono", model.Telefono ?? ""),
                            new SqlParameter("@userId", user.Id)
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
            
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index", "Home");
        }
        AddErrors(result);
    }
    return View(model);
}

// Clase auxiliar para el resultado de la consulta
public class ClienteInfo
{
    public int IdCliente { get; set; }
    public string EmailCliente { get; set; }
    public string CedulaCliente { get; set; }
}

