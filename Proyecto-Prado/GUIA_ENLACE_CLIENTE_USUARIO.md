# 🔗 Guía: Enlazar Cliente con AspNetUsers

## 📋 Objetivo

Permitir que los clientes puedan:
- ✅ Acceder al sistema web con su cuenta
- ✅ Solicitar reservas en línea
- ✅ Ver sus reservas históricas
- ✅ El admin puede crear reservas manualmente para cualquier cliente

## 🏗️ Estructura

### Relación
```
Cliente (1) ←→ (0..1) AspNetUsers
```

- Un cliente puede tener **un usuario** (opcional)
- Un usuario puede estar vinculado a **un cliente**
- Si un cliente no tiene usuario, el admin puede crear reservas manualmente

### Campos Agregados

**Tabla Cliente:**
- `IdUsuario` (NVARCHAR(128), NULL) - Foreign Key a `AspNetUsers.Id`

## 📝 Pasos para Implementar

### 1. Ejecutar Script SQL

```sql
-- Crear el enlace
EXEC DB/dbo/Tables/Cliente_Enlazar_AspNetUsers.sql

-- Vincular clientes existentes (opcional)
EXEC DB/dbo/Tables/Vincular_Clientes_Usuarios.sql
```

### 2. Flujo para Clientes Nuevos

#### Opción A: Cliente se Registra en el Sistema Web
1. Cliente crea cuenta en `AspNetUsers` (registro normal)
2. Al crear la cuenta, automáticamente crear registro en `Cliente`
3. Vincular `Cliente.IdUsuario` con `AspNetUsers.Id`

#### Opción B: Admin Crea Cliente Manualmente
1. Admin crea registro en `Cliente` (sin usuario)
2. Si el cliente quiere acceso web, admin crea usuario en `AspNetUsers`
3. Vincular ambos registros

### 3. Flujo para Clientes Existentes (Migrados)

1. Ejecutar `Vincular_Clientes_Usuarios.sql` para vincular automáticamente por email/cédula
2. Para clientes sin vincular:
   - Si quieren acceso web: crear usuario y vincular
   - Si no: dejar sin vincular (admin puede crear reservas manualmente)

## 💻 Código de Ejemplo (C#)

### Crear Cliente con Usuario

```csharp
// Al registrar un nuevo usuario
public async Task<IdentityResult> RegistrarCliente(string email, string password, 
    string nombre, string cedula, string telefono)
{
    // 1. Crear usuario en AspNetUsers
    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        cedula = cedula,
        NombreCompleto = nombre,
        Telefono = telefono
    };
    
    var result = await UserManager.CreateAsync(user, password);
    
    if (result.Succeeded)
    {
        // 2. Crear cliente y vincular
        var cliente = new Cliente
        {
            NombreCliente = nombre,
            EmailCliente = email,
            CedulaCliente = cedula,
            TelefonoCliente = telefono,
            IdUsuario = user.Id  // ← VINCULACIÓN
        };
        
        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();
    }
    
    return result;
}
```

### Obtener Cliente desde Usuario Actual

```csharp
// En un controlador
public async Task<IActionResult> MisReservas()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    var cliente = await _context.Cliente
        .Include(c => c.Reservas)
        .FirstOrDefaultAsync(c => c.IdUsuario == userId);
    
    if (cliente == null)
    {
        return NotFound("Cliente no encontrado");
    }
    
    return View(cliente.Reservas);
}
```

### Crear Reserva (Cliente o Admin)

```csharp
// Cliente crea su propia reserva
public async Task<IActionResult> CrearReserva(ReservaViewModel model)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    var cliente = await _context.Cliente
        .FirstOrDefaultAsync(c => c.IdUsuario == userId);
    
    if (cliente == null)
    {
        return Unauthorized();
    }
    
    var reserva = new Reservas
    {
        IdCliente = cliente.IdCliente,  // ← Usa IdCliente (INT)
        FechaInicio = model.FechaInicio,
        FechaFinal = model.FechaFinal,
        // ... otros campos
    };
    
    _context.Reservas.Add(reserva);
    await _context.SaveChangesAsync();
    
    return RedirectToAction("MisReservas");
}

// Admin crea reserva para cualquier cliente
public async Task<IActionResult> CrearReservaAdmin(int idCliente, ReservaViewModel model)
{
    // Admin puede crear reserva para cualquier cliente
    // No necesita que el cliente tenga usuario
    var reserva = new Reservas
    {
        IdCliente = idCliente,  // ← Puede ser cualquier cliente
        FechaInicio = model.FechaInicio,
        FechaFinal = model.FechaFinal,
        // ... otros campos
    };
    
    _context.Reservas.Add(reserva);
    await _context.SaveChangesAsync();
    
    return RedirectToAction("Reservas");
}
```

## 🔍 Consultas Útiles

### Ver Clientes con Usuario
```sql
SELECT 
    c.IdCliente,
    c.NombreCliente,
    c.EmailCliente,
    u.UserName,
    u.Email
FROM Cliente c
INNER JOIN AspNetUsers u ON c.IdUsuario = u.Id;
```

### Ver Clientes sin Usuario
```sql
SELECT 
    IdCliente,
    NombreCliente,
    EmailCliente,
    CedulaCliente
FROM Cliente
WHERE IdUsuario IS NULL;
```

### Contar Reservas por Tipo de Cliente
```sql
SELECT 
    CASE 
        WHEN c.IdUsuario IS NOT NULL THEN 'Con Usuario Web'
        ELSE 'Solo Admin'
    END AS TipoCliente,
    COUNT(r.IdReserva) AS TotalReservas
FROM Cliente c
LEFT JOIN Reservas r ON c.IdCliente = r.IdCliente
GROUP BY CASE 
    WHEN c.IdUsuario IS NOT NULL THEN 'Con Usuario Web'
    ELSE 'Solo Admin'
END;
```

## ⚠️ Consideraciones

1. **Unicidad**: Un usuario solo puede estar vinculado a un cliente (índice único)
2. **Opcional**: `IdUsuario` es NULL, permitiendo clientes sin acceso web
3. **Eliminación**: Si se elimina un usuario, el cliente se desvincula (ON DELETE SET NULL)
4. **Búsqueda**: Se puede buscar cliente por `IdUsuario` o por datos normales

## ✅ Ventajas de esta Estructura

- ✅ Clientes pueden acceder al sistema web
- ✅ Admin puede crear reservas para cualquier cliente (con o sin usuario)
- ✅ Historial completo de reservas por cliente
- ✅ Flexibilidad: no todos los clientes necesitan usuario
- ✅ Seguridad: autenticación manejada por ASP.NET Identity


