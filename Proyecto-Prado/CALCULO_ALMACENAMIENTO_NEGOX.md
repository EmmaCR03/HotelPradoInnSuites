# 💾 Cálculo de Almacenamiento - Plan Negox

## 📊 Espacio Disponible en Cada Plan

| Plan | Espacio en Disco | Precio/Año |
|------|------------------|------------|
| **Estándar** | 5 GB | $54 |
| **Avanzado** ⭐ | **10 GB** | $114 |
| **Premium** | 25 GB | $234 |

---

## 🔍 ¿Qué Ocupa Espacio en el Servidor?

### Componentes que Consumen Espacio:

1. **Aplicación ASP.NET** (~200-500 MB)
   - Archivos compilados (.dll)
   - Vistas (.cshtml)
   - Configuraciones
   - Scripts y CSS

2. **Base de Datos SQL Server** (Variable - crece con el tiempo)
   - Tablas con datos
   - Índices
   - Logs de transacciones

3. **Imágenes y Archivos Estáticos** (Variable - puede ser grande)
   - Imágenes de habitaciones
   - Imágenes de departamentos
   - Hero banners
   - Imágenes del sitio web

4. **Logs y Archivos Temporales** (~100-500 MB)
   - Logs de aplicación
   - Archivos temporales
   - Cache

---

## 📈 Cálculo Detallado para Hotel Prado

### 1. Aplicación ASP.NET

**Estimación:**
- Archivos compilados: ~50-100 MB
- Vistas y contenido: ~50-100 MB
- Scripts y CSS: ~20-50 MB
- Dependencias NuGet: ~100-200 MB
- **Total:** ~200-500 MB

### 2. Base de Datos SQL Server

**Datos que ya migraste:**
- **Clientes:** ~135,865 registros (según análisis)
- **Reservas:** ~22,555 registros
- **Facturas:** Variable
- **Otros datos:** Mantenimientos, bitácora, etc.

**Cálculo de tamaño:**

| Tipo de Dato | Registros | Tamaño por Registro | Total |
|--------------|-----------|---------------------|-------|
| Clientes | 135,865 | ~1 KB | ~136 MB |
| Reservas | 22,555 | ~2 KB | ~45 MB |
| Facturas | Variable | ~3 KB | ~50-100 MB |
| Mantenimientos | Variable | ~1 KB | ~10-20 MB |
| Bitácora | Variable | ~0.5 KB | ~10-50 MB |
| Otros | Variable | Variable | ~20-50 MB |
| **Índices** | - | - | ~50-100 MB |
| **Total Base de Datos** | - | - | **~300-500 MB inicial** |

**Crecimiento estimado:**
- **Por año:** ~50-100 MB (nuevas reservas, facturas, etc.)
- **En 5 años:** ~600 MB - 1 GB
- **En 10 años:** ~1-2 GB

### 3. Imágenes y Archivos Estáticos

**Tu proyecto tiene:**
- Imágenes de habitaciones
- Imágenes de departamentos
- Hero banners
- Imágenes del sitio web
- Archivos CR2 (RAW de cámara - grandes)

**Tamaño REAL medido:**
- ✅ **Imágenes actuales:** **457 MB** (479,621,120 bytes)
- **Archivos RAW (CR2):** Incluidos en el total (si los hay)
- **Total actual:** **~457 MB**

**Nota:** Este tamaño es razonable y está bien optimizado. Si optimizas más, podrías reducir a ~300-350 MB.

**⚠️ IMPORTANTE:** Si subes archivos RAW (CR2), pueden ser muy grandes. Recomendación: NO subir RAW, solo JPG optimizados.

### 4. Logs y Temporales

**Estimación:**
- Logs de aplicación: ~50-200 MB
- Archivos temporales: ~50-200 MB
- Cache: ~50-100 MB
- **Total:** ~150-500 MB

---

## 📊 Total Estimado de Espacio Necesario

### ✅ Escenario REAL (Con tus imágenes actuales):

