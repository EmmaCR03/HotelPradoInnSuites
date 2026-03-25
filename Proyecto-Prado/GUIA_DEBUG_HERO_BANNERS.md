# 🔍 Guía de Debugging para Hero Banners

## 📍 Puntos Clave para Breakpoints

### 1. **Subida de Imagen (Backend)**
**Archivo:** `HotelPrado.UI/Controllers/AdminController.cs`

#### Breakpoint 1: Inicio del método
```csharp
// Línea 63 - POST: Admin/SubirImagenHeroBanner
public ActionResult SubirImagenHeroBanner(int id, HttpPostedFileBase imagen)
```
**Inspecciona:**
- `id` - ¿Es el ID correcto?
- `imagen` - ¿No es null?
- `imagen.FileName` - ¿Nombre del archivo correcto?
- `imagen.ContentLength` - ¿Tamaño del archivo?

#### Breakpoint 2: Después de guardar archivo
```csharp
// Línea 92 - Después de imagen.SaveAs(filePath);
```
**Inspecciona:**
- `filePath` - ¿Ruta completa correcta?
- `urlImagen` - ¿URL relativa correcta? (debe ser `/Img/hero-banners/hero_X_YYYYMMDDHHMMSS.jpg`)

#### Breakpoint 3: Antes de guardar en BD
```csharp
// Línea 100 - var configuracion = contexto.ConfiguracionHeroBannerTabla.Find(id);
```
**Inspecciona:**
- `configuracion` - ¿No es null?
- `configuracion.UrlImagen` - ¿Valor anterior?

#### Breakpoint 4: Después de SaveChanges
```csharp
// Línea 110 - contexto.SaveChanges();
```
**Inspecciona:**
- `configuracion.UrlImagen` - ¿Se actualizó correctamente?
- Verifica en BD: `SELECT * FROM ConfiguracionHeroBanner WHERE IdConfiguracion = [id]`

---

### 2. **Actualización por URL (Backend)**
**Archivo:** `HotelPrado.UI/Controllers/AdminController.cs`

#### Breakpoint 5: Inicio del método
```csharp
// Línea 124 - POST: Admin/ActualizarHeroBanner
public ActionResult ActualizarHeroBanner(int id, string urlImagen)
```
**Inspecciona:**
- `id` - ¿ID correcto?
- `urlImagen` - ¿URL no está vacía?

#### Breakpoint 6: Después de SaveChanges
```csharp
// Línea 145 - contexto.SaveChanges();
```
**Inspecciona:**
- `configuracion.UrlImagen` - ¿Se actualizó?

---

### 3. **Carga de Hero Banner (Backend)**
**Archivo:** `HotelPrado.UI/Controllers/HomeController.cs`

#### Breakpoint 7: Método ObtenerHeroBanner
```csharp
// Línea 13 - private string ObtenerHeroBanner(string pagina)
```
**Inspecciona:**
- `pagina` - ¿Nombre correcto? ("Home", "Habitaciones", "Contacto", etc.)

#### Breakpoint 8: Después de consultar BD
```csharp
// Línea 24 - .FirstOrDefault();
```
**Inspecciona:**
- `config` - ¿No es null?
- `config.UrlImagen` - ¿URL correcta?
- `config.FechaActualizacion` - ¿Es la más reciente?

#### Breakpoint 9: En cada Action
```csharp
// Líneas 38, 45, 52, 58
ViewBag.HeroBannerUrl = ObtenerHeroBanner("...");
```
**Inspecciona:**
- `ViewBag.HeroBannerUrl` - ¿Tiene el valor correcto?

---

### 4. **JavaScript (Frontend)**
**Archivo:** `HotelPrado.UI/Views/Admin/ConfigurarHeroBanners.cshtml`

#### Breakpoint 10: Inicio de guardarBanner
```javascript
// Línea 228 - function guardarBanner(id)
```
**Inspecciona en consola del navegador (F12):**
- `id` - ¿ID correcto?
- `file` - ¿Archivo seleccionado?
- `urlImagen` - ¿URL ingresada?

