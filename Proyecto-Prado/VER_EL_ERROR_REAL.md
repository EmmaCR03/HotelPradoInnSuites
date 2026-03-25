# Cómo ver el error REAL en el servidor

Sin el mensaje de error exacto estamos adivinando. Sigue estos pasos para ver qué está fallando.

---

## Paso 1: Republicar y subir

1. En Visual Studio: **Release** → **Rebuild Solution** → **Publish** a PradoPubli.
2. En FileZilla: sube **todo** el contenido de PradoPubli a **/site1** (sobrescribir todo).
3. Asegúrate de que **Web.config** se suba (si te pide “sobrescribir”, di que sí; la cadena de conexión ya es la de producción en tu proyecto).

---

## Paso 2: Ver el error paso a paso

En el navegador, abre esta URL en tu sitio:

**`http://pradoinn-001-site1.stempurl.com/VerErrorRegister`**

(Usa tu URL real si es distinta.)

Esa página hace lo mismo que Register pero en 4 pasos y, si algo falla, **muestra el error en texto** (mensaje, tipo de excepción, stack).

- Si ves **“1. OWIN: FALLO”** → el problema es OWIN en el servidor.
- Si ves **“2. Base de datos: FALLO”** → el problema es la conexión a SQL (cadena de conexión, firewall, usuario/contraseña).
- Si ves **“3. UserManager: FALLO”** → el problema es Identity/OWIN.
- Si ves **“4. Vista Register: FALLO”** → el problema es la vista o algo que usa.

Copia **todo** el texto que salga en esa página (sobre todo el “Mensaje”, “Tipo” y “Stack”) y guárdalo o envíamelo.

---

## Paso 3: Si /VerErrorRegister también da 500

Entonces el fallo es muy al inicio (arranque de la app o IIS). Haz esto:

1. En FileZilla, abre el **Web.config** que está en **/site1** en el servidor.
2. Busca la sección **`<customErrors`**.
3. Ponla así (aunque ya diga algo parecido):
   ```xml
   <customErrors mode="Off" defaultRedirect="~/Error">
     <error statusCode="404" redirect="~/Error" />
     <error statusCode="500" redirect="~/Error" />
   </customErrors>
   ```
   Lo importante: **mode="Off"**.
4. Guarda el archivo en el servidor.
5. Vuelve a abrir **`/Account/Register`** (o **`/VerErrorRegister`**).
6. Ahora IIS debería mostrar la “página amarilla” de error de ASP.NET con el mensaje y el stack completo. **Copia todo** ese mensaje.

---

## Paso 4: Revisar logs en el panel

En el panel de Smarterasp / Site4Now:

1. Busca **“Logs”**, **“Error Log”** o **“IIS Logs”**.
2. Abre el log más reciente y busca líneas del momento en que cargaste `/Account/Register` o `/VerErrorRegister`.
3. Ahí suele salir el mismo error. Copia esas líneas.

---

## Resumen

| Qué hacer | Para qué |
|-----------|----------|
| Abrir **/VerErrorRegister** | Ver en qué paso falla (OWIN, BD, UserManager, vista) y el texto del error. |
| Poner **customErrors mode="Off"** en Web.config del servidor | Ver la “página amarilla” con el error completo si todo lo demás da 500. |
| Revisar **logs** en el panel | Obtener el mismo error desde el servidor. |

Cuando tengas **el mensaje de error exacto** (de /VerErrorRegister, de la página amarilla o del log), con eso se puede ver la causa y decirte el cambio concreto (código o configuración).
