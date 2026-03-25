#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import pyodbc

CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

conn = pyodbc.connect(CONNECTION_STRING)
cursor = conn.cursor()

cursor.execute("""
    SELECT c.name, t.name, c.max_length, c.precision, c.scale
    FROM sys.columns c 
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id 
    WHERE c.object_id = OBJECT_ID('dbo.Facturas') 
    AND c.is_nullable = 0 
    ORDER BY c.name
""")

print("Columnas NOT NULL en Facturas:")
print("-" * 60)
for row in cursor.fetchall():
    col_name, type_name, max_len, precision, scale = row
    if type_name in ['varchar', 'nvarchar', 'char', 'nchar']:
        print(f"  {col_name}: {type_name}({max_len})")
    elif type_name in ['decimal', 'numeric']:
        print(f"  {col_name}: {type_name}({precision},{scale})")
    else:
        print(f"  {col_name}: {type_name}")

conn.close()

