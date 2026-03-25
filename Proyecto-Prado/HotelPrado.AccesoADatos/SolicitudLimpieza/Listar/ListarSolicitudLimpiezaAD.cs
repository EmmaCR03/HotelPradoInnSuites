using HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Listar;
using HotelPrado.Abstracciones.Modelos.SolicitudLimpieza;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace HotelPrado.AccesoADatos.SolicitudLimpieza.Listar
{
    public class ListarSolicitudLimpiezaAD : IListarSolicitudLimpiezaAD
    {
        Contexto _contexto;

        public ListarSolicitudLimpiezaAD()
        {
            _contexto = new Contexto();
        }

        public List<SolicitudLimpiezaDTO> Listar()
        {
            var laListaDeSolicitudesLimpieza = new List<SolicitudLimpiezaDTO>();

            using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        s.IdSolicitudLimpieza,
                        s.Descripcion,
                        s.Estado,
                        s.idDepartamento,
                        s.DepartamentoNombre,
                        s.idHabitacion,
                        s.FechaSolicitud,
                        CASE 
                            WHEN h.IdHabitacion IS NOT NULL THEN 'Habitación ' + ISNULL(h.NumeroHabitacion, CAST(h.IdHabitacion AS VARCHAR))
                            ELSE NULL
                        END AS HabitacionNombre
                    FROM SolicitudesLimpieza s
                    LEFT JOIN Habitaciones h ON s.idHabitacion = h.IdHabitacion
                    ORDER BY s.FechaSolicitud DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laListaDeSolicitudesLimpieza.Add(new SolicitudLimpiezaDTO
                            {
                                IdSolicitudLimpieza = reader.GetInt32(reader.GetOrdinal("IdSolicitudLimpieza")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? null : reader.GetString(reader.GetOrdinal("Estado")),
                                idDepartamento = reader.IsDBNull(reader.GetOrdinal("idDepartamento")) ? 0 : reader.GetInt32(reader.GetOrdinal("idDepartamento")),
                                DepartamentoNombre = reader.IsDBNull(reader.GetOrdinal("DepartamentoNombre")) ? null : reader.GetString(reader.GetOrdinal("DepartamentoNombre")),
                                idHabitacion = reader.IsDBNull(reader.GetOrdinal("idHabitacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("idHabitacion")),
                                HabitacionNombre = reader.IsDBNull(reader.GetOrdinal("HabitacionNombre")) ? null : reader.GetString(reader.GetOrdinal("HabitacionNombre")),
                                FechaSolicitud = reader.IsDBNull(reader.GetOrdinal("FechaSolicitud")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaSolicitud"))
                            });
                        }
                    }
                }
            }

            return laListaDeSolicitudesLimpieza;
        }
    }
}

