# 🔧 Solución: Error "Invalid Hostname" HTTP 400

## ❌ Problema

El error **"Bad Request - Invalid Hostname"** significa que IIS Express está rechazando la conexión porque el hostname (IP) no está en la lista de hosts permitidos.

Aunque cambiaste el binding a `*:44380:*`, IIS Express necesita que uses la **IP específica** en lugar de `*`.

---

## ✅ Solución: Usar IP Específica

### Ya lo corregí automáticamente

He modificado el archivo `applicationhost.config` para usar tu IP específica (`192.168.0.12`) en lugar de `*`.

**Cambios realizados:**
- ✅ `*:44380:*` → `*:44380:192.168.0.12` (HTTPS)
- ✅ `*:56582:*` → `*:56582:192.168.0.12` (HTTP)

---

## 🔄 Pasos para Aplicar

### 1. Cerrar Visual Studio
**IMPORTANTE:** Cierra Visual Studio completamente.

### 2. El archivo ya está modificado
Ya corregí el archivo `applicationhost.config` con tu IP.

### 3. Abrir Visual Studio y Ejecutar
1. Abre Visual Studio
2. Ejecuta el proyecto (F5)
3. Espera a que inicie completamente

### 4. Probar desde PC
En tu navegador de PC, prueba:
- `https://192.168.0.12:44380`
- `http://192.168.0.12:56582`

**Deberían funcionar ahora.**

### 5. Probar desde iPhone
En Safari del iPhone:
- `https://192.168.0.12:44380`
- O `http://192.168.0.12:56582` (más fácil, sin certificado)

---

## ⚠️ Si tu IP Cambia

Si tu IP cambia (por DHCP), necesitarás actualizar el archivo:

1. **Obtén tu nueva IP:** `ipconfig` en CMD
2. **Modifica applicationhost.config:**
   - Busca `192.168.0.12`
   - Reemplázala con tu nueva IP
3. **Guarda y reinicia Visual Studio**

---

## 🔍 Alternativa: Usar localhost + hosts file

Si prefieres no depender de la IP, puedes:

1. **Editar el archivo hosts:**
   - Ruta: `C:\Windows\System32\drivers\etc\hosts`
   - Agrega: `192.168.0.12 hotelprado.local`
   - Guarda (como Administrador)

2. **Usar en navegador:** `http://hotelprado.local:56582`

Pero la solución con IP directa es más simple.

---

## ✅ Resumen

**Problema:** IIS Express rechaza `*` como hostname

**Solución:** Usar IP específica (`192.168.0.12`) en lugar de `*`

**Ya corregido:** El archivo `applicationhost.config` ya tiene tu IP

**Próximo paso:** Cerrar Visual Studio, abrir de nuevo, ejecutar (F5) y probar

---

## 🎯 URLs para Probar

**Desde PC:**
- `https://192.168.0.12:44380`
- `http://192.168.0.12:56582`

**Desde iPhone (Safari):**
- `https://192.168.0.12:44380` (puede pedir aceptar certificado)
- `http://192.168.0.12:56582` (más fácil, sin certificado)

**¡Debería funcionar ahora!** 🎉

