#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de Migración de Empresas y Check-In/Check-Out desde DBF a SQL Server
"""
import os
import sys
from struct import unpack
import pyodbc
from datetime import datetime

# Configuración de conexión a SQL Server
CONNECTION_STRING = "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost;DATABASE=HotelPrado;Trusted_Connection=yes;"

def leer_registro_dbf(archivo, campos, record_num, header_size, record_size):
    """Lee un registro específico de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            f.seek(header_size + (record_num * record_size))
            
            delete_flag = unpack('B', f.read(1))[0]
            if delete_flag == 0x2A:
                return None
            
            registro = {}
            for campo in campos:
                field_data = f.read(campo['longitud'])
                try:
                    if campo['tipo'] == 'C':
                        valor = field_data.decode('latin-1', errors='ignore').strip()
                        valor = valor.encode('ascii', errors='ignore').decode('ascii')
                    elif campo['tipo'] == 'N':
                        valor_str = field_data.decode('ascii', errors='ignore').strip()
                        try:
                            if campo['decimales'] > 0:
                                valor = float(valor_str) if valor_str else None
                            else:
                                valor = int(valor_str) if valor_str else None
                        except:
                            valor = None
                    elif campo['tipo'] == 'D':
                        fecha_str = field_data.decode('ascii', errors='ignore').strip()
                        if len(fecha_str) == 8:
                            try:
                                year = int(fecha_str[:4])
                                month = int(fecha_str[4:6])
                                day = int(fecha_str[6:8])
                                valor = datetime(year, month, day) if year > 1900 else None
                            except:
                                valor = None
                        else:
                            valor = None
                    elif campo['tipo'] == 'L':
                        valor = field_data[0] in [b'T', b't', b'Y', b'y', ord('T'), ord('t'), ord('Y'), ord('y')]
                    elif campo['tipo'] == 'I':
                        valor = unpack('<i', field_data[:4])[0] if len(field_data) >= 4 else None
                    else:
                        valor = field_data.decode('latin-1', errors='ignore').strip()
                except:
                    valor = None
                
                registro[campo['nombre']] = valor
            
            return registro
    except Exception as e:
        return None

def leer_estructura_dbf(archivo):
    """Lee la estructura de un archivo DBF"""
    try:
        with open(archivo, 'rb') as f:
            signature = unpack('B', f.read(1))[0]
            year, month, day = unpack('BBB', f.read(3))
            
            f.seek(4)
            num_records = unpack('<I', f.read(4))[0]
            
            f.seek(8)
            header_size = unpack('<H', f.read(2))[0]
            
            f.seek(10)
            record_size = unpack('<H', f.read(2))[0]
            
            num_fields = (header_size - 33) // 32
            
            campos = []
            f.seek(32)
            
            for i in range(num_fields):
                field_data = f.read(32)
                field_name = field_data[0:11].decode('ascii', errors='ignore').strip('\x00')
                field_type = chr(field_data[11])
                field_length = field_data[16]
                field_decimals = field_data[17]
                
                if field_name:
                    campos.append({
                        'nombre': field_name,
                        'tipo': field_type,
                        'longitud': field_length,
                        'decimales': field_decimals
                    })
            
            return {
                'exito': True,
                'num_records': num_records,
                'header_size': header_size,
                'record_size': record_size,
                'campos': campos
            }
    except Exception as e:
        return {
            'exito': False,
            'error': str(e)
        }