| Componente | Tamaño | Notas |
|------------|--------|-------|
| Aplicación ASP.NET | 300 MB | Fijo |
| Base de Datos (inicial) | 400 MB | Crece con el tiempo |
| Base de Datos (5 años) | 800 MB | Proyección |
| **Imágenes (REAL)** | **457 MB** | ✅ Tamaño actual medido |
| Logs y temporales | 300 MB | Variable |
| **Total Inicial** | **~1.46 GB** | ✅ |
| **Total en 5 años** | **~1.86 GB** | ✅ |
| **Total en 10 años** | **~2.26 GB** | ✅ |

**¡Excelente!** Tus imágenes están bien optimizadas. 457 MB es un tamaño razonable.

---

## ✅ ¿Es Suficiente el Plan Avanzado (10 GB)?

### Comparación:

| Escenario | Espacio Necesario | Plan Avanzado (10 GB) | ¿Suficiente? |
|-----------|-------------------|----------------------|--------------|
| **REAL (con tus imágenes)** | **~1.5-2.3 GB** | 10 GB | ✅ **SOBRA MUCHO** (7.7-8.5 GB libres) |
| **Con más imágenes futuras** | ~3-4 GB | 10 GB | ✅ **Suficiente** (6-7 GB libres) |
| **Con muchas imágenes nuevas** | ~5-6 GB | 10 GB | ✅ **Suficiente** (4-5 GB libres) |

### Conclusión:

✅ **Plan Avanzado (10 GB) es SUFICIENTE** para tu proyecto, especialmente si:
- Optimizas las imágenes (comprimir, usar JPG en lugar de RAW)
- Limpias logs periódicamente
- No subes archivos RAW (CR2)

⚠️ **Puede ser ajustado** si:
- Subes muchas imágenes sin optimizar
- Subes archivos RAW
- No limpias logs

---

## 🎯 Recomendaciones para Optimizar Espacio

### 1. Optimizar Imágenes (MUY IMPORTANTE)

**Antes de subir:**
- ✅ Comprimir imágenes JPG (calidad 80-85%)
- ✅ Redimensionar imágenes grandes (máx. 1920x1080 para web)
- ✅ Convertir PNG a JPG cuando sea posible
- ✅ **NO subir archivos RAW (CR2)** - solo JPG procesados

**Herramientas:**
- **TinyPNG/TinyJPG:** Comprime sin perder mucha calidad
- **ImageOptim:** Para Mac
- **GIMP/Photoshop:** Para redimensionar y optimizar

**Ahorro estimado:** 50-70% de espacio en imágenes

### 2. Limpiar Logs Periódicamente

**Configurar:**
- Rotación automática de logs
- Eliminar logs antiguos (>30 días)
- Limitar tamaño de logs

**Ahorro estimado:** 100-300 MB

### 3. Optimizar Base de Datos

**Acciones:**
- Comprimir índices periódicamente
- Eliminar datos históricos muy antiguos (si no se necesitan)
- Limpiar tablas temporales

**Ahorro estimado:** 50-100 MB

### 4. No Subir Archivos Innecesarios

**Evitar:**
- Archivos RAW (CR2)
- Archivos de desarrollo (.cs, .csproj)
- Backups en el servidor
- Archivos temporales grandes

---

## 📊 Cómo Verificar el Uso de Espacio

### En Plesk (Panel de Negox):

1. **Ver uso total:**
   - Inicia sesión en Plesk
   - Ve a "Statistics" o "Resource Usage"
   - Verás el espacio usado vs disponible

2. **Ver por componente:**
   - **Aplicación:** Ve a "Files" → Tamaño de carpetas
   - **Base de datos:** Ve a "Databases" → Tamaño de cada BD
   - **Logs:** Ve a "Logs" → Tamaño de archivos de log

3. **Configurar alertas:**
   - Configura alerta al 80% de uso
   - Recibirás email cuando te acerques al límite

### Comandos Útiles (si tienes acceso SSH):

```bash
# Ver tamaño total
du -sh /ruta/a/tu/sitio

# Ver tamaño por carpeta
du -sh /ruta/a/tu/sitio/*

# Ver tamaño de base de datos
# (Desde SQL Server Management Studio o Plesk)
```

---

## 🚨 ¿Qué Pasa si Te Quedas Sin Espacio?

### Opciones:

1. **Optimizar** (Gratis)
   - Comprimir imágenes
   - Limpiar logs
   - Eliminar archivos innecesarios

