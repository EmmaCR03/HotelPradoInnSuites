using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cliente.Listar;
using HotelPrado.Abstracciones.Modelos.Cliente;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelPrado.AccesoADatos.Cliente.Listar
{
    public class ListarClientesAD : IListarClientesAD
    {
        Contexto _contexto;

        public ListarClientesAD()
        {
            _contexto = new Contexto();
        }

        public List<ClienteDTO> Listar(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true)
        {
            var laListaDeClientes = new List<ClienteDTO>();
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Limpiar y normalizar la búsqueda
                string busquedaNormalizada = null;
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    busquedaNormalizada = busqueda.Trim();
                }
                
                // Construir la consulta con búsqueda y ordenamiento
                // Manejar TelefonoCliente que puede ser INT o VARCHAR
                string query = @"
                    SELECT 
                        c.IdCliente,
                        c.NombreCliente,
                        c.PrimerApellidoCliente,
                        c.SegundoApellidoCliente,
                        c.EmailCliente,
                        CASE 
                            WHEN c.TelefonoCliente IS NULL THEN NULL
                            ELSE CAST(c.TelefonoCliente AS VARCHAR(15))
                        END AS TelefonoCliente,
                        c.DireccionCliente,
                        c.CedulaCliente,
                        c.IdEmpresa,
                        c.IdUsuario
                    FROM Cliente c
                    WHERE 1=1";
                
                // Agregar filtro de búsqueda (case-insensitive)
                if (!string.IsNullOrEmpty(busquedaNormalizada))
                {
                    query += @"
                        AND (
                            (c.NombreCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.NombreCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.PrimerApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.PrimerApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.SegundoApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.SegundoApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.EmailCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.EmailCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.CedulaCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.CedulaCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.TelefonoCliente IS NOT NULL AND CAST(c.TelefonoCliente AS VARCHAR(15)) LIKE @Busqueda)
                        )";
                }
                
                // Validar y construir ORDER BY
                var camposOrdenamientoValidos = new[] { "IdCliente", "NombreCliente", "EmailCliente", "CedulaCliente", "TelefonoCliente" };
                if (!camposOrdenamientoValidos.Contains(ordenarPor))
                {
                    ordenarPor = "IdCliente";
                }
                
                string direccionOrden = ordenAscendente ? "ASC" : "DESC";
                query += $" ORDER BY c.{ordenarPor} {direccionOrden}";
                
                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(busquedaNormalizada))
                    {
                        command.Parameters.AddWithValue("@Busqueda", $"%{busquedaNormalizada}%");
                    }
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laListaDeClientes.Add(new ClienteDTO
                            {
                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? null : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                PrimerApellidoCliente = reader.IsDBNull(reader.GetOrdinal("PrimerApellidoCliente")) ? null : reader.GetString(reader.GetOrdinal("PrimerApellidoCliente")),
                                SegundoApellidoCLiente = reader.IsDBNull(reader.GetOrdinal("SegundoApellidoCliente")) ? null : reader.GetString(reader.GetOrdinal("SegundoApellidoCliente")),
                                EmailCliente = reader.IsDBNull(reader.GetOrdinal("EmailCliente")) ? null : reader.GetString(reader.GetOrdinal("EmailCliente")),
                                TelefonoCliente = reader.IsDBNull(reader.GetOrdinal("TelefonoCliente")) ? null : reader.GetString(reader.GetOrdinal("TelefonoCliente")),
                                DireccionCliente = reader.IsDBNull(reader.GetOrdinal("DireccionCliente")) ? null : reader.GetString(reader.GetOrdinal("DireccionCliente")),
                                CedulaCliente = reader.IsDBNull(reader.GetOrdinal("CedulaCliente")) ? null : reader.GetString(reader.GetOrdinal("CedulaCliente")),
                                IdEmpresa = reader.IsDBNull(reader.GetOrdinal("IdEmpresa")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEmpresa")),
                                IdUsuario = reader.IsDBNull(reader.GetOrdinal("IdUsuario")) ? null : reader.GetString(reader.GetOrdinal("IdUsuario"))
                            });
                        }
                    }
                }
            }

            return laListaDeClientes;
        }

        public List<ClienteDTO> ListarPaginado(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true, int pagina = 1, int tamanoPagina = 50, string filtroUsuario = null)
        {
            var laListaDeClientes = new List<ClienteDTO>();
            
            // Validar parámetros
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 50;
            if (tamanoPagina > 200) tamanoPagina = 200; // Límite máximo
            
            int skip = (pagina - 1) * tamanoPagina;
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Limpiar y normalizar la búsqueda
                string busquedaNormalizada = null;
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    busquedaNormalizada = busqueda.Trim();
                }
                
                // Construir la consulta con búsqueda, ordenamiento y paginación
                string query = @"
                    SELECT 
                        c.IdCliente,
                        c.NombreCliente,
                        c.PrimerApellidoCliente,
                        c.SegundoApellidoCliente,
                        c.EmailCliente,
                        CASE 
                            WHEN c.TelefonoCliente IS NULL THEN NULL
                            ELSE CAST(c.TelefonoCliente AS VARCHAR(15))
                        END AS TelefonoCliente,
                        c.DireccionCliente,
                        c.CedulaCliente,
                        c.IdEmpresa,
                        c.IdUsuario
                    FROM Cliente c
                    WHERE 1=1";
                
                // Agregar filtro de búsqueda (case-insensitive)
                if (!string.IsNullOrEmpty(busquedaNormalizada))
                {
                    query += @"
                        AND (
                            (c.NombreCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.NombreCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.PrimerApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.PrimerApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.SegundoApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.SegundoApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.EmailCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.EmailCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.CedulaCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.CedulaCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.TelefonoCliente IS NOT NULL AND CAST(c.TelefonoCliente AS VARCHAR(15)) LIKE @Busqueda)
                        )";
                }
                
                // Agregar filtro de usuario
                if (!string.IsNullOrEmpty(filtroUsuario))
                {
                    if (filtroUsuario == "conUsuario")
                    {
                        query += " AND c.IdUsuario IS NOT NULL";
                    }
                    else if (filtroUsuario == "sinUsuario")
                    {
                        query += " AND c.IdUsuario IS NULL";
                    }
                }
                
                // Validar y construir ORDER BY
                var camposOrdenamientoValidos = new[] { "IdCliente", "NombreCliente", "EmailCliente", "CedulaCliente", "TelefonoCliente" };
                if (!camposOrdenamientoValidos.Contains(ordenarPor))
                {
                    ordenarPor = "IdCliente";
                }
                
                string direccionOrden = ordenAscendente ? "ASC" : "DESC";
                query += $" ORDER BY c.{ordenarPor} {direccionOrden}";
                
                // Agregar paginación
                query += $" OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
                
                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(busquedaNormalizada))
                    {
                        command.Parameters.AddWithValue("@Busqueda", $"%{busquedaNormalizada}%");
                    }
                    command.Parameters.AddWithValue("@Skip", skip);
                    command.Parameters.AddWithValue("@Take", tamanoPagina);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laListaDeClientes.Add(new ClienteDTO
                            {
                                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NombreCliente")) ? null : reader.GetString(reader.GetOrdinal("NombreCliente")),
                                PrimerApellidoCliente = reader.IsDBNull(reader.GetOrdinal("PrimerApellidoCliente")) ? null : reader.GetString(reader.GetOrdinal("PrimerApellidoCliente")),
                                SegundoApellidoCLiente = reader.IsDBNull(reader.GetOrdinal("SegundoApellidoCliente")) ? null : reader.GetString(reader.GetOrdinal("SegundoApellidoCliente")),
                                EmailCliente = reader.IsDBNull(reader.GetOrdinal("EmailCliente")) ? null : reader.GetString(reader.GetOrdinal("EmailCliente")),
                                TelefonoCliente = reader.IsDBNull(reader.GetOrdinal("TelefonoCliente")) ? null : reader.GetString(reader.GetOrdinal("TelefonoCliente")),
                                DireccionCliente = reader.IsDBNull(reader.GetOrdinal("DireccionCliente")) ? null : reader.GetString(reader.GetOrdinal("DireccionCliente")),
                                CedulaCliente = reader.IsDBNull(reader.GetOrdinal("CedulaCliente")) ? null : reader.GetString(reader.GetOrdinal("CedulaCliente")),
                                IdEmpresa = reader.IsDBNull(reader.GetOrdinal("IdEmpresa")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEmpresa")),
                                IdUsuario = reader.IsDBNull(reader.GetOrdinal("IdUsuario")) ? null : reader.GetString(reader.GetOrdinal("IdUsuario"))
                            });
                        }
                    }
                }
            }

            return laListaDeClientes;
        }

        public int ContarTotal(string busqueda = null, string filtroUsuario = null)
        {
            int total = 0;
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Limpiar y normalizar la búsqueda
                string busquedaNormalizada = null;
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    busquedaNormalizada = busqueda.Trim();
                }
                
                string query = "SELECT COUNT(*) FROM Cliente c WHERE 1=1";
                
                // Agregar filtro de búsqueda
                if (!string.IsNullOrEmpty(busquedaNormalizada))
                {
                    query += @"
                        AND (
                            (c.NombreCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.NombreCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.PrimerApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.PrimerApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.SegundoApellidoCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.SegundoApellidoCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.EmailCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.EmailCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.CedulaCliente IS NOT NULL AND LOWER(LTRIM(RTRIM(c.CedulaCliente))) LIKE LOWER(@Busqueda)) OR
                            (c.TelefonoCliente IS NOT NULL AND CAST(c.TelefonoCliente AS VARCHAR(15)) LIKE @Busqueda)
                        )";
                }
                
                // Agregar filtro de usuario
                if (!string.IsNullOrEmpty(filtroUsuario))
                {
                    if (filtroUsuario == "conUsuario")
                    {
                        query += " AND c.IdUsuario IS NOT NULL";
                    }
                    else if (filtroUsuario == "sinUsuario")
                    {
                        query += " AND c.IdUsuario IS NULL";
                    }
                }
                
                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(busquedaNormalizada))
                    {
                        command.Parameters.AddWithValue("@Busqueda", $"%{busquedaNormalizada}%");
                    }
                    
                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        total = Convert.ToInt32(result);
                    }
                }
            }

            return total;
        }
    }
}