def migrar_empresas(archivo_dbf, conexion):
    """Migra empresas desde EMPRESAS.dbf"""
    print("=" * 80)
    print("MIGRANDO EMPRESAS DESDE EMPRESAS.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    cursor = conexion.cursor()
    empresas_insertadas = 0
    empresas_duplicadas = 0
    empresas_con_error = 0
    
    codigos_procesados = set()
    
    for i in range(estructura['num_records']):
        if (i + 1) % 100 == 0:
            print(f"Procesando registro {i + 1}/{estructura['num_records']}...")
        
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            i, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if not registro:
            continue
        
        # Extraer datos
        cod_empresa = registro.get('COD_EMPR')
        nombre_empresa = registro.get('NOM_EMPR', '').strip()
        telefono1 = registro.get('TELEFONO1')
        telefono2 = registro.get('TELEFONO2')
        fax = registro.get('FAX')
        contacto = registro.get('CONTACTO', '').strip()
        direccion = registro.get('DIRECCION', '').strip()
        observaciones = registro.get('OBSERVA_E', '').strip()
        limite_credito = registro.get('LIMITE_CRE')
        correo = registro.get('CORREORECE', '').strip()
        
        # Validar datos mínimos
        if not nombre_empresa or len(nombre_empresa) == 0:
            continue
        
        # Evitar duplicados por código
        if cod_empresa:
            if cod_empresa in codigos_procesados:
                empresas_duplicadas += 1
                continue
            codigos_procesados.add(cod_empresa)
        
        # Convertir teléfonos
        telefono1_str = None
        if telefono1:
            try:
                telefono1_str = str(int(telefono1)) if isinstance(telefono1, (int, float)) else str(telefono1).strip()
            except:
                telefono1_str = str(telefono1).strip() if telefono1 else None
        
        telefono2_str = None
        if telefono2:
            try:
                telefono2_str = str(int(telefono2)) if isinstance(telefono2, (int, float)) else str(telefono2).strip()
            except:
                telefono2_str = str(telefono2).strip() if telefono2 else None
        
        fax_str = None
        if fax:
            try:
                fax_str = str(int(fax)) if isinstance(fax, (int, float)) else str(fax).strip()
            except:
                fax_str = str(fax).strip() if fax else None
        
        # Insertar empresa
        try:
            cursor.execute("""
                INSERT INTO Empresas (
                    CodigoEmpresa,
                    NombreEmpresa,
                    Telefono1,
                    Telefono2,
                    Fax,
                    Contacto,
                    Direccion,
                    Observaciones,
                    LimiteCredito,
                    CorreoElectronico
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                int(cod_empresa) if cod_empresa else None,
                nombre_empresa[:100] if nombre_empresa else None,
                telefono1_str[:15] if telefono1_str else None,
                telefono2_str[:15] if telefono2_str else None,
                fax_str[:15] if fax_str else None,
                contacto[:100] if contacto else None,
                direccion[:300] if direccion else None,
                observaciones[:500] if observaciones else None,
                float(limite_credito) if limite_credito else None,
                correo[:100] if correo else None
            ))
            empresas_insertadas += 1
        except Exception as e:
            empresas_con_error += 1
            if empresas_con_error <= 5:
                print(f"Error insertando empresa {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE EMPRESAS")
    print("=" * 80)
    print(f"Empresas insertadas: {empresas_insertadas}")
    print(f"Empresas duplicadas (omitidas): {empresas_duplicadas}")
    print(f"Empresas con error: {empresas_con_error}")
    print()

def migrar_checkin(archivo_dbf, conexion):
    """Migra check-in/check-out desde CHEKIN.dbf"""
    print("=" * 80)
    print("MIGRANDO CHECK-IN/CHECK-OUT DESDE CHEKIN.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    # Verificar si existe tabla CheckIn
    cursor = conexion.cursor()
    cursor.execute("""
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CheckIn')
        BEGIN
            CREATE TABLE [dbo].[CheckIn] (
                [IdCheckIn] INT IDENTITY(1,1) NOT NULL,
                [IdReserva] INT NULL,
                [IdCliente] INT NULL,
                [IdHabitacion] INT NULL,
                [NombreCliente] NVARCHAR(100) NULL,
                [CedulaCliente] VARCHAR(16) NULL,
                [TelefonoCliente] VARCHAR(15) NULL,
                [DireccionCliente] NVARCHAR(200) NULL,
                [FechaCheckIn] DATETIME NULL,
                [FechaCheckOut] DATETIME NULL,
                [NumeroAdultos] INT NULL,
                [NumeroNinos] INT NULL,
                [Total] DECIMAL(18,2) NULL,
                [Estado] NVARCHAR(50) NULL,
                [Observaciones] NVARCHAR(500) NULL,
                [IdEmpresa] INT NULL,
                [CodigoFolio] INT NULL,
                [NumeroTransaccion] INT NULL,
                CONSTRAINT [PK_CheckIn] PRIMARY KEY CLUSTERED ([IdCheckIn] ASC),
                CONSTRAINT [FK_CheckIn_Reservas] FOREIGN KEY ([IdReserva]) REFERENCES [dbo].[Reservas] ([IdReserva]),
                CONSTRAINT [FK_CheckIn_Cliente] FOREIGN KEY ([IdCliente]) REFERENCES [dbo].[Cliente] ([IdCliente]),
                CONSTRAINT [FK_CheckIn_Habitaciones] FOREIGN KEY ([IdHabitacion]) REFERENCES [dbo].[Habitaciones] ([IdHabitacion]),
                CONSTRAINT [FK_CheckIn_Empresas] FOREIGN KEY ([IdEmpresa]) REFERENCES [dbo].[Empresas] ([IdEmpresa])
            );
        END
    """)
    conexion.commit()
    print("Tabla CheckIn verificada/creada")
    print()
    
    checkins_insertados = 0
    checkins_sin_cliente = 0
    checkins_con_error = 0
    
    for i in range(estructura['num_records']):
        if (i + 1) % 1000 == 0:
            print(f"Procesando registro {i + 1}/{estructura['num_records']}...")
        
        registro = leer_registro_dbf(
            archivo_dbf, 
            estructura['campos'], 
            i, 
            estructura['header_size'], 
            estructura['record_size']
        )
        
        if not registro:
            continue
        
        # Extraer datos
        n_folio = registro.get('N_FOLIO')
        no_habita = registro.get('NO_HABITA')
        cliente = registro.get('CLIENTE', '').strip()
        cedula = registro.get('CEDULA', '').strip()
        telefono = registro.get('TELEFONO')
        direccion = registro.get('DIRECCION', '').strip()
        cod_empresa = registro.get('COD_EMPR')
        fecha_in = registro.get('FECHA_IN')
        fecha_out = registro.get('FECHA_OUT')
        n_adultos = registro.get('N_ADULTOS')
        n_ninos = registro.get('N_NINOS')
        total = registro.get('TOTAL')
        estatus = registro.get('ESTATUS')
        observaciones = registro.get('OBSERVA_R', '').strip()
        num_tran = registro.get('NUM_TRAN')
        no_reserva = registro.get('NO_RESERVA')
        
        # Buscar cliente
        id_cliente = None
        if cedula:
            cursor.execute("SELECT IdCliente FROM Cliente WHERE CedulaCliente = ?", (cedula,))
            result = cursor.fetchone()
            if result:
                id_cliente = result[0]
        
        if not id_cliente and cliente:
            nombre_buscar = cliente[:50]
            cursor.execute(
                "SELECT TOP 1 IdCliente FROM Cliente WHERE NombreCliente LIKE ?",
                (f"%{nombre_buscar}%",)
            )
            result = cursor.fetchone()
            if result:
                id_cliente = result[0]
        
        # Buscar reserva
        id_reserva = None
        if no_reserva:
            # Buscar por algún campo relacionado (puede necesitar ajuste)
            pass
        
        # Buscar habitación
        id_habitacion = None
        if no_habita:
            try:
                cursor.execute("SELECT IdHabitacion FROM Habitaciones WHERE NumeroHabitacion = ?", (int(no_habita),))
                result = cursor.fetchone()
                if result:
                    id_habitacion = result[0]
            except:
                pass
        
        # Buscar empresa
        id_empresa = None
        if cod_empresa:
            try:
                cursor.execute("SELECT IdEmpresa FROM Empresas WHERE CodigoEmpresa = ?", (int(cod_empresa),))
                result = cursor.fetchone()
                if result:
                    id_empresa = result[0]
            except:
                pass
        
        # Convertir teléfono
        telefono_str = None
        if telefono:
            try:
                telefono_str = str(int(telefono)) if isinstance(telefono, (int, float)) else str(telefono).strip()
            except:
                telefono_str = str(telefono).strip() if telefono else None
        
        # Determinar estado
        estado = 'Activo'
        if estatus:
            if estatus == 0:
                estado = 'Check-In'
            elif estatus == 1:
                estado = 'Check-Out'
            elif estatus == 2:
                estado = 'Cancelado'
        
        # Insertar check-in
        try:
            cursor.execute("""
                INSERT INTO CheckIn (
                    IdReserva,
                    IdCliente,
                    IdHabitacion,
                    NombreCliente,
                    CedulaCliente,
                    TelefonoCliente,
                    DireccionCliente,
                    FechaCheckIn,
                    FechaCheckOut,
                    NumeroAdultos,
                    NumeroNinos,
                    Total,
                    Estado,
                    Observaciones,
                    IdEmpresa,
                    CodigoFolio,
                    NumeroTransaccion
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                id_reserva,
                id_cliente,
                id_habitacion if id_habitacion else 1,  # Temporal si no existe
                cliente[:100] if cliente else None,
                cedula[:16] if cedula else None,
                telefono_str[:15] if telefono_str else None,
                direccion[:200] if direccion else None,
                fecha_in,
                fecha_out,
                int(n_adultos) if n_adultos else None,
                int(n_ninos) if n_ninos else None,
                float(total) if total else None,
                estado,
                observaciones[:500] if observaciones else None,
                id_empresa,
                int(n_folio) if n_folio else None,
                int(num_tran) if num_tran else None
            ))
            checkins_insertados += 1
        except Exception as e:
            checkins_con_error += 1
            if checkins_con_error <= 5:
                print(f"Error insertando check-in {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE CHECK-IN")
    print("=" * 80)
    print(f"Check-ins insertados: {checkins_insertados}")
    print(f"Check-ins sin cliente (omitidos): {checkins_sin_cliente}")
    print(f"Check-ins con error: {checkins_con_error}")
    print()

def main():
    print("=" * 80)
    print("SCRIPT DE MIGRACION DE EMPRESAS Y CHECK-IN")
    print("=" * 80)
    print()
    
    base_path = "dbViejaHotel/sicre_d"
    
    archivo_empresas = os.path.join(base_path, "EMPRESAS.dbf")
    archivo_chekin = os.path.join(base_path, "CHEKIN.dbf")
    
    if not os.path.exists(archivo_empresas):
        print(f"ERROR: No se encontró {archivo_empresas}")
        return
    
    if not os.path.exists(archivo_chekin):
        print(f"ERROR: No se encontró {archivo_chekin}")
        return
    
    try:
        print("Conectando a SQL Server...")
        conexion = pyodbc.connect(CONNECTION_STRING)
        print("Conexión exitosa")
        print()
    except Exception as e:
        print(f"ERROR al conectar a SQL Server: {e}")
        return
    
    try:
        # Migrar empresas primero
        migrar_empresas(archivo_empresas, conexion)
        
        # Migrar check-in
        migrar_checkin(archivo_chekin, conexion)
        
        print("=" * 80)
        print("MIGRACION COMPLETADA")
        print("=" * 80)
        
    except Exception as e:
        print(f"ERROR durante la migración: {e}")
        import traceback
        traceback.print_exc()
    finally:
        conexion.close()

if __name__ == "__main__":
    main()

