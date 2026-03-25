# Sesión: mismo comportamiento en local y en hosting

## Por qué en local funciona y en hosting a veces no

| Aspecto | Local (IIS Express) | Hosting (SmarterASP, etc.) |
|--------|----------------------|----------------------------|
| **machineKey** | Se genera solo y se mantiene con el proceso | El App Pool puede reciclarse y generar **otra** clave → la cookie de Identity no se puede descifrar → parece que no hay sesión |
| **Windows Auth** | Puede estar activa (usuario tipo WIN8232\...) | Normalmente no; solo cuenta la cookie de la app |
| **HTTPS** | Suele ser HTTP | Suele ser HTTPS → la cookie debe ser compatible (SameAsRequest ya lo es) |

## Cambios aplicados en el proyecto

1. **Solo “Hola” con sesión de la app**  
   Se usa `AuthenticationType == "ApplicationCookie"`. Si la única identidad es Windows (o no hay cookie), se muestran “Registrarse” e “Iniciar sesión”. Así se evita ver “Hola WIN8232\...” tras cerrar sesión en local.

2. **machineKey fijo solo en publicación**  
   En `Web.Release.config` se añade un `machineKey` cuando publicas en **Release**. En local (Debug) no se usa; en el servidor la cookie de Identity siempre se puede descifrar aunque el App Pool se recicle.

3. **Cookie de Identity**  
   En `Startup.Auth.cs`: `CookieSecure = SameAsRequest`, `CookieSameSite = Lax`, validación cada 24 h. Válido tanto para HTTP (local) como HTTPS (hosting).

## Qué hacer al publicar

- Publicar con configuración **Release** (para que se aplique la transformación de `Web.Release.config` y se inserte el `machineKey`).
- En el host, no hace falta tocar el `Web.config` a mano para el `machineKey` si ya publicas en Release.

## Si en hosting la sesión sigue sin mantenerse

1. **Comprobar que el sitio va por HTTPS**  
   Si el sitio es `https://...`, no acceder por `http://...` (la cookie puede no enviarse).

2. **Comprobar que se publicó en Release**  
   En el `Web.config` desplegado debe existir el bloque `<machineKey ...>` dentro de `<system.web>`.

3. **Generar tu propio machineKey (opcional)**  
   Si quisieras claves distintas, genera nuevas en:  
   https://www.developmentnow.com/asp-net-machinekey-generator/  
   - validationKey: 64 caracteres hex para HMACSHA256.  
   - decryptionKey: 64 caracteres hex para AES.  
   Sustituye en `Web.Release.config` y vuelve a publicar.

4. **Base de datos**  
   Identity valida la cookie contra la BD cada 24 h. Si la conexión a la BD falla o el usuario no existe, la sesión puede “caer”. Revisar cadena de conexión y que la BD sea accesible desde el host.

Con esto, el comportamiento de sesión (iniciar sesión → “Hola [usuario]” → cerrar sesión → “Registrarse” / “Iniciar sesión”) debería ser el mismo en local y en hosting.
