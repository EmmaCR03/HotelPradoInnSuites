#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de Migración de Cargos y Temporadas desde DBF a SQL Server
"""
import os
import sys
from struct import unpack
import pyodbc
from datetime import datetime
from datetime import timedelta

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
                    elif campo['tipo'] == 'T':
                        # DateTime: 8 bytes - 4 bytes fecha (días desde 01/01/0001) + 4 bytes tiempo (milisegundos desde medianoche)
                        if len(field_data) >= 8:
                            try:
                                days = unpack('<I', field_data[0:4])[0]
                                ms = unpack('<I', field_data[4:8])[0]
                                # Convertir días desde 01/01/0001 a fecha
                                base_date = datetime(1, 1, 1)
                                fecha = base_date + timedelta(days=days-1)
                                # Agregar milisegundos
                                fecha = fecha + timedelta(milliseconds=ms)
                                valor = fecha if fecha.year > 1900 else None
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

def migrar_temporadas(archivo_dbf, conexion):
    """Migra temporadas desde TEMPORADAS.dbf"""
    print("=" * 80)
    print("MIGRANDO TEMPORADAS DESDE TEMPORADAS.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    # Verificar/crear tabla Temporadas
    cursor = conexion.cursor()
    cursor.execute("""
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Temporadas')
        BEGIN
            CREATE TABLE [dbo].[Temporadas] (
                [IdTemporada] INT IDENTITY(1,1) NOT NULL,
                [NumeroTemporada] INT NULL,
                [Descripcion] NVARCHAR(60) NULL,
                [CodigoCuenta] BIGINT NULL,
                [AumentaAl] INT NULL,
                CONSTRAINT [PK_Temporadas] PRIMARY KEY CLUSTERED ([IdTemporada] ASC)
            );
            
            CREATE UNIQUE NONCLUSTERED INDEX [IX_Temporadas_NumeroTemporada] 
            ON [dbo].[Temporadas] ([NumeroTemporada])
            WHERE [NumeroTemporada] IS NOT NULL;
        END
    """)
    conexion.commit()
    print("Tabla Temporadas verificada/creada")
    print()
    
    temporadas_insertadas = 0
    temporadas_duplicadas = 0
    temporadas_con_error = 0
    
    numeros_procesados = set()
    
    for i in range(estructura['num_records']):
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
        num_temp = registro.get('M_NUMTEMP')
        descripcion = registro.get('DESCRIP_TE', '').strip()
        cod_cta = registro.get('COD_CTA')
        aumenta_al = registro.get('AUMENTA_AL')
        
        # Validar datos mínimos
        if not num_temp:
            continue
        
        # Evitar duplicados
        if num_temp in numeros_procesados:
            temporadas_duplicadas += 1
            continue
        numeros_procesados.add(num_temp)
        
        # Verificar si ya existe
        try:
            cursor.execute("SELECT IdTemporada FROM Temporadas WHERE NumeroTemporada = ?", (int(num_temp),))
            if cursor.fetchone():
                temporadas_duplicadas += 1
                continue
        except:
            pass
        
        # Insertar temporada
        try:
            cursor.execute("""
                INSERT INTO Temporadas (
                    NumeroTemporada,
                    Descripcion,
                    CodigoCuenta,
                    AumentaAl
                )
                VALUES (?, ?, ?, ?)
            """, (
                int(num_temp) if num_temp else None,
                descripcion[:60] if descripcion else None,
                int(cod_cta) if cod_cta else None,
                int(aumenta_al) if aumenta_al else None
            ))
            temporadas_insertadas += 1
        except Exception as e:
            temporadas_con_error += 1
            if temporadas_con_error <= 5:
                print(f"Error insertando temporada {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE TEMPORADAS")
    print("=" * 80)
    print(f"Temporadas insertadas: {temporadas_insertadas}")
    print(f"Temporadas duplicadas (omitidas): {temporadas_duplicadas}")
    print(f"Temporadas con error: {temporadas_con_error}")
    print()

def migrar_cargos(archivo_dbf, conexion):
    """Migra cargos desde CARGOS.dbf"""
    print("=" * 80)
    print("MIGRANDO CARGOS DESDE CARGOS.dbf")
    print("=" * 80)
    print()
    
    estructura = leer_estructura_dbf(archivo_dbf)
    if not estructura['exito']:
        print(f"Error leyendo estructura: {estructura['error']}")
        return
    
    print(f"Total registros: {estructura['num_records']}")
    print()
    
    # Verificar/crear tabla Cargos
    cursor = conexion.cursor()
    cursor.execute("""
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cargos')
        BEGIN
            CREATE TABLE [dbo].[Cargos] (
                [IdCargo] INT IDENTITY(1,1) NOT NULL,
                [IdCheckIn] INT NULL,
                [CodigoExtra] INT NULL,
                [DescripcionExtra] NVARCHAR(60) NULL,
                [NumeroDocumento] INT NULL,
                [MontoCargo] DECIMAL(18,2) NULL,
                [MontoServicio] DECIMAL(18,2) NULL,
                [ImpuestoVenta] DECIMAL(18,2) NULL,
                [ImpuestoHotel] DECIMAL(18,2) NULL,
                [ImpuestoServicio] DECIMAL(18,2) NULL,
                [MontoTotal] DECIMAL(18,2) NULL,
                [QuienPaga] INT NULL,
                [FechaHora] DATETIME NULL,
                [NumeroEmpleado] BIGINT NULL,
                [Cancelado] BIT NULL,
                [Notas] NVARCHAR(200) NULL,
                [EnFacturaExtras] BIT NULL,
                [CuentaError] BIT NULL,
                [NumeroCierre] INT NULL,
                [PagoImpuesto] BIT NULL,
                [TipoCambio] DECIMAL(6,2) NULL,
                [FechaTraslado] DATETIME NULL,
                [Facturar] BIT NULL,
                [Secuencia] INT NULL,
                [NoContable] BIT NULL,
                [NumeroFolioOriginal] INT NULL,
                CONSTRAINT [PK_Cargos] PRIMARY KEY CLUSTERED ([IdCargo] ASC)
            );
            
            CREATE NONCLUSTERED INDEX [IX_Cargos_NumeroFolio] 
            ON [dbo].[Cargos] ([NumeroFolioOriginal])
            WHERE [NumeroFolioOriginal] IS NOT NULL;
            
            CREATE NONCLUSTERED INDEX [IX_Cargos_FechaHora] 
            ON [dbo].[Cargos] ([FechaHora])
            WHERE [FechaHora] IS NOT NULL;
            
            IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CheckIn')
            BEGIN
                ALTER TABLE [dbo].[Cargos]
                ADD CONSTRAINT [FK_Cargos_CheckIn] 
                FOREIGN KEY ([IdCheckIn]) REFERENCES [dbo].[CheckIn] ([IdCheckIn]);
            END
        END
    """)
    conexion.commit()
    print("Tabla Cargos verificada/creada")
    print()
    
    cargos_insertados = 0
    cargos_con_error = 0
    
    for i in range(estructura['num_records']):
        if (i + 1) % 5000 == 0:
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
        cod_extra = registro.get('COD_EXTRA')
        descrip_ex = registro.get('DESCRIP_EX', '').strip()
        no_docu = registro.get('NO_DOCU')
        monto_carg = registro.get('MONTO_CARG')
        m_r_servic = registro.get('M_R_SERVIC')
        m_imp_vent = registro.get('M_IMP_VENT')
        m_imp_hote = registro.get('M_IMP_HOTE')
        m_imp_serv = registro.get('M_IMP_SERV')
        monto_tota = registro.get('MONTO_TOTA')
        q_paga = registro.get('Q_PAGA')
        fecha_hora = registro.get('FECHA_HORA')
        no_emplea = registro.get('NO_EMPLEA')
        cancelado = registro.get('CANCELADO')
        notas = registro.get('NOTAS', '').strip()
        en_fextras = registro.get('EN_FEXTRAS')
        cta_error = registro.get('CTA_ERROR')
        no_cierre = registro.get('NO_CIERRE')
        pag_imp = registro.get('PAG_IMP')
        tipo_cambv = registro.get('TIPO_CAMBV')
        f_traslado = registro.get('F_TRASLADO')
        facturar = registro.get('FACTURAR')
        secuencia = registro.get('SECUENCIA')
        no_contabl = registro.get('NO_CONTABL')
        
        # Buscar IdCheckIn por N_FOLIO (CodigoFolio en CheckIn)
        id_checkin = None
        if n_folio:
            try:
                cursor.execute(
                    "SELECT IdCheckIn FROM CheckIn WHERE CodigoFolio = ?",
                    (int(n_folio),)
                )
                result = cursor.fetchone()
                if result:
                    id_checkin = result[0]
            except:
                pass
        
        # Insertar cargo
        try:
            cursor.execute("""
                INSERT INTO Cargos (
                    IdCheckIn,
                    CodigoExtra,
                    DescripcionExtra,
                    NumeroDocumento,
                    MontoCargo,
                    MontoServicio,
                    ImpuestoVenta,
                    ImpuestoHotel,
                    ImpuestoServicio,
                    MontoTotal,
                    QuienPaga,
                    FechaHora,
                    NumeroEmpleado,
                    Cancelado,
                    Notas,
                    EnFacturaExtras,
                    CuentaError,
                    NumeroCierre,
                    PagoImpuesto,
                    TipoCambio,
                    FechaTraslado,
                    Facturar,
                    Secuencia,
                    NoContable,
                    NumeroFolioOriginal
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                id_checkin,
                int(cod_extra) if cod_extra and cod_extra <= 2147483647 else None,
                descrip_ex[:60] if descrip_ex else None,
                int(no_docu) if no_docu and isinstance(no_docu, (int, float)) else None,
                float(monto_carg) if monto_carg else None,
                float(m_r_servic) if m_r_servic else None,
                float(m_imp_vent) if m_imp_vent else None,
                float(m_imp_hote) if m_imp_hote else None,
                float(m_imp_serv) if m_imp_serv else None,
                float(monto_tota) if monto_tota else None,
                int(q_paga) if q_paga and isinstance(q_paga, (int, float)) else None,
                fecha_hora,
                int(no_emplea) if no_emplea else None,
                bool(cancelado) if cancelado is not None else None,
                notas[:200] if notas else None,
                bool(en_fextras) if en_fextras is not None else None,
                bool(cta_error) if cta_error is not None else None,
                int(no_cierre) if no_cierre and isinstance(no_cierre, (int, float)) else None,
                bool(pag_imp) if pag_imp is not None else None,
                float(tipo_cambv) if tipo_cambv else None,
                f_traslado,
                bool(facturar) if facturar is not None else None,
                int(secuencia) if secuencia and secuencia <= 2147483647 else None,
                bool(no_contabl) if no_contabl is not None else None,
                int(n_folio) if n_folio and isinstance(n_folio, (int, float)) else None
            ))
            cargos_insertados += 1
        except Exception as e:
            cargos_con_error += 1
            if cargos_con_error <= 5:
                print(f"Error insertando cargo {i + 1}: {e}")
    
    conexion.commit()
    
    print()
    print("=" * 80)
    print("RESUMEN DE MIGRACION DE CARGOS")
    print("=" * 80)
    print(f"Cargos insertados: {cargos_insertados}")
    print(f"Cargos con error: {cargos_con_error}")
    print()

def main():
    print("=" * 80)
    print("SCRIPT DE MIGRACION DE CARGOS Y TEMPORADAS")
    print("=" * 80)
    print()
    
    base_path = "dbViejaHotel/sicre_d"
    
    archivo_cargos = os.path.join(base_path, "CARGOS.dbf")
    archivo_temporadas = os.path.join(base_path, "TEMPORADAS.dbf")
    
    if not os.path.exists(archivo_cargos):
        print(f"ERROR: No se encontró {archivo_cargos}")
        return
    
    if not os.path.exists(archivo_temporadas):
        print(f"ERROR: No se encontró {archivo_temporadas}")
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
        # Migrar temporadas primero (son pocas)
        migrar_temporadas(archivo_temporadas, conexion)
        
        # Migrar cargos (son muchos)
        migrar_cargos(archivo_cargos, conexion)
        
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

