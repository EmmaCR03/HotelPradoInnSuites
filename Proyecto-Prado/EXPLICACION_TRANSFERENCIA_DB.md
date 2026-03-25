# 📊 Explicación: Transferencia Mensual vs Base de Datos

## 🔍 Diferencia Importante

### Transferencia Mensual (100 GB) ≠ Espacio de Base de Datos

Son dos cosas **completamente diferentes**:

---

## 1️⃣ Transferencia Mensual (100 GB) - "Ancho de Banda"

### ¿Qué es?
La **transferencia mensual** (también llamada "bandwidth" o "ancho de banda") es la cantidad de **datos que se transfieren** entre tu servidor y los visitantes de tu sitio web.

### ¿Qué consume transferencia?

#### ✅ Datos que SÍ consumen transferencia:

1. **Páginas web visitadas**
   - Cada vez que alguien visita una página, se descargan:
     - HTML de la página
     - CSS (estilos)
     - JavaScript
     - Imágenes mostradas
   - **Ejemplo:** Una página con imágenes puede ser 2-5 MB por visita

2. **Consultas a la base de datos** (cuando se muestran resultados)
   - Cuando un usuario ve:
     - Lista de reservas → se transfieren los datos de reservas
     - Lista de clientes → se transfieren datos de clientes
     - Reportes → se transfieren datos del reporte
     - Facturas → se transfieren datos de facturas
   - **Ejemplo:** Una página con 50 reservas puede transferir ~100-500 KB de datos

3. **Descargas de archivos**
   - Si los usuarios descargan:
     - Facturas PDF
     - Reportes Excel
     - Imágenes
   - **Ejemplo:** Una factura PDF puede ser 200-500 KB

4. **Subida de archivos**
   - Cuando subes imágenes de habitaciones
   - Cuando subes documentos
   - **Ejemplo:** Una imagen de 2 MB subida = 2 MB de transferencia

5. **APIs y servicios**
   - Si tienes APIs que responden con datos JSON
   - **Ejemplo:** Una respuesta JSON puede ser 10-50 KB

---

## 2️⃣ Espacio de Base de Datos (Almacenamiento)

### ¿Qué es?
El **espacio de base de datos** es la cantidad de **datos almacenados** en tu base de datos SQL Server.

### ¿Qué ocupa espacio en la BD?

1. **Tablas con datos:**
   - Reservas (cada reserva ocupa espacio)
   - Clientes (cada cliente ocupa espacio)
   - Facturas (cada factura ocupa espacio)
   - Mantenimientos
   - Bitácora de eventos
   - etc.

2. **Índices** (para búsquedas rápidas)

3. **Logs de transacciones**

### Ejemplo de tamaño:
- **1 reserva:** ~1-2 KB
- **1 cliente:** ~500 bytes - 1 KB
- **1 factura:** ~2-5 KB
- **1 entrada de bitácora:** ~500 bytes

**Estimación para Hotel Prado:**
- 1,000 reservas = ~1-2 MB
- 500 clientes = ~500 KB - 1 MB
- 500 facturas = ~1-2.5 MB
- Bitácora (10,000 eventos) = ~5 MB
- **Total estimado:** 10-20 MB inicialmente, creciendo a 100-500 MB con el tiempo

---

## 📊 ¿Cuánto de los 100 GB es de la Base de Datos?

### Respuesta Corta:
**Muy poco directamente.** La mayoría de la transferencia es de imágenes, CSS, JavaScript, etc.

### Desglose Típico para Hotel Prado:

#### Escenario: 1,000 visitas al mes

| Tipo de Dato | Tamaño por Visita | Total/Mes | % del Total |
|--------------|-------------------|-----------|-------------|
| **Imágenes** | 2-5 MB | 2-5 GB | 2-5% |
| **CSS/JS** | 200-500 KB | 200-500 MB | 0.2-0.5% |
| **HTML** | 50-200 KB | 50-200 MB | 0.05-0.2% |
| **Datos de BD** | 10-100 KB | 10-100 MB | 0.01-0.1% |
| **PDFs/Reportes** | 200-500 KB | 200-500 MB | 0.2-0.5% |
| **Total** | ~3-6 MB | ~3-6 GB | **3-6% de 100 GB** |

### Conclusión:
- **Datos de BD:** ~10-100 MB/mes (0.01-0.1% de 100 GB)
- **Resto (imágenes, CSS, etc.):** ~2-5 GB/mes (2-5% de 100 GB)
- **Disponible:** ~95 GB sin usar

