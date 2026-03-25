# 📱 Acceso desde Teléfono - Guía Simple (Sin Desastre)

## 🎯 Solución Rápida (3 Pasos)

### ✅ Paso 1: Obtener tu IP Local

1. Abre **CMD** (Símbolo del sistema)
2. Escribe: `ipconfig`
3. Busca **"Dirección IPv4"** (probablemente algo como `192.168.0.2` o `192.168.1.XXX`)
4. **Anota esta IP** (la necesitarás)

---

### ✅ Paso 2: Configurar IIS Express (Solo una vez)

**Opción A: Modificar applicationhost.config (RECOMENDADO - No rompe nada)**

1. **Cierra Visual Studio** (si está abierto)

2. **Busca el archivo** `applicationhost.config`:
   - Está en: `.vs\config\applicationhost.config` (dentro de tu proyecto)
   - O busca con: `dir /s applicationhost.config` en CMD

3. **Abre el archivo** con Notepad o cualquier editor

4. **Busca** la línea que dice:
   ```xml
   <binding protocol="https" bindingInformation="*:44380:localhost" />
   ```

5. **Cámbiala por:**
   ```xml
   <binding protocol="https" bindingInformation="*:44380:*" />
   ```

6. **Guarda el archivo**

**✅ Listo!** Esto permite que cualquier dispositivo en tu red pueda conectarse.

---

### ✅ Paso 3: Configurar Firewall (Solo una vez)

**Método Rápido - PowerShell (Como Administrador):**

1. Abre **PowerShell como Administrador**:
   - Clic derecho en PowerShell → "Ejecutar como administrador"

2. Ejecuta este comando:
   ```powershell
   New-NetFirewallRule -DisplayName "IIS Express - Hotel Prado" -Direction Inbound -LocalPort 44380 -Protocol TCP -Action Allow
   ```

3. Presiona Enter

**✅ Listo!** El firewall ahora permite conexiones al puerto 44380.

---

## 📱 Paso 4: Acceder desde tu Teléfono

1. **Asegúrate de que:**
   - ✅ Tu teléfono está en la **misma red WiFi** que tu computadora
   - ✅ El proyecto está **ejecutándose** en Visual Studio (F5)

2. **En tu teléfono:**
   - Abre el navegador (Chrome, Safari, etc.)
   - Escribe: `https://TU_IP:44380`
   - Ejemplo: `https://192.168.0.2:44380`

3. **Si aparece advertencia de certificado:**
   - Es normal (certificado de desarrollo)
   - Haz clic en **"Avanzado"** o **"Advanced"**
   - Luego **"Continuar al sitio"** o **"Proceed"**

4. **¡Listo!** Deberías ver tu aplicación en el teléfono.

---

## 🔍 ¿Cuál es mi IP?

Si no sabes tu IP, ejecuta en CMD:
```
ipconfig
```

Busca la línea **"Dirección IPv4"** en la sección de tu adaptador WiFi/Ethernet.

**Ejemplos comunes:**
- `192.168.0.2`
- `192.168.1.100`
- `192.168.0.10`

---

## ⚠️ Si No Funciona

### Verificar:

1. **¿Está ejecutándose el proyecto?**
   - Debe estar corriendo en Visual Studio (F5)

2. **¿Están en la misma red WiFi?**
   - Tu PC y tu teléfono deben estar en la misma red

3. **¿Configuraste el firewall?**
   - Verifica que ejecutaste el comando de PowerShell como administrador

4. **¿La IP es correcta?**
   - Verifica con `ipconfig` que la IP no cambió

5. **¿Modificaste applicationhost.config?**
   - Debe decir `*:44380:*` (no `*:44380:localhost`)

---

## 🔄 Si la IP Cambia

Si tu IP cambia (por DHCP), simplemente:
1. Ejecuta `ipconfig` de nuevo
2. Usa la nueva IP en tu teléfono
3. No necesitas cambiar nada más

---

## 💡 Tips

- ✅ **Guarda la IP en favoritos** del navegador del teléfono
- ✅ **Usa HTTP si HTTPS da problemas**: `http://TU_IP:44380` (pero necesitas configurar también el puerto HTTP en el firewall)
- ✅ **Prueba primero desde tu PC**: Abre `https://TU_IP:44380` en tu navegador de la PC para verificar que funciona

---

## 🚀 Alternativa: Usar HTTP (Más Simple)

Si HTTPS da problemas, puedes usar HTTP:

1. **En applicationhost.config**, también cambia:
   ```xml
   <binding protocol="http" bindingInformation="*:44380:localhost" />
   ```
   Por:
   ```xml
   <binding protocol="http" bindingInformation="*:44380:*" />
   ```

2. **En el firewall**, también permite HTTP (puerto 44380)

3. **En tu teléfono**, usa: `http://TU_IP:44380` (sin la 's' de https)

---

## ✅ Resumen Rápido

1. **Obtén tu IP:** `ipconfig` en CMD
2. **Modifica applicationhost.config:** Cambia `localhost` por `*` en el binding
3. **Configura firewall:** Ejecuta el comando de PowerShell
4. **Accede desde teléfono:** `https://TU_IP:44380`

**¡Eso es todo!** Sin tocar el .csproj ni romper nada. 🎉


