# 📋 Acceso a la Tabla Facturas de la Base de Datos Vieja

## 🎯 Objetivo

Este documento explica cómo acceder y trabajar con la tabla **Facturas** de la base de datos vieja (Visual FoxPro) del sistema antiguo del hotel.

---

## 📍 Ubicación de los Archivos DBF

La base de datos vieja se encuentra en la carpeta `dbViejaHotel/`. Hay múltiples subdirectorios que pueden contener archivos relacionados con facturas:

### Directorios Principales:
- `dbViejaHotel/sicre_d/` - Base de datos principal
- `dbViejaHotel/Emmanuel/` - Base de datos alternativa
- `dbViejaHotel/dbaipa/` - Base de datos adicional

### Archivos Relacionados con Facturas:
- `tmp_fact.DBF` - Archivo temporal de facturas (encontrado en la raíz)
- Posibles archivos: `FACTURA.DBF`, `FACTURAS.DBF`, `FACTURACION.DBF` en los subdirectorios

---

## 🔍 Cómo Buscar la Tabla Facturas

### Opción 1: Búsqueda Manual
1. Navega a la carpeta `dbViejaHotel/`
2. Busca archivos que contengan "fact" en el nombre:
   ```bash
   # En Windows PowerShell
   Get-ChildItem -Path "dbViejaHotel" -Recurse -Filter "*fact*.dbf"
   ```

### Opción 2: Usar el Script Python
He creado un script para escanear y encontrar archivos DBF relacionados con facturas:

```python
import os
from pathlib import Path

def buscar_facturas():
    base_path = Path('dbViejaHotel')
    archivos_facturas = []
    
    for dbf_file in base_path.rglob('*fact*.dbf'):
        archivos_facturas.append(dbf_file)
        print(f"Encontrado: {dbf_file}")
    
    return archivos_facturas

if __name__ == '__main__':
    buscar_facturas()
```

---

## 📊 Estructura Esperada de la Tabla Facturas

Basándote en las tablas relacionadas (CARGOS.dbf), la tabla Facturas probablemente contiene:

### Campos Probables:
- `N_FOLIO` (Numérico, 10) - Número de folio/factura
- `FECHA` (Fecha) - Fecha de la factura
- `CLIENTE` (Texto) - Nombre del cliente
- `CEDULA` (Texto) - Cédula del cliente
- `TELEFONO` (Numérico) - Teléfono
- `DIRECCION` (Texto) - Dirección
- `EMAIL` (Texto) - Email
- `COD_EMPR` (Numérico) - Código de empresa
- `MONTO_TOTAL` (Numérico, 14) - Monto total de la factura
- `ESTADO` (Texto) - Estado de la factura (Pagada, Pendiente, Cancelada)
- `FECHA_PAGO` (Fecha) - Fecha de pago
- `METODO_PAGO` (Texto) - Método de pago
- `NOTAS` (Texto) - Notas adicionales

---

## 🔧 Cómo Leer la Tabla Facturas

### Opción 1: Usar el Lector DBF (Recomendado)

1. **Compilar el programa**:
   ```bash
   cd LectorDBF
   dotnet build
   ```

2. **Ejecutar con el archivo de facturas**:
   ```bash
   dotnet run "dbViejaHotel/sicre_d/FACTURA.DBF"
   # O el archivo que encuentres
   ```

3. **El programa mostrará**:
   - ✅ Estructura completa de la tabla
   - ✅ Tipos de datos de cada campo
   - ✅ Primeros registros de ejemplo
   - ✅ Sugerencia de estructura SQL equivalente

### Opción 2: Usar Python

```python
from EscanearDBF_Importantes import leer_estructura_dbf

# Leer estructura de facturas
resultado = leer_estructura_dbf('dbViejaHotel/sicre_d/FACTURA.DBF')

if resultado['exito']:
    print(f"Registros: {resultado['num_records']}")
    print("\nCampos:")
    for campo in resultado['campos']:
        print(f"  - {campo['nombre']}: {campo['tipo']} ({campo['tamaño']})")
```

