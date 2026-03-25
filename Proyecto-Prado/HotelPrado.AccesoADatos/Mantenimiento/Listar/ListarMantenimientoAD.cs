using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace HotelPrado.AccesoADatos.Mantenimiento.Listar
{
    public class ListarMantenimientoAD : IListarMantenimientoAD
    {
        Contexto _contexto;

        public ListarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public List<MantenimientoDTO> Listar()
        {
            var laListaDeMantenimientos = new List<MantenimientoDTO>();
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Primero verificamos qué columnas existen en la tabla Mantenimiento
                var columnasExistentes = new List<string>();
                string checkColumnsQuery = @"
                    SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mantenimiento'";
                
                using (var checkCommand = new SqlCommand(checkColumnsQuery, connection))
                {
                    using (var reader = checkCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columnasExistentes.Add(reader.GetString(0));
                        }
                    }
                }
                
                // Verificar si existe alguna columna relacionada con habitación
                bool existeColumnaHabitacion = columnasExistentes.Any(c => 
                    c.Equals("IdHabitacion", System.StringComparison.OrdinalIgnoreCase) || 
                    c.Equals("idHabitacion", System.StringComparison.OrdinalIgnoreCase));
                
                string nombreColumnaHabitacion = columnasExistentes.FirstOrDefault(c => 
                    c.Equals("IdHabitacion", System.StringComparison.OrdinalIgnoreCase) || 
                    c.Equals("idHabitacion", System.StringComparison.OrdinalIgnoreCase));
                
                // Construir la consulta SQL dinámicamente
                string query;
                if (existeColumnaHabitacion && !string.IsNullOrEmpty(nombreColumnaHabitacion))
                {
                    // Si existe la columna, hacer el JOIN
                    query = $@"
                        SELECT 
                            m.IdMantenimiento,
                            m.Descripcion,
                            m.Estado,
                            m.idDepartamento,
                            m.DepartamentoNombre,
                            m.[{nombreColumnaHabitacion}] as idHabitacion,
                            CASE 
                                WHEN h.IdHabitacion IS NOT NULL THEN 'Habitación ' + ISNULL(h.NumeroHabitacion, CAST(h.IdHabitacion AS VARCHAR))
                                ELSE NULL
                            END AS HabitacionNombre
                        FROM Mantenimiento m
                        LEFT JOIN Habitaciones h ON m.[{nombreColumnaHabitacion}] = h.IdHabitacion";
                }
                else
                {
                    // Si NO existe la columna, no incluirla y poner valores por defecto
                    query = @"
                        SELECT 
                            m.IdMantenimiento,
                            m.Descripcion,
                            m.Estado,
                            m.idDepartamento,
                            m.DepartamentoNombre,
                            0 as idHabitacion,
                            NULL AS HabitacionNombre
                        FROM Mantenimiento m";
                }
                
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laListaDeMantenimientos.Add(new MantenimientoDTO
                            {
                                IdMantenimiento = reader.GetInt32(reader.GetOrdinal("IdMantenimiento")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                                idDepartamento = reader.IsDBNull(reader.GetOrdinal("idDepartamento")) ? 0 : reader.GetInt32(reader.GetOrdinal("idDepartamento")),
                                DepartamentoNombre = reader.IsDBNull(reader.GetOrdinal("DepartamentoNombre")) ? null : reader.GetString(reader.GetOrdinal("DepartamentoNombre")),
                                idHabitacion = reader.IsDBNull(reader.GetOrdinal("idHabitacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("idHabitacion")),
                                HabitacionNombre = reader.IsDBNull(reader.GetOrdinal("HabitacionNombre")) ? null : reader.GetString(reader.GetOrdinal("HabitacionNombre"))
                            });
                        }
                    }
                }
            }

            return laListaDeMantenimientos;
        }
    }
}