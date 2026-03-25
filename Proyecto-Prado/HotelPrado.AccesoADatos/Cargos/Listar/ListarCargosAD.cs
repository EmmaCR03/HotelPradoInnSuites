using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Listar;
using HotelPrado.Abstracciones.Modelos.Cargos;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HotelPrado.AccesoADatos.Cargos.Listar
{
    public class ListarCargosAD : IListarCargosAD
    {
        Contexto _contexto;

        public ListarCargosAD()
        {
            _contexto = new Contexto();
        }

        public List<CargosDTO> Listar()
        {
            var laListaDeCargos = new List<CargosDTO>();
            
            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();
                
                // Consulta optimizada: Solo traer los últimos 1000 registros y campos esenciales
                string query = @"
                    SELECT TOP 1000
                        c.IdCargo,
                        c.IdCheckIn,
                        c.CodigoExtra,
                        c.DescripcionExtra,
                        c.MontoTotal,
                        c.FechaHora,
                        c.Cancelado,
                        c.Facturar,
                        ISNULL(ci.NombreCliente, '') AS CheckInNombre,
                        ISNULL(cl.NombreCliente, '') AS ClienteNombre
                    FROM Cargos c
                    LEFT JOIN CheckIn ci ON c.IdCheckIn = ci.IdCheckIn
                    LEFT JOIN Cliente cl ON ci.IdCliente = cl.IdCliente
                    ORDER BY c.FechaHora DESC, c.IdCargo DESC";
                
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laListaDeCargos.Add(new CargosDTO
                            {
                                IdCargo = reader.GetInt32(reader.GetOrdinal("IdCargo")),
                                IdCheckIn = reader.IsDBNull(reader.GetOrdinal("IdCheckIn")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdCheckIn")),
                                CodigoExtra = reader.IsDBNull(reader.GetOrdinal("CodigoExtra")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CodigoExtra")),
                                DescripcionExtra = reader.IsDBNull(reader.GetOrdinal("DescripcionExtra")) ? null : reader.GetString(reader.GetOrdinal("DescripcionExtra")),
                                MontoTotal = reader.IsDBNull(reader.GetOrdinal("MontoTotal")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                                FechaHora = reader.IsDBNull(reader.GetOrdinal("FechaHora")) ? (System.DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaHora")),
                                Cancelado = reader.IsDBNull(reader.GetOrdinal("Cancelado")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("Cancelado")),
                                Facturar = reader.IsDBNull(reader.GetOrdinal("Facturar")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("Facturar")),
                                CheckInNombre = reader.IsDBNull(reader.GetOrdinal("CheckInNombre")) ? null : reader.GetString(reader.GetOrdinal("CheckInNombre")),
                                ClienteNombre = reader.IsDBNull(reader.GetOrdinal("ClienteNombre")) ? null : reader.GetString(reader.GetOrdinal("ClienteNombre"))
                            });
                        }
                    }
                }
            }

            return laListaDeCargos;
        }
    }
}

