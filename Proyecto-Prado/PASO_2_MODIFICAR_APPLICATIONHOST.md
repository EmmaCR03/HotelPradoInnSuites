# ✅ Paso 2: Modificar applicationhost.config

## 🎯 Objetivo
Permitir que IIS Express acepte conexiones desde tu red local (no solo localhost).

---

## 📝 Pasos

### 1. Cerrar Visual Studio
**IMPORTANTE:** Cierra Visual Studio completamente antes de modificar el archivo.

### 2. Buscar el archivo

El archivo está en:
```
.vs\config\applicationhost.config
```

**Dentro de tu proyecto** (carpeta `.vs` que está oculta normalmente).

### 3. Abrir el archivo

- **Clic derecho** en `applicationhost.config`
- **Abrir con** → Notepad (o cualquier editor de texto)

### 4. Buscar la línea

Presiona `Ctrl + F` y busca:
```
44380
```

O busca:
```
bindingInformation="*:44380:localhost"
```

### 5. Modificar

**Encuentra esta línea:**
```xml
<binding protocol="https" bindingInformation="*:44380:localhost" />
```

**Cámbiala por:**
```xml
<binding protocol="https" bindingInformation="*:44380:*" />
```

**También busca si hay una línea para HTTP:**
```xml
<binding protocol="http" bindingInformation="*:44380:localhost" />
```

**Cámbiala por:**
```xml
<binding protocol="http" bindingInformation="*:44380:*" />
```

### 6. Guardar

- **Ctrl + S** para guardar
- Cierra el archivo

---

## ✅ Verificar

Después de modificar:

1. **Abre Visual Studio**
2. **Ejecuta el proyecto** (F5)
3. **En tu PC**, prueba: `https://192.168.0.12:44380`
4. **En tu teléfono** (misma red WiFi): `https://192.168.0.12:44380`

---

## ⚠️ Si no encuentras el archivo

Si no ves la carpeta `.vs`:

1. En el Explorador de Windows, ve a **Vista** → **Mostrar elementos ocultos**
2. O escribe en la barra de direcciones: `.vs\config\applicationhost.config`

---

## 🔄 Si algo sale mal

Si modificaste algo incorrectamente:

1. **Cierra Visual Studio**
2. **Elimina la carpeta `.vs`** dentro de tu proyecto
3. **Abre Visual Studio de nuevo**
4. **Ejecuta el proyecto** (F5)
5. Visual Studio regenerará el archivo automáticamente

---

## ✅ Resumen

1. ✅ Cerrar Visual Studio
2. ✅ Abrir `.vs\config\applicationhost.config`
3. ✅ Cambiar `localhost` por `*` en las líneas de binding
4. ✅ Guardar
5. ✅ Ejecutar proyecto y probar desde teléfono

