#!/usr/bin/env python3
# -*- coding: utf-8 -*-
from EscanearDBF_Importantes import leer_estructura_dbf
import os

base_path = 'dbViejaHotel/sicre_d'

print("=" * 80)
print("ESTRUCTURA DE CARGOS.dbf")
print("=" * 80)
archivo_cargos = os.path.join(base_path, 'CARGOS.dbf')
r_cargos = leer_estructura_dbf(archivo_cargos)
if r_cargos['exito']:
    print(f"Registros: {r_cargos['num_records']}")
    print(f"Campos: {len(r_cargos['campos'])}")
    print("\nTodos los campos:")
    for c in r_cargos['campos']:
        print(f"  - {c['nombre']:<20} {c['tipo']} ({c['longitud']} bytes)")
    if r_cargos['muestras']:
        print("\nMuestra de datos (primer registro):")
        for k, v in list(r_cargos['muestras'][0].items())[:15]:
            print(f"  {k}: {v}")

print("\n" + "=" * 80)
print("ESTRUCTURA DE TEMPORADAS.dbf")
print("=" * 80)
archivo_temporadas = os.path.join(base_path, 'TEMPORADAS.dbf')
r_temp = leer_estructura_dbf(archivo_temporadas)
if r_temp['exito']:
    print(f"Registros: {r_temp['num_records']}")
    print(f"Campos: {len(r_temp['campos'])}")
    print("\nTodos los campos:")
    for c in r_temp['campos']:
        print(f"  - {c['nombre']:<20} {c['tipo']} ({c['longitud']} bytes)")
    if r_temp['muestras']:
        print("\nMuestra de datos (primer registro):")
        for k, v in r_temp['muestras'][0].items():
            print(f"  {k}: {v}")