#### Breakpoint 11: Antes de fetch
```javascript
// Línea 247 - fetch('@Url.Action("SubirImagenHeroBanner", "Admin")'
```
**Inspecciona:**
- `formData` - ¿Tiene los datos correctos?
- Abre Network tab en DevTools para ver la petición

#### Breakpoint 12: Respuesta del servidor
```javascript
// Línea 252 - .then(data => {
```
**Inspecciona:**
- `data.success` - ¿Es true?
- `data.urlImagen` - ¿URL correcta?
- `data.message` - ¿Mensaje de éxito?

---

## 🛠️ Logs Temporales para Agregar

### En AdminController.cs - SubirImagenHeroBanner:

```csharp
// Después de línea 95
string urlImagen = $"/Img/hero-banners/{fileName}";
System.Diagnostics.Debug.WriteLine($"[DEBUG] URL Imagen generada: {urlImagen}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] FilePath completo: {filePath}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] Archivo existe: {System.IO.File.Exists(filePath)}");

// Después de línea 110
contexto.SaveChanges();
System.Diagnostics.Debug.WriteLine($"[DEBUG] Guardado en BD - URL: {configuracion.UrlImagen}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] Fecha Actualización: {configuracion.FechaActualizacion}");
```

### En HomeController.cs - ObtenerHeroBanner:

```csharp
// Después de línea 24
var config = contexto.ConfiguracionHeroBannerTabla
    .Where(c => c.Pagina == pagina)
    .OrderByDescending(c => c.FechaActualizacion)
    .FirstOrDefault();

System.Diagnostics.Debug.WriteLine($"[DEBUG] Página: {pagina}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] Config encontrada: {config != null}");
if (config != null)
{
    System.Diagnostics.Debug.WriteLine($"[DEBUG] URL Imagen: {config.UrlImagen}");
    System.Diagnostics.Debug.WriteLine($"[DEBUG] Fecha Actualización: {config.FechaActualizacion}");
}
```

---

## 🔎 Cómo Ver los Logs

1. **En Visual Studio:**
   - Abre la ventana "Output" (Ver → Output)
   - Selecciona "Debug" en el dropdown
   - Verás los mensajes `[DEBUG]`

2. **En el navegador (JavaScript):**
   - Abre DevTools (F12)
   - Ve a la pestaña "Console"
   - Verás los `console.log()`

3. **Network Tab (Peticiones HTTP):**
   - Abre DevTools (F12)
   - Ve a la pestaña "Network"
   - Filtra por "XHR" o "Fetch"
   - Haz clic en la petición para ver:
     - Request (lo que se envía)
     - Response (lo que se recibe)
     - Headers

---

## ✅ Checklist de Verificación

Cuando debuguees, verifica:

1. ✅ ¿El archivo se guarda físicamente en `~/Img/hero-banners/`?
2. ✅ ¿La URL en la BD es correcta?
3. ✅ ¿El ViewBag tiene el valor correcto en el controlador?
4. ✅ ¿La vista está usando `ViewBag.HeroBannerUrl`?
5. ✅ ¿El archivo existe en la ruta física?
6. ✅ ¿Hay errores 404 en Network tab?
7. ✅ ¿El caché del navegador está limpio?

---

## 🚨 Errores Comunes

### Error: "404 Not Found" en la imagen
- **Causa:** El archivo no existe en la ruta física
- **Solución:** Verifica que `Server.MapPath("~/Img/hero-banners/")` apunte a la carpeta correcta

### Error: La imagen no se actualiza
- **Causa:** Caché del navegador o ViewBag no se actualiza
- **Solución:** 
  - Ctrl + F5 para recarga forzada
  - Verifica que `AutoDetectChangesEnabled = true`
  - Verifica que se está consultando la BD en cada request

### Error: "Configuración no encontrada"
- **Causa:** El ID no existe en la BD
- **Solución:** Verifica que el ID sea correcto: `SELECT * FROM ConfiguracionHeroBanner`