---

## 🎯 Ejemplos Prácticos

### Ejemplo 1: Usuario ve lista de reservas
1. **Consulta a BD:** Obtiene 50 reservas (~50 KB de datos)
2. **Se genera HTML:** Con los datos (~100 KB)
3. **Se descargan imágenes:** 5 imágenes de habitaciones (~2 MB)
4. **Se descargan CSS/JS:** (~300 KB)
5. **Total transferido:** ~2.45 MB
   - **De la BD:** 50 KB (2%)
   - **Resto:** 2.4 MB (98%)

### Ejemplo 2: Usuario descarga factura PDF
1. **Consulta a BD:** Obtiene datos de factura (~5 KB)
2. **Se genera PDF:** (~300 KB)
3. **Usuario descarga PDF:** 300 KB transferidos
4. **Total transferido:** ~305 KB
   - **De la BD:** 5 KB (1.6%)
   - **PDF generado:** 300 KB (98.4%)

### Ejemplo 3: Usuario sube imagen de habitación
1. **Usuario sube imagen:** 2 MB
2. **Se guarda en servidor:** (no cuenta como transferencia de salida)
3. **Se actualiza BD:** Registro de imagen (~1 KB)
4. **Total transferido:** 2 MB (solo subida)
   - **De la BD:** 1 KB (0.05%)
   - **Imagen:** 2 MB (99.95%)

---

## 📈 ¿Cuándo se consume más transferencia?

### Alto consumo de transferencia:
1. **Muchas visitas** con imágenes grandes
2. **Descargas frecuentes** de PDFs/reportes
3. **Videos** (si los tienes)
4. **APIs públicas** con mucho tráfico

### Bajo consumo de transferencia:
1. **Pocas visitas**
2. **Imágenes optimizadas** (comprimidas)
3. **Solo uso interno** (personal del hotel)
4. **Sin descargas masivas**

---

## 💡 Para Hotel Prado (Tráfico No Alto)

### Estimación Realista:

**Escenario Conservador:**
- **Visitas/mes:** 500-1,000
- **Transferencia estimada:** 5-15 GB/mes
- **Disponible:** 85-95 GB sin usar
- **Datos de BD transferidos:** ~50-200 MB/mes

**Conclusión:**
- ✅ **100 GB es MÁS que suficiente**
- ✅ Los datos de BD son una **fracción mínima** (0.1-0.2%)
- ✅ La mayoría de transferencia es de **imágenes y archivos estáticos**

---

## ⚠️ Importante: Espacio de BD vs Transferencia

### Espacio de Base de Datos:
- **Plan Avanzado:** Incluye 2 bases de datos SQL Server
- **Tamaño de cada BD:** Depende de tus datos
- **No hay límite específico** en el plan (pero está limitado por el espacio total de 10 GB)
- **Tu BD probablemente:** 100 MB - 2 GB (crece con el tiempo)

### Transferencia Mensual:
- **Plan Avanzado:** 100 GB/mes
- **Se resetea cada mes**
- **Si excedes:** Puede haber costo adicional o limitación

---

## 🎯 Resumen

### Pregunta: ¿Cuánto de los 100 GB es de la base de datos?

**Respuesta:**
- **Directamente de BD:** ~10-200 MB/mes (0.01-0.2%)
- **Indirectamente (páginas que usan BD):** ~1-5 GB/mes (1-5%)
- **Mayoría (imágenes, CSS, etc.):** ~2-10 GB/mes (2-10%)
- **Total usado:** ~3-15 GB/mes (3-15% de 100 GB)
- **Disponible:** ~85-97 GB sin usar

### Conclusión:
✅ **100 GB es más que suficiente** para tu caso
✅ Los datos de BD son una **fracción muy pequeña** de la transferencia
✅ La mayoría es de **imágenes y archivos estáticos**
✅ **No te preocupes** por exceder los 100 GB con tráfico bajo-medio

---

## 📊 Monitoreo

### Cómo ver cuánto usas:
1. **En Plesk:** Ve a "Statistics" o "Bandwidth"
2. **Verás:**
   - Transferencia total del mes
   - Desglose por tipo de archivo
   - Proyección de uso

### Alertas:
- Plesk puede enviarte alertas si te acercas al límite
- Puedes configurar alertas al 80% o 90% de uso

---

**En resumen:** Los 100 GB de transferencia son principalmente para imágenes, CSS, JavaScript y páginas web. Los datos de la base de datos son una fracción muy pequeña (menos del 1% típicamente).



