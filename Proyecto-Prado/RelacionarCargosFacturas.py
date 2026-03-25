#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para Relacionar Cargos con Facturas
Relaciona cargos existentes con facturas existentes o crea nuevas facturas
"""
import os
import sys
import pyodbc
import uuid
from datetime import datetime

# Configurar codificación para Windows
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

# Configuración de conexión a SQL Server
CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

def relacionar_cargos_con_facturas_existentes(conexion, crear_facturas=False):
    """
    Relaciona cargos con facturas existentes por QuienPaga
    Si crear_facturas=True, crea nuevas facturas a partir de cargos agrupados
    """
    cursor = conexion.cursor()
    
    print("=" * 80)
    print("RELACIONANDO CARGOS CON FACTURAS")
    print("=" * 80)
    print()
    
    # Paso 1: Contar cargos sin relación
    cursor.execute("""
        SELECT COUNT(*) 
        FROM Cargos 
        WHERE IdFactura IS NULL
    """)
    cargos_sin_relacion = cursor.fetchone()[0]
    
    print(f"Cargos sin relación con facturas: {cargos_sin_relacion:,}")
    print()
    
    # Paso 2: Relacionar cargos con facturas existentes por QuienPaga
    print("Paso 1: Relacionando cargos con facturas existentes por QuienPaga...")
    
    cursor.execute("""
        UPDATE c
        SET c.IdFactura = f.IdFactura
        FROM Cargos c
        INNER JOIN (
            SELECT 
                f.IdFactura,
                f.QuienPaga,
                ROW_NUMBER() OVER (PARTITION BY f.QuienPaga ORDER BY f.FechaHoraFactura DESC) AS rn
            FROM Facturas f
            WHERE f.QuienPaga IS NOT NULL
        ) f ON c.QuienPaga = f.QuienPaga AND f.rn = 1
        WHERE c.IdFactura IS NULL
          AND c.QuienPaga IS NOT NULL
    """)
    
    cargos_relacionados_quienpaga = cursor.rowcount
    conexion.commit()
    
    print(f"  [OK] Cargos relacionados por QuienPaga: {cargos_relacionados_quienpaga:,}")
    print()
    
    # Paso 3: Relacionar por IdCheckIn
    print("Paso 2: Relacionando cargos con facturas por IdCheckIn...")
    
    cursor.execute("""
        UPDATE c
        SET c.IdFactura = f.IdFactura
        FROM Cargos c
        INNER JOIN Facturas f ON c.IdCheckIn = f.IdCheckIn
        WHERE c.IdFactura IS NULL
          AND c.IdCheckIn IS NOT NULL
          AND f.IdCheckIn IS NOT NULL
    """)
    
    cargos_relacionados_checkin = cursor.rowcount
    conexion.commit()
    
    print(f"  [OK] Cargos relacionados por IdCheckIn: {cargos_relacionados_checkin:,}")
    print()
    
    # Paso 4: Crear facturas a partir de cargos agrupados (opcional)
    if crear_facturas:
        print("Paso 3: Creando facturas a partir de cargos agrupados...")
        
        # Obtener o crear IDs válidos para EmisorId y ReceptorId
        # Primero intentar obtener un emisor existente
        cursor.execute("SELECT TOP 1 Id FROM Emisores")
        emisor_row = cursor.fetchone()
        if not emisor_row:
            # Intentar crear un emisor dummy
            try:
                cursor.execute("INSERT INTO Emisores (Id) OUTPUT INSERTED.Id VALUES (NEWID())")
                emisor_row = cursor.fetchone()
                conexion.commit()
                print("  Registro dummy creado en Emisores")
            except Exception as e:
                print(f"  ERROR: No se pudo crear emisor. Necesitas crear al menos un registro en Emisores.")
                print(f"  Error: {e}")
                return  # No podemos continuar sin emisor
        
        emisor_id = uuid.UUID(str(emisor_row[0]))
        
        # Ahora para ReceptorId
        cursor.execute("SELECT TOP 1 Id FROM Receptores")
        receptor_row = cursor.fetchone()
        if not receptor_row:
            try:
                cursor.execute("INSERT INTO Receptores (Id) OUTPUT INSERTED.Id VALUES (NEWID())")
                receptor_row = cursor.fetchone()
                conexion.commit()
                print("  Registro dummy creado en Receptores")
            except Exception as e:
                print(f"  ERROR: No se pudo crear receptor. Necesitas crear al menos un registro en Receptores.")
                print(f"  Error: {e}")
                return  # No podemos continuar sin receptor
        
        receptor_id = uuid.UUID(str(receptor_row[0]))
        
        # Verificar si existe la columna NumeroFactura
        cursor.execute("""
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'Facturas' AND COLUMN_NAME = 'NumeroFactura'
        """)
        tiene_numero_factura = cursor.fetchone()[0] > 0
        
        # Obtener el siguiente número de factura (si existe la columna)
        if tiene_numero_factura:
            cursor.execute("SELECT ISNULL(MAX(NumeroFactura), 0) FROM Facturas")
            siguiente_numero = cursor.fetchone()[0] + 1
        else:
            # Si no existe NumeroFactura, usar IdFactura como número
            cursor.execute("SELECT ISNULL(MAX(IdFactura), 0) FROM Facturas")
            siguiente_numero = cursor.fetchone()[0] + 1
        
        # Agrupar cargos por QuienPaga que no tienen factura
        cursor.execute("""
            SELECT 
                c.QuienPaga,
                MAX(CASE WHEN c.NumeroEmpleado IS NOT NULL AND c.NumeroEmpleado <= 2147483647 
                    THEN CAST(c.NumeroEmpleado AS INT) ELSE NULL END) AS IdEmpleado,
                MAX(c.FechaHora) AS FechaHoraFactura,
                SUM(ISNULL(c.MontoCargo, 0)) AS TotalConsumos,
                SUM(ISNULL(c.ImpuestoHotel, 0)) AS ImpuestoICT,
                SUM(ISNULL(c.ImpuestoServicio, 0)) AS ImpuestoServicio,
                SUM(ISNULL(c.ImpuestoVenta, 0)) AS ImpuestoVentas,
                SUM(ISNULL(c.MontoTotal, 0)) AS TotalGeneral
            FROM Cargos c
            WHERE c.IdFactura IS NULL
              AND c.QuienPaga IS NOT NULL
              AND NOT EXISTS (
                  SELECT 1 FROM Facturas f 
                  WHERE f.QuienPaga = c.QuienPaga
              )
            GROUP BY c.QuienPaga
        """)
        
        grupos = cursor.fetchall()
        facturas_creadas = 0
        
        for grupo in grupos:
            quien_paga, id_empleado, fecha_hora, total_consumos, impuesto_ict, impuesto_serv, impuesto_vent, total_general = grupo
            
            # Generar valores para columnas requeridas de facturación electrónica
            clave_factura = f"FACT-{siguiente_numero:06d}" if tiene_numero_factura else f"FACT-{quien_paga}-{datetime.now().strftime('%Y%m%d')}"
            numero_consecutivo = f"CONS-{siguiente_numero:06d}" if tiene_numero_factura else f"CONS-{quien_paga}"
            fecha_emision = fecha_hora if fecha_hora else datetime.now()
            # emisor_id y receptor_id ya están definidos arriba
            total_venta = total_general if total_general else 0
            total_descuento = 0
            total_impuesto = (impuesto_ict or 0) + (impuesto_serv or 0) + (impuesto_vent or 0)
            total_comprobante = total_venta + total_impuesto
            estado = 'Pendiente'
            
            # Insertar factura y obtener IdFactura
            # Incluir TODAS las columnas requeridas de facturación electrónica
            if tiene_numero_factura:
                cursor.execute("""
                    INSERT INTO Facturas (
                        Clave,
                        NumeroConsecutivo,
                        FechaEmision,
                        EmisorId,
                        ReceptorId,
                        TotalVenta,
                        TotalDescuento,
                        TotalImpuesto,
                        TotalComprobante,
                        Estado,
                        NumeroFactura,
                        IdEmpleado,
                        FechaHoraFactura,
                        TotalConsumos,
                        ImpuestoICT,
                        ImpuestoServicio,
                        ImpuestoVentas,
                        TotalGeneral,
                        QuienPaga,
                        Cerrado,
                        EnFacturaExtras,
                        FechaCreacion,
                        FechaModificacion
                    ) OUTPUT INSERTED.IdFactura
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0, 0, GETDATE(), GETDATE())
                """, clave_factura, numero_consecutivo, fecha_emision, emisor_id, receptor_id,
                    total_venta, total_descuento, total_impuesto, total_comprobante, estado,
                    siguiente_numero, id_empleado, fecha_hora, total_consumos, 
                    impuesto_ict, impuesto_serv, impuesto_vent, total_general, quien_paga)
            else:
                # Si no existe NumeroFactura, no lo incluimos en el INSERT
                cursor.execute("""
                    INSERT INTO Facturas (
                        Clave,
                        NumeroConsecutivo,
                        FechaEmision,
                        EmisorId,
                        ReceptorId,
                        TotalVenta,
                        TotalDescuento,
                        TotalImpuesto,
                        TotalComprobante,
                        Estado,
                        IdEmpleado,
                        FechaHoraFactura,
                        TotalConsumos,
                        ImpuestoICT,
                        ImpuestoServicio,
                        ImpuestoVentas,
                        TotalGeneral,
                        QuienPaga,
                        Cerrado,
                        EnFacturaExtras,
                        FechaCreacion,
                        FechaModificacion
                    ) OUTPUT INSERTED.IdFactura
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0, 0, GETDATE(), GETDATE())
                """, clave_factura, numero_consecutivo, fecha_emision, emisor_id, receptor_id,
                    total_venta, total_descuento, total_impuesto, total_comprobante, estado,
                    id_empleado, fecha_hora, total_consumos, 
                    impuesto_ict, impuesto_serv, impuesto_vent, total_general, quien_paga)
            
            # Obtener el IdFactura insertado
            id_factura = cursor.fetchone()[0]
            
            # Relacionar cargos con la factura creada
            cursor.execute("""
                UPDATE Cargos
                SET IdFactura = ?
                WHERE QuienPaga = ? AND IdFactura IS NULL
            """, id_factura, quien_paga)
            
            siguiente_numero += 1
            facturas_creadas += 1
            
            if facturas_creadas % 100 == 0:
                conexion.commit()
                print(f"  Procesadas {facturas_creadas} facturas...")
        
        conexion.commit()
        print(f"  [OK] Facturas creadas: {facturas_creadas:,}")
        print()
    
    # Resumen final
    cursor.execute("SELECT COUNT(*) FROM Cargos WHERE IdFactura IS NOT NULL")
    total_relacionados = cursor.fetchone()[0]
    
    cursor.execute("SELECT COUNT(*) FROM Cargos")
    total_cargos = cursor.fetchone()[0]
    
    porcentaje = (total_relacionados / total_cargos * 100) if total_cargos > 0 else 0
    
    print("=" * 80)
    print("RESUMEN")
    print("=" * 80)
    print(f"Total de cargos: {total_cargos:,}")
    print(f"Cargos relacionados: {total_relacionados:,}")
    print(f"Porcentaje relacionado: {porcentaje:.2f}%")
    print()
    
    # Mostrar algunos cargos sin relación
    cursor.execute("""
        SELECT TOP 10
            IdCargo,
            QuienPaga,
            NumeroFolioOriginal,
            IdCheckIn,
            FechaHora,
            MontoTotal
        FROM Cargos
        WHERE IdFactura IS NULL
        ORDER BY FechaHora DESC
    """)
    
    cargos_sin_relacion = cursor.fetchall()
    if cargos_sin_relacion:
        print("Ejemplos de cargos sin relación (primeros 10):")
        print(f"{'IdCargo':<10} {'QuienPaga':<15} {'NumeroFolio':<15} {'FechaHora':<20} {'MontoTotal':<15}")
        print("-" * 80)
        for cargo in cargos_sin_relacion:
            id_cargo, quien_paga, num_folio, id_checkin, fecha_hora, monto = cargo
            fecha_str = fecha_hora.strftime('%Y-%m-%d %H:%M') if fecha_hora else 'N/A'
            print(f"{id_cargo:<10} {str(quien_paga or 'N/A'):<15} {str(num_folio or 'N/A'):<15} {fecha_str:<20} {str(monto or 0):<15}")
        print()

def main():
    import sys
    
    crear_facturas = '--crear-facturas' in sys.argv
    
    try:
        print("Conectando a SQL Server...")
        conexion = pyodbc.connect(CONNECTION_STRING)
        print("Conexión exitosa")
        print()
    except Exception as e:
        print(f"ERROR al conectar a SQL Server: {e}")
        return
    
    try:
        relacionar_cargos_con_facturas_existentes(conexion, crear_facturas=crear_facturas)
        
        print("=" * 80)
        print("PROCESO COMPLETADO")
        print("=" * 80)
        
    except Exception as e:
        print(f"ERROR durante el proceso: {e}")
        import traceback
        traceback.print_exc()
    finally:
        conexion.close()

if __name__ == "__main__":
    print()
    print("Uso:")
    print("  python RelacionarCargosFacturas.py              # Solo relacionar con facturas existentes")
    print("  python RelacionarCargosFacturas.py --crear-facturas  # También crear facturas nuevas")
    print()
    main()

