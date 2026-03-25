# 🔗 Solución para Clientes Migrados sin Cuenta

## 📋 Problema

Tienes **1,839 clientes migrados** que:
- ✅ Están en la tabla `Cliente`
- ❌ NO tienen cuenta en `AspNetUsers`
- ❌ NO pueden acceder al sistema web

## ✅ Solución: Vinculación Automática

### Opción 1: Registro Automático (Recomendada)

Cuando un cliente migrado se registre en el sistema web:
1. El sistema busca si existe un cliente con ese email o cédula
2. Si existe, lo vincula automáticamente con el nuevo usuario
3. Si no existe, crea un nuevo cliente

**Ventajas:**
- ✅ No requiere intervención manual
- ✅ Los clientes se vinculan automáticamente al registrarse
- ✅ Mantiene el historial de reservas existente

### Opción 2: Admin Crea Usuarios

El admin puede crear usuarios para clientes existentes:
1. Admin crea usuario en el sistema
2. Sistema busca cliente por email/cédula
3. Vincula automáticamente

### Opción 3: Script Masivo (No Recomendado)

Crear usuarios automáticamente para todos los clientes migrados:
- ⚠️ Requiere generar contraseñas temporales
- ⚠️ Los clientes deben cambiar contraseña
- ⚠️ Puede crear usuarios no deseados

## 🚀 Implementación

### Paso 1: Ejecutar Script SQL

```sql
-- Crear el enlace entre Cliente y AspNetUsers
EXEC DB/dbo/Tables/Cliente_Enlazar_AspNetUsers.sql
```

### Paso 2: Modificar AccountController

Agregar el código de `AccountController_Vinculacion.cs` al método `Register` en `AccountController.cs`.

**Ubicación:** `HotelPrado.UI/Controllers/AccountController.cs` (línea ~150)

### Paso 3: Probar

1. Intentar registrarse con un email de un cliente migrado
2. Verificar que se vincula automáticamente
3. El cliente puede ver sus reservas históricas

## 📊 Flujo Completo

```
Cliente Migrado (sin usuario)
    ↓
Cliente se registra en el sistema web
    ↓
Sistema busca: ¿Existe cliente con este email/cédula?
    ↓
    ├─ SÍ → Vincular cliente existente con nuevo usuario
    │        ✅ Mantiene historial de reservas
    │        ✅ Puede ver reservas anteriores
    │
    └─ NO → Crear nuevo cliente
             ✅ Cliente nuevo con usuario
```

## 🔍 Consultas Útiles

### Ver Clientes Migrados sin Usuario
```sql
SELECT 
    IdCliente,
    NombreCliente,
    EmailCliente,
    CedulaCliente,
    COUNT(r.IdReserva) AS TotalReservas
FROM Cliente c
LEFT JOIN Reservas r ON c.IdCliente = r.IdCliente
WHERE c.IdUsuario IS NULL
GROUP BY IdCliente, NombreCliente, EmailCliente, CedulaCliente
ORDER BY TotalReservas DESC;
```

### Ver Clientes Vinculados
```sql
SELECT 
    c.IdCliente,
    c.NombreCliente,
    c.EmailCliente,
    u.UserName,
    u.Email,
    COUNT(r.IdReserva) AS TotalReservas
FROM Cliente c
INNER JOIN AspNetUsers u ON c.IdUsuario = u.Id
LEFT JOIN Reservas r ON c.IdCliente = r.IdCliente
GROUP BY c.IdCliente, c.NombreCliente, c.EmailCliente, u.UserName, u.Email;
```

### Estadísticas
```sql
SELECT 
    'Total Clientes' AS Tipo,
    COUNT(*) AS Cantidad
FROM Cliente
UNION ALL
SELECT 
    'Con Usuario Web' AS Tipo,
    COUNT(*) AS Cantidad
FROM Cliente
WHERE IdUsuario IS NOT NULL
UNION ALL
SELECT 
    'Sin Usuario Web' AS Tipo,
    COUNT(*) AS Cantidad
FROM Cliente
WHERE IdUsuario IS NULL;
```

## ⚠️ Consideraciones

1. **Email/Cédula Únicos**: Asegúrate de que no haya duplicados
2. **Validación**: El sistema valida que el email/cédula coincidan
3. **Seguridad**: Solo vincula si el cliente no tiene usuario ya
4. **Historial**: Las reservas antiguas se mantienen vinculadas al cliente

## 🎯 Resultado Final

- ✅ Clientes migrados pueden registrarse y vincularse automáticamente
- ✅ Mantienen su historial de reservas
- ✅ Pueden crear nuevas reservas
- ✅ Admin puede crear reservas para clientes sin usuario
- ✅ Sistema flexible: funciona con o sin usuario