### Opción 3: Usar DBF Viewer Plus

1. Descarga DBF Viewer Plus: https://www.dbfview.com/
2. Abre el archivo `FACTURA.DBF` (o el que encuentres)
3. Explora la estructura y datos
4. Exporta a Excel o CSV si es necesario

---

## 💾 Migración a SQL Server

Una vez que identifiques la estructura de la tabla Facturas, puedes:

### 1. Crear la Tabla en SQL Server

```sql
CREATE TABLE [dbo].[Facturas] (
    [IdFactura] INT IDENTITY(1,1) NOT NULL,
    [NumeroFolio] INT NOT NULL,
    [FechaFactura] DATETIME NOT NULL,
    [IdCliente] INT NULL,
    [NombreCliente] NVARCHAR(100) NULL,
    [CedulaCliente] NVARCHAR(16) NULL,
    [TelefonoCliente] NVARCHAR(20) NULL,
    [EmailCliente] NVARCHAR(100) NULL,
    [DireccionCliente] NVARCHAR(200) NULL,
    [CodigoEmpresa] INT NULL,
    [MontoTotal] DECIMAL(18,2) NOT NULL,
    [Estado] NVARCHAR(50) NOT NULL,
    [FechaPago] DATETIME NULL,
    [MetodoPago] NVARCHAR(50) NULL,
    [Notas] NVARCHAR(500) NULL,
    [FechaCreacion] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([IdFactura] ASC),
    CONSTRAINT [FK_Facturas_Cliente] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente])
);
```

### 2. Crear Script de Migración

Similar a los scripts existentes (`MigrarCargosYTemporadas.py`), puedes crear:

```python
# MigrarFacturas.py
from EscanearDBF_Importantes import leer_estructura_dbf, leer_datos_dbf
import pyodbc

def migrar_facturas():
    # Leer datos del DBF
    datos = leer_datos_dbf('dbViejaHotel/sicre_d/FACTURA.DBF')
    
    # Conectar a SQL Server
    conn = pyodbc.connect('...')
    cursor = conn.cursor()
    
    # Migrar datos
    for registro in datos:
        cursor.execute("""
            INSERT INTO Facturas (NumeroFolio, FechaFactura, ...)
            VALUES (?, ?, ...)
        """, registro)
    
    conn.commit()
    conn.close()
```

---

## 🔗 Relación con Otras Tablas

La tabla Facturas probablemente se relaciona con:

1. **CARGOS.dbf** - Los cargos que componen la factura
2. **CLIENTE/EXTRAS_AUT.dbf** - Información del cliente
3. **RESERVAS.dbf** - Reserva asociada (a través de N_FOLIO)
4. **EMPRESAS.dbf** - Empresa asociada (a través de COD_EMPR)

---

## 📝 Próximos Pasos

1. ✅ **Buscar el archivo**: Encuentra el archivo `FACTURA.DBF` (o similar) en `dbViejaHotel/`
2. ✅ **Analizar estructura**: Usa el LectorDBF para ver la estructura completa
3. ✅ **Crear tabla SQL**: Crea la tabla en SQL Server basándote en la estructura encontrada
4. ✅ **Crear script de migración**: Desarrolla un script Python para migrar los datos
5. ✅ **Validar datos**: Verifica que los datos migrados sean correctos

---

## ⚠️ Notas Importantes

- La tabla Facturas es **crítica** para el sistema de facturación
- Asegúrate de hacer un **backup** antes de migrar
- Verifica la **integridad referencial** con otras tablas
- Considera migrar también los **detalles de factura** (si existe una tabla relacionada)

---

## 📞 Soporte

Si encuentras problemas al acceder a la tabla Facturas:
1. Verifica que el archivo existe en la ubicación esperada
2. Asegúrate de tener permisos de lectura
3. Revisa que el formato del archivo sea DBF válido
4. Consulta los logs del LectorDBF para más detalles
