2. **Agregar Espacio** (Negox)
   - **+10 GB:** $5/mes adicionales
   - Puedes agregar según necesites

3. **Cambiar a Plan Premium** (Si necesitas mucho más)
   - **25 GB:** $234/año
   - Solo si realmente necesitas mucho espacio

---

## 📈 Proyección de Crecimiento

### ✅ Escenario REAL (Con tus imágenes actuales - 457 MB):

| Año | Aplicación | Base de Datos | Imágenes | Logs | **Total** | **Disponible (10 GB)** |
|-----|------------|---------------|----------|------|-----------|------------------------|
| **Inicial** | 300 MB | 400 MB | **457 MB** | 300 MB | **~1.46 GB** | **~8.54 GB** ✅ |
| **1 año** | 300 MB | 450 MB | 500 MB | 300 MB | **~1.55 GB** | **~8.45 GB** ✅ |
| **3 años** | 300 MB | 550 MB | 600 MB | 300 MB | **~1.75 GB** | **~8.25 GB** ✅ |
| **5 años** | 300 MB | 800 MB | 800 MB | 300 MB | **~2.2 GB** | **~7.8 GB** ✅ |
| **10 años** | 300 MB | 1.2 GB | 1.2 GB | 300 MB | **~3 GB** | **~7 GB** ✅ |

**¡Excelente!** Con 457 MB de imágenes, tienes **más que suficiente espacio** en Plan Avanzado (10 GB).

**Conclusión:** ✅ Con optimización, **10 GB es más que suficiente** para 10+ años

---

## 💡 Plan de Acción Recomendado

### Paso 1: Antes de Subir
1. ✅ **Optimizar todas las imágenes** (comprimir, redimensionar)
2. ✅ **NO subir archivos RAW** (solo JPG procesados)
3. ✅ **Limpiar archivos innecesarios** del proyecto

### Paso 2: Después de Subir
1. ✅ **Monitorear uso** en Plesk (primer mes)
2. ✅ **Configurar alertas** al 80% de uso
3. ✅ **Limpiar logs** periódicamente

### Paso 3: Si Necesitas Más Espacio
1. ✅ **Primero optimizar** (comprimir imágenes, limpiar)
2. ✅ **Si aún necesitas:** Agregar +10 GB por $5/mes
3. ✅ **Solo si es mucho:** Considerar Plan Premium

---

## ✅ Resumen Final

### Pregunta: ¿Cuánto almacenamiento tendré y es suficiente?

**Respuesta:**

**Plan Avanzado (Recomendado):**
- ✅ **10 GB de espacio** disponible
- ✅ **Suficiente para tu proyecto** con optimización
- ✅ **Sobra espacio** para crecer (5-7 GB libres inicialmente)
- ✅ **Puedes agregar más** si necesitas (+10 GB por $5/mes)

**Estimación de uso (REAL con tus imágenes):**
- **Inicial:** **~1.5 GB** (con tus 457 MB de imágenes)
- **En 5 años:** ~2.2 GB
- **En 10 años:** ~3 GB

**Recomendación:**
1. ✅ **Tus imágenes ya están bien** (457 MB es razonable)
2. ✅ **NO subas archivos RAW** (CR2) - solo JPG procesados
3. ✅ **Monitorea el uso** en Plesk (aunque no será necesario por mucho tiempo)
4. ✅ **Plan Avanzado (10 GB) es MÁS que suficiente** - te sobra ~8.5 GB inicialmente

---

## 🔗 Próximos Pasos

1. **Calcular tamaño real de imágenes:**
   - Revisa la carpeta `Img/` de tu proyecto
   - Calcula el tamaño total
   - Optimiza antes de subir

2. **Verificar tamaño de base de datos:**
   - Después de migrar datos, verifica el tamaño en SQL Server
   - Proyecta crecimiento futuro

3. **Configurar monitoreo:**
   - En Plesk, configura alertas de espacio
   - Revisa periódicamente el uso

**En resumen:** Plan Avanzado (10 GB) es suficiente para tu proyecto, especialmente si optimizas las imágenes. Tienes margen para crecer y puedes agregar más espacio si lo necesitas.

