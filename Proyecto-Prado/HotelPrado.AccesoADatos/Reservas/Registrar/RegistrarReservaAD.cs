using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Reservas.Registrar
{
    public class RegistrarReservaAD : IRegistrarReservaAD
    {
        Contexto _contexto;

        public RegistrarReservaAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(ReservasTabla laReservaAGuardar)
        {
            try
            {
                // IMPORTANTE: IdCliente en Reservas es NVARCHAR(128) que referencia a AspNetUsers.Id (GUID)
                // NO convertir a INT, usar el GUID directamente
                if (string.IsNullOrEmpty(laReservaAGuardar.IdCliente))
                {
                    throw new Exception("IdCliente (GUID del usuario) es requerido.");
                }

                // Usar SQL directo para evitar problemas de mapeo
                using (var connection = new SqlConnection(_contexto.Database.Connection.ConnectionString))
                {
                    await connection.OpenAsync();
                    
                    string query = @"
                        INSERT INTO Reservas 
                        (IdCliente, NombreCliente, cantidadPersonas, IdHabitacion, FechaInicio, FechaFinal, EstadoReserva, MontoTotal)
                        VALUES 
                        (@IdCliente, @NombreCliente, @cantidadPersonas, @IdHabitacion, @FechaInicio, @FechaFinal, @EstadoReserva, @MontoTotal);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Usar el GUID directamente, no convertir a INT
                        command.Parameters.AddWithValue("@IdCliente", laReservaAGuardar.IdCliente);
                        command.Parameters.AddWithValue("@NombreCliente", (object)laReservaAGuardar.NombreCliente ?? DBNull.Value);
                        command.Parameters.AddWithValue("@cantidadPersonas", laReservaAGuardar.cantidadPersonas);
                        command.Parameters.AddWithValue("@IdHabitacion", laReservaAGuardar.IdHabitacion);
                        command.Parameters.AddWithValue("@FechaInicio", (object)laReservaAGuardar.FechaInicio ?? DBNull.Value);
                        command.Parameters.AddWithValue("@FechaFinal", (object)laReservaAGuardar.FechaFinal ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EstadoReserva", (object)laReservaAGuardar.EstadoReserva ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MontoTotal", laReservaAGuardar.MontoTotal);
                        
                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            return 1; // Reserva creada exitosamente
                        }
                    }
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la reserva:  " + ex.Message);
                return 0;
            }
        }

    }
}