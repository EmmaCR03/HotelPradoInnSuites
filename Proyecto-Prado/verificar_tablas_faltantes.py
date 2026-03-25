#!/usr/bin/env python3
# -*- coding: utf-8 -*-
from EscanearDBF_Importantes import leer_estructura_dbf
import os

base_path = 'dbViejaHotel/sicre_d'

# Tablas que ya migramos
tablas_migradas = ['EXTRAS_AUT', 'RESERVAS', 'EMPRESAS', 'CHEKIN']

# Tablas importantes que podrían faltar
tablas_importantes = ['HABITAC', 'TIPOS_HAB', 'EMPLEADOS', 'CARGOS', 'TARIFAS', 'TEMPORADAS']

print("=" * 80)
print("VERIFICACIÓN DE TABLAS IMPORTANTES")
print("=" * 80)
print()

print("TABLAS YA MIGRADAS:")
for tabla in tablas_migradas:
    archivo = os.path.join(base_path, tabla + '.dbf')
    if os.path.exists(archivo):
        r = leer_estructura_dbf(archivo)
        if r['exito']:
            print(f"  [OK] {tabla}: {r['num_records']} registros")
    else:
        print(f"  [?] {tabla}: NO ENCONTRADO")

print()
print("TABLAS IMPORTANTES NO MIGRADAS:")
for tabla in tablas_importantes:
    archivo = os.path.join(base_path, tabla + '.dbf')
    if os.path.exists(archivo):
        r = leer_estructura_dbf(archivo)
        if r['exito']:
            print(f"  [!] {tabla}: {r['num_records']} registros - NO MIGRADA")
            # Mostrar algunos campos importantes
            campos_principales = [c['nombre'] for c in r['campos'][:5]]
            print(f"      Campos principales: {', '.join(campos_principales)}")
    else:
        print(f"  - {tabla}: NO EXISTE")

print()
print("=" * 80)
print("RESUMEN")
print("=" * 80)
print()
print("Las tablas más importantes para un hotel son:")
print("  1. Clientes (EXTRAS_AUT) ✓")
print("  2. Reservas (RESERVAS) ✓")
print("  3. Check-In/Check-Out (CHEKIN) ✓")
print("  4. Empresas (EMPRESAS) ✓")
print()
print("Otras tablas que podrían ser útiles:")
print("  - HABITAC: Información de habitaciones")
print("  - TIPOS_HAB: Tipos de habitaciones")
print("  - CARGOS: Cargos/facturación")
print("  - EMPLEADOS: Información de empleados")
print("  - TARIFAS: Tarifas del hotel")
print("  - TEMPORADAS: Temporadas de tarifas")

