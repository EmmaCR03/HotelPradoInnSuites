# ✅ Solución Definitiva: Acceso desde Red Local

## 🔍 Problema Actual

Los bindings complejos no funcionan. Necesitamos una solución más simple y directa.

---

## ✅ Solución: Modificar .csproj (Más Simple)

En lugar de modificar `applicationhost.config` (que se regenera), modifiquemos el `.csproj` que es más estable.

---

## 🔧 Paso 1: Modificar HotelPrado.UI.csproj

1. **Cierra Visual Studio** (si está abierto)

2. **Abre el archivo:** `HotelPrado.UI/HotelPrado.UI.csproj`

3. **Busca la línea:**
   ```xml
   <IISUrl>https://localhost:44380/</IISUrl>
   ```

4. **Cámbiala por:**
   ```xml
   <IISUrl>https://192.168.0.12:44380/</IISUrl>
   ```

5. **Guarda el archivo**

---

## 🔧 Paso 2: Modificar applicationhost.config (Simple)

1. **Abre el archivo:** `.vs/HotelPrado.UI/config/applicationhost.config`

2. **Busca la sección de HotelPrado.UI** (línea ~164)

3. **Encuentra los bindings:**
   ```xml
   <bindings>
       <binding protocol="https" bindingInformation="*:44380:localhost" />
       <binding protocol="http" bindingInformation="*:56582:localhost" />
   </bindings>
   ```

4. **Cámbialos por:**
   ```xml
   <bindings>
       <binding protocol="https" bindingInformation="*:44380:192.168.0.12" />
       <binding protocol="http" bindingInformation="*:56582:192.168.0.12" />
   </bindings>
   ```

5. **Guarda el archivo**

---

## 🔧 Paso 3: Verificar Firewall

Asegúrate de que el firewall permita ambos puertos:

**PowerShell como Administrador:**
```powershell
New-NetFirewallRule -DisplayName "IIS Express HTTPS - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "IIS Express HTTP - Hotel Prado" -Direction Inbound -LocalPort 56582 -Protocol TCP -Action Allow
```

---

## ✅ Paso 4: Probar

1. **Abre Visual Studio**
2. **Ejecuta el proyecto (F5)**
3. **Debería abrirse con:** `https://192.168.0.12:44380`

4. **Desde iPhone:**
   - `http://192.168.0.12:56582` (más fácil)
   - O `https://192.168.0.12:44380` (puede pedir certificado)

---

## ⚠️ Nota Importante

**Con esta configuración:**
- ✅ Funciona con la IP (`192.168.0.12`)
- ❌ NO funciona con `localhost` (pero puedes cambiarlo cuando quieras)

**Si necesitas volver a localhost:**
- Cambia `applicationhost.config` de vuelta a `localhost`
- O cambia el `.csproj` de vuelta a `localhost`

---

## 🔄 Alternativa: Usar Solo HTTP (Más Simple)

Si HTTPS da problemas, puedes usar solo HTTP:

1. **En applicationhost.config**, deja solo:
   ```xml
   <bindings>
       <binding protocol="http" bindingInformation="*:56582:192.168.0.12" />
   </bindings>
   ```

2. **En .csproj**, cambia a:
   ```xml
   <IISUrl>http://192.168.0.12:56582/</IISUrl>
   ```

3. **Usa en iPhone:** `http://192.168.0.12:56582`

---

## ✅ Resumen

**Solución Simple:**
1. Modificar `.csproj` para usar IP
2. Modificar `applicationhost.config` para usar IP
3. Configurar firewall
4. Probar

**Ventaja:** Más simple, menos bindings, menos problemas.

**Desventaja:** No funciona con localhost (pero puedes cambiarlo fácilmente).

---

## 🎯 URLs Finales

**Desde PC:**
- `https://192.168.0.12:44380`
- `http://192.168.0.12:56582`

**Desde iPhone:**
- `http://192.168.0.12:56582` ← Recomendado

¡Prueba esta solución más simple!


