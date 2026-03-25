#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import pyodbc

CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

conn = pyodbc.connect(CONNECTION_STRING)
cursor = conn.cursor()

cursor.execute("""
    SELECT 
        c.name AS NombreColumna,
        t.name AS TipoDato,
        c.max_length,
        c.precision,
        c.scale,
        c.is_nullable,
        c.is_identity
    FROM sys.columns c 
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id 
    WHERE c.object_id = OBJECT_ID('dbo.Facturas') 
    ORDER BY c.column_id
""")

print("TODAS las columnas de la tabla Facturas:")
print("=" * 80)
print(f"{'Columna':<30} {'Tipo':<20} {'NULL':<8} {'Identity':<10}")
print("-" * 80)

for row in cursor.fetchall():
    col_name, type_name, max_len, precision, scale, is_nullable, is_identity = row
    
    if type_name in ['varchar', 'nvarchar', 'char', 'nchar']:
        tipo_completo = f"{type_name}({max_len})"
    elif type_name in ['decimal', 'numeric']:
        tipo_completo = f"{type_name}({precision},{scale})"
    else:
        tipo_completo = type_name
    
    nullable = "SÍ" if is_nullable else "NO"
    identity = "SÍ" if is_identity else "NO"
    
    print(f"{col_name:<30} {tipo_completo:<20} {nullable:<8} {identity:<10}")

conn.close()

