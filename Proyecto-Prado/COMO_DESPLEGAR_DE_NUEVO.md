# Cómo desplegar de nuevo - Hotel Prado

## 1. Publicar desde Visual Studio

1. **Abrir la solución** en Visual Studio (`HotelPrado.sln` o el .sln del proyecto).
2. **Configuración Release**
   - En la barra superior: cambiar de "Debug" a **Release**.
3. **Limpiar y compilar**
   - Menú **Build** → **Clean Solution**.
   - Menú **Build** → **Build Solution** (o Ctrl+Shift+B).
   - Revisar que no haya errores en la ventana "Error List".
4. **Publicar**
   - Clic derecho en el proyecto **HotelPrado.UI** → **Publish** (Publicar).
   - Si aparece un perfil (por ejemplo "FolderProfile"), seleccionarlo y **Publish**.
   - Si no hay perfil:
     - **Target**: "Folder".
     - **Location**: elegir una carpeta (ej: `C:\Users\emmag\OneDrive\Documentos\PradoPubli` o una nueva).
     - **Publish**.

Al terminar, en la carpeta de publicación tendrás todo lo necesario: `bin`, `Content`, `Scripts`, `Views`, `Web.config`, `Global.asax`, `Img`, etc.

---

## 2. Subir al hosting

Depende de tu proveedor (por ejemplo el que te da `pradoinn-001-site1.stempurl.com`):

### Si usas **administración de archivos (File Manager / FTP)**:
- Sube **todo el contenido** de la carpeta de publicación (no la carpeta en sí, sino lo que hay dentro).
- La raíz del sitio debe contener: `bin`, `Content`, `Scripts`, `Views`, `Web.config`, `Global.asax`, etc.
- No olvides **Web.config** y la carpeta **bin** con todas las DLLs.

### Si usas **Web Deploy (IIS)**:
- En Visual Studio: Publicar → elegir **IIS, FTP, etc.** y configurar servidor, sitio y credenciales según te indique tu hosting.

---

## 3. Revisar en el servidor

1. **Web.config**
   - En el servidor, revisa que la **cadena de conexión** en `Web.config` sea la de producción (servidor y base de datos correctos).
2. **Probar**
   - Abre la URL del sitio (ej: `http://pradoinn-001-site1.stempurl.com/`).
   - Prueba la página de inicio y **Account/Login** para confirmar que ya no hay bucle de redirección.

---

## Resumen rápido

| Paso | Acción |
|------|--------|
| 1 | Visual Studio → Release → Clean Solution → Build Solution |
| 2 | Clic derecho en **HotelPrado.UI** → Publish → Folder → Publicar |
| 3 | Subir todo el contenido de la carpeta de publicación al hosting |
| 4 | Verificar `Web.config` (cadena de conexión) en el servidor |
| 5 | Probar la web y el login |

Si tu hosting es con **cPanel** o **Negox**, tienes más detalle en `GUIA_DESPLIEGUE_CPANEL.md` y `GUIA_DESPLIEGUE_NEGOX.md`.
