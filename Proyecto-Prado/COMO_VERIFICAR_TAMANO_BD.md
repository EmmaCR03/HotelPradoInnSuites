# 📊 Cómo Verificar el Tamaño de tu Base de Datos

## 🎯 Métodos para Ver el Tamaño

Tienes varias formas de verificar cuánto espacio está ocupando tu base de datos:

---

## ✅ Método 1: SQL Server Management Studio (SSMS) - MÁS FÁCIL

### Paso 1: Conectar a la Base de Datos
1. Abre **SQL Server Management Studio**
2. Conecta a tu servidor SQL Server
3. Expande **Databases** en el Explorador de Objetos
4. Haz clic derecho en **HotelPrado**
5. Selecciona **Reports** → **Standard Reports** → **Disk Usage**

### Paso 2: Ver el Reporte
- Verás un gráfico con:
  - **Tamaño total** de la base de datos
  - **Espacio usado** vs **Espacio reservado**
  - **Tamaño por tabla**

---

## ✅ Método 2: Consulta SQL Simple (RÁPIDO)

### Consulta Básica - Tamaño Total:

```sql
-- Ver tamaño total de la base de datos
EXEC sp_spaceused;
```

**Resultado:**
- `database_size`: Tamaño total reservado
- `unallocated space`: Espacio no asignado
- `reserved`: Espacio reservado
- `data`: Espacio usado por datos
- `index_size`: Espacio usado por índices
- `unused`: Espacio reservado pero no usado

---

## ✅ Método 3: Consulta Detallada por Tabla

### Ver Tamaño de Cada Tabla:

```sql
-- Ver tamaño de cada tabla (ordenado por tamaño)
SELECT 
    t.NAME AS NombreTabla,
    s.Name AS NombreEsquema,
    p.rows AS NumeroFilas,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS Tamaño_MB
FROM 
    sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE 
    t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.OBJECT_ID > 255 
GROUP BY 
    t.Name, s.Name, p.Rows
ORDER BY 
    Tamaño_MB DESC;
```

**Esto te muestra:**
- Nombre de cada tabla
- Número de filas
- Tamaño en MB
- Ordenado de mayor a menor

---

## ✅ Método 4: Script Completo (RECOMENDADO)

He creado un script completo en: `Scripts/Verificar_Tamano_BaseDatos.sql`

**Este script incluye:**
1. ✅ Tamaño total de la base de datos
2. ✅ Tamaño por archivo
3. ✅ Tamaño por tabla (top 20 más grandes)
4. ✅ Resumen por esquema
5. ✅ Tamaño de índices
6. ✅ Información detallada
7. ✅ Tamaño de logs
8. ✅ Tablas con más filas

**Cómo usarlo:**
1. Abre **SQL Server Management Studio**
2. Conecta a tu servidor
3. Abre el archivo `Scripts/Verificar_Tamano_BaseDatos.sql`
4. Ejecuta las consultas que necesites (F5)

---

## 📊 Interpretación de Resultados

### Tamaño Total:
- **< 100 MB:** Muy pequeño, no te preocupes
- **100 MB - 500 MB:** Pequeño, normal para empezar
- **500 MB - 1 GB:** Mediano, sigue siendo manejable
- **1 GB - 3 GB:** Grande, pero aún manejable
- **> 3 GB:** Muy grande, considera optimización

### Para Hotel Prado:
- **Esperado inicialmente:** 100 MB - 500 MB
- **Después de 1 año:** 500 MB - 1 GB
- **Después de varios años:** 1 GB - 3 GB

---

## 🔍 Qué Buscar

### Tablas que Pueden Ocupar Mucho Espacio:

1. **BitacoraEventos** ⚠️
   - Puede crecer mucho si registras muchos eventos
   - Considera limpiar eventos antiguos periódicamente

2. **Facturas**
   - Crece con cada reserva
   - Normal que crezca con el tiempo

3. **Reservas**
   - Crece con cada reserva
   - Normal que crezca

4. **Imágenes** (si están en BD)
   - Si guardas imágenes en la base de datos, pueden ocupar mucho
   - Mejor guardarlas en archivos

---

## 💡 Consejos para Reducir Tamaño

### 1. Limpiar Bitácora Antigua
```sql
-- Eliminar eventos de bitácora con más de 6 meses
DELETE FROM BitacoraEventos 
WHERE FechaDeEvento < DATEADD(MONTH, -6, GETDATE());
```

### 2. Comprimir Base de Datos
```sql
-- Comprimir la base de datos (libera espacio no usado)
DBCC SHRINKDATABASE('HotelPrado');
```

### 3. Reorganizar Índices
```sql
-- Reorganizar índices fragmentados
ALTER INDEX ALL ON [NombreTabla] REORGANIZE;
```

### 4. Eliminar Datos Antiguos
- Considera archivar reservas antiguas
- Eliminar solicitudes de limpieza completadas antiguas

---

## 📋 Consulta Rápida para Ver Todo

```sql
-- Consulta rápida que muestra todo lo importante
SELECT 
    'Tamaño Total BD' AS Concepto,
    CAST(SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS bigint) * 8192.) / 1024 / 1024 AS DECIMAL(10, 2)) AS Tamaño_MB,
    CAST(SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS bigint) * 8192.) / 1024 / 1024 / 1024 AS DECIMAL(10, 2)) AS Tamaño_GB
FROM sys.database_files
WHERE type_desc = 'ROWS'

UNION ALL

SELECT 
    'Tabla: ' + t.NAME AS Concepto,
    CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS DECIMAL(10, 2)) AS Tamaño_MB,
    NULL AS Tamaño_GB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.NAME NOT LIKE 'dt%' AND t.is_ms_shipped = 0 AND i.OBJECT_ID > 255
GROUP BY t.Name
ORDER BY Tamaño_MB DESC;
```

---

## 🎯 Resumen

### Para Ver Tamaño Rápido:
```sql
EXEC sp_spaceused;
```

### Para Ver Tamaño por Tabla:
Usa el script `Scripts/Verificar_Tamano_BaseDatos.sql` - Consulta #3

### Para Ver Todo Detallado:
Ejecuta todo el script `Scripts/Verificar_Tamano_BaseDatos.sql`

---

## ⚠️ Notas Importantes

1. **Espacio Reservado vs Usado:**
   - SQL Server reserva espacio aunque no lo use todo
   - Puedes liberar espacio no usado con `DBCC SHRINKDATABASE`

2. **Índices Ocupan Espacio:**
   - Los índices pueden ocupar tanto o más que los datos
   - Son necesarios para rendimiento

3. **Logs de Transacciones:**
   - El archivo de log puede crecer mucho
   - Se limpia automáticamente con backups

4. **Tamaño en Hosting:**
   - Verifica el límite de tamaño de base de datos en tu plan
   - Plan Estándar Negox: 1 BD (verifica límite de tamaño)
   - Plan Avanzado Negox: 2 BD (verifica límite de tamaño)

---

## 📞 Si Necesitas Ayuda

Si encuentras que tu base de datos está ocupando mucho espacio:
1. Identifica qué tablas ocupan más (usa el script)
2. Considera limpiar datos antiguos
3. Considera archivar datos históricos
4. Si es necesario, escala a un plan con más espacio
