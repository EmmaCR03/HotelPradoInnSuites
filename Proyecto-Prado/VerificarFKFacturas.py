#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import pyodbc

CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

conn = pyodbc.connect(CONNECTION_STRING)
cursor = conn.cursor()

# Verificar si EmisorId y ReceptorId permiten NULL
cursor.execute("""
    SELECT c.name, c.is_nullable 
    FROM sys.columns c 
    WHERE c.object_id = OBJECT_ID('dbo.Facturas') 
    AND c.name IN ('EmisorId', 'ReceptorId')
""")

print("Columnas EmisorId y ReceptorId:")
for row in cursor.fetchall():
    print(f"  {row[0]}: {'NULL permitido' if row[1] else 'NOT NULL'}")

# Verificar si existen tablas Emisores y Receptores
cursor.execute("SELECT COUNT(*) FROM sys.tables WHERE name IN ('Emisores', 'Receptores')")
count = cursor.fetchone()[0]
print(f"\nTablas relacionadas encontradas: {count}")

if count > 0:
    # Intentar obtener un ID válido de Emisores
    try:
        cursor.execute("SELECT TOP 1 Id FROM Emisores")
        row = cursor.fetchone()
        if row:
            print(f"EmisorId válido encontrado: {row[0]}")
        else:
            print("Tabla Emisores existe pero está vacía")
    except:
        print("No se pudo consultar Emisores")
    
    # Intentar obtener un ID válido de Receptores
    try:
        cursor.execute("SELECT TOP 1 Id FROM Receptores")
        row = cursor.fetchone()
        if row:
            print(f"ReceptorId válido encontrado: {row[0]}")
        else:
            print("Tabla Receptores existe pero está vacía")
    except:
        print("No se pudo consultar Receptores")

conn.close()

