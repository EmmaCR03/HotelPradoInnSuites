# 📖 Guía: Cómo Leer Archivos DBF del Sistema Antiguo

## 🎯 Objetivo

Los archivos `.DBF` y `.FPT` que viste en las fotos son la base de datos del sistema antiguo (Visual FoxPro). Necesitamos leerlos para migrar los datos a SQL Server.

---

## 🔧 Opción 1: Usar el Lector DBF (Recomendado)

He creado un programa en C# que puede leer los archivos DBF directamente.

### Pasos:

1. **Compilar el programa**:
   ```bash
   cd LectorDBF
   dotnet build
   ```

2. **Ejecutar el programa**:
   ```bash
   dotnet run "C:\ruta\a\tu\archivo.DBF"
   ```

   O simplemente arrastra un archivo `.DBF` sobre el ejecutable.

3. **El programa mostrará**:
   - ✅ Estructura de la tabla (campos, tipos, tamaños)
   - ✅ Primeros 10 registros de datos
   - ✅ Sugerencia de estructura SQL equivalente

---

## 🔧 Opción 2: Usar Excel (Más fácil pero limitado)

1. Abre **Microsoft Excel**
2. Ve a **Datos** → **Obtener datos** → **Desde archivo** → **Desde base de datos**
3. Selecciona el archivo `.DBF`
4. Excel intentará abrirlo (puede que necesites el driver de Visual FoxPro)

**Limitación**: Solo funciona si tienes el driver de Visual FoxPro instalado.

---

## 🔧 Opción 3: Usar DBF Viewer (Herramienta externa)

1. Descarga **DBF Viewer Plus** (gratis): https://www.dbfview.com/
2. Abre el archivo `.DBF`
3. Puedes exportar a Excel o CSV

---

## 🔧 Opción 4: Usar Python (Si tienes Python instalado)

He creado un script Python alternativo:

```bash
python leer_dbf.py "ruta\archivo.DBF"
```

---

## 📋 ¿Qué archivos DBF necesitas?

Basándote en las tablas que tienes en SQL Server, necesitarías estos archivos DBF:

1. **CLIENTE.DBF** → Para migrar clientes
2. **RESERVAS.DBF** → Para migrar reservas
3. **HABITACIONES.DBF** → Para migrar habitaciones
4. **DEPARTAMENTO.DBF** → Para migrar departamentos
5. **COLABORADOR.DBF** → Para migrar colaboradores
6. **CITAS.DBF** → Para migrar citas

Y posiblemente:
- **TIPOHABITACION.DBF** (si existe)
- **TIPODEPARTAMENTO.DBF** (si existe)

---

## 🚨 Problemas Comunes

### Error: "No se puede leer el archivo"
- Verifica que el archivo no esté corrupto
- Asegúrate de que no esté siendo usado por otro programa
- Verifica los permisos del archivo

### Error: "Encoding incorrecto"
- Los archivos DBF antiguos pueden usar diferentes codificaciones
- El programa intenta usar Windows-1252 (español), pero puede necesitar ajustes

### Archivos .FPT (Memo)
- Los archivos `.FPT` contienen campos de tipo "Memo" (texto largo)
- Necesitan leerse junto con el `.DBF` correspondiente
- El lector actual no los lee automáticamente (se puede mejorar)

---

## 📝 Próximos Pasos

1. **Localiza los archivos DBF** del sistema antiguo
2. **Ejecuta el lector** en cada archivo importante
3. **Compara la estructura** con tus tablas SQL Server
4. **Identifica diferencias** y campos faltantes
5. **Crea el script de migración** una vez que tengas la estructura clara

---

## ❓ ¿Dónde están los archivos DBF?

Según las fotos que compartiste, parecen estar en una carpeta `exe` en tu escritorio. Busca archivos como:

- `CLIENTE.DBF`, `CLIENTES.DBF`
- `RESERVA.DBF`, `RESERVAS.DBF`
- `HABITACION.DBF`, `HABITACIONES.DBF`
- etc.

**Pregunta importante**: ¿Tienes acceso a esa carpeta con los archivos DBF? Si es así, podemos empezar a leerlos ahora mismo.






