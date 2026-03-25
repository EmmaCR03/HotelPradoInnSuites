# 📝 Instrucciones para Actualizar Bitácora con Usuario

## ✅ Cambios Completados

1. ✅ Campo `Usuario` agregado a la tabla `bitacoraEventos`
2. ✅ Modelos `BitacoraEventosDTO` y `BitacoraTabla` actualizados
3. ✅ Vista `IndexBitacoraEventos` actualizada para mostrar usuario
4. ✅ Helper `BitacoraHelper` creado para obtener usuario actual

## 🔧 Pasos para Completar la Implementación

### Paso 1: Ejecutar Script SQL

Ejecuta el script SQL para agregar la columna `Usuario` a la tabla:

```sql
-- Archivo: DB/dbo/Tables/ALTER_bitacoraEventos_AgregarUsuario.sql
```

**Instrucciones:**
1. Abre SQL Server Management Studio (SSMS)
2. Conéctate a tu base de datos `HotelPrado`
3. Abre el archivo `ALTER_bitacoraEventos_AgregarUsuario.sql`
4. Ejecuta el script (F5)

### Paso 2: Actualizar Controladores

Necesitas actualizar **todos los lugares** donde se crea un `BitacoraEventosDTO` para incluir el usuario.

#### Ejemplo de Actualización:

**ANTES:**
```csharp
var bitacora = new BitacoraEventosDTO
{
    IdEvento = 0,
    TablaDeEvento = "ModuloHabitaciones",
    TipoDeEvento = "Creación",
    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
    DescripcionDeEvento = "Se creó una nueva habitación.",
    StackTrace = "Sin errores",
    DatosAnteriores = "NA",
    DatosPosteriores = datos
};
```

**DESPUÉS:**
```csharp
using HotelPrado.UI.Helpers; // Agregar al inicio del archivo

var bitacora = new BitacoraEventosDTO
{
    IdEvento = 0,
    TablaDeEvento = "ModuloHabitaciones",
    TipoDeEvento = "Creación",
    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
    DescripcionDeEvento = "Se creó una nueva habitación.",
    StackTrace = "Sin errores",
    DatosAnteriores = "NA",
    DatosPosteriores = datos,
    Usuario = BitacoraHelper.ObtenerUsuarioActual() // ← AGREGAR ESTA LÍNEA
};
```

### Paso 3: Archivos a Actualizar

Busca todos los archivos que contienen `new BitacoraEventosDTO`:

**Controladores:**
- `HotelPrado.UI/Controllers/HabitacionController.cs` (✅ Ejemplo actualizado)
- `HotelPrado.UI/Controllers/DepartamentoController.cs`
- `HotelPrado.UI/Controllers/ReservasAController.cs`
- `HotelPrado.UI/Controllers/CargosController.cs`
- `HotelPrado.UI/Controllers/SolicitudLimpiezaController.cs`

**Lógica de Negocio:**
- `HotelPrado.LN/Facturas/Registrar/RegistrarFacturasLN.cs`
- `HotelPrado.LN/Facturas/Editar/EditarFacturasLN.cs`
- `HotelPrado.LN/Facturas/Eliminar/EliminarFacturasLN.cs`
- `HotelPrado.LN/Reservas/Registrar/RegistrarReservaLN.cs`
- `HotelPrado.LN/Temporadas/Registrar/RegistrarTemporadasLN.cs`
- `HotelPrado.LN/Temporadas/Editar/EditarTemporadasLN.cs`
- `HotelPrado.LN/Temporadas/Eliminar/EliminarTemporadasLN.cs`
- `HotelPrado.LN/SolicitudLimpieza/Registrar/RegistrarSolicitudLimpiezaLN.cs`
- `HotelPrado.LN/Citas/Registrar/RegistrarCitasLN.cs`
- `HotelPrado.LN/Colaborador/Registrar/RegistrarColaboradorLN.cs`
- `HotelPrado.LN/Habitaciones/Registrar/RegistrarHabitacionesLN.cs`
- `HotelPrado.LN/Departamentos/Registrar/RegistrarDepartamentoLN.cs`
- `HotelPrado.LN/Cargos/Registrar/RegistrarCargosLN.cs`
- `HotelPrado.LN/Mantenimiento/Registrar/RegistrarMantenimientoLN.cs`

### Paso 4: Agregar Using Statement

En cada archivo que uses `BitacoraHelper`, agrega al inicio:

```csharp
using HotelPrado.UI.Helpers;
```

**Nota:** Para archivos en la capa `LN` (Lógica de Negocio), necesitarás pasar el usuario como parámetro desde el controlador, ya que `LN` no debería tener dependencia de `UI`.

### Paso 5: Verificar Funcionamiento

1. Inicia sesión como administrador
2. Realiza alguna acción que genere un evento en la bitácora (crear, editar, eliminar)
3. Ve a **Bitácora de Eventos**
4. Verifica que la columna "Usuario" muestre tu nombre de usuario

## 🔍 Búsqueda Rápida

Para encontrar todos los lugares donde necesitas hacer cambios, usa esta búsqueda en Visual Studio:

**Buscar:** `new BitacoraEventosDTO`
**Reemplazar con cuidado:** Agregar `Usuario = BitacoraHelper.ObtenerUsuarioActual(),` antes del cierre de llaves

## ⚠️ Notas Importantes

1. **Para capa LN**: Si el código está en la capa de Lógica de Negocio, el usuario debe pasarse como parámetro desde el controlador, no usar `BitacoraHelper` directamente.

2. **Valores por defecto**: Si no hay usuario autenticado, se guardará "Sistema" automáticamente.

3. **Datos existentes**: Los registros antiguos en la bitácora mostrarán "Sistema" como usuario (valor por defecto).

## ✅ Checklist de Verificación

- [ ] Script SQL ejecutado
- [ ] Helper `BitacoraHelper` compilado correctamente
- [ ] Al menos un controlador actualizado como ejemplo
- [ ] Vista muestra columna de usuario
- [ ] Modal muestra usuario en detalles
- [ ] Probar crear un evento y verificar que se guarda el usuario

---

**Última actualización**: 2024
**Estado**: Parcialmente implementado - Pendiente actualizar todos los controladores











