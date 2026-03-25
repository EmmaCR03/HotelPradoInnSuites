using HotelPrado.Abstracciones.Interfaces.AccesoADatos.SolicitudLimpieza.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.SolicitudLimpieza;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.SolicitudLimpieza.Registrar
{
    public class RegistrarSolicitudLimpiezaAD : IRegistrarSolicitudLimpiezaAD
    {
        Contexto _contexto;

        public RegistrarSolicitudLimpiezaAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(SolicitudLimpiezaTabla laSolicitudLimpiezaAGuardar)
        {
            if (laSolicitudLimpiezaAGuardar == null)
            {
                throw new ArgumentNullException(nameof(laSolicitudLimpiezaAGuardar), "La solicitud de limpieza no puede ser nula.");
            }

            // Asegurar que el estado esté establecido
            if (string.IsNullOrEmpty(laSolicitudLimpiezaAGuardar.Estado))
            {
                laSolicitudLimpiezaAGuardar.Estado = "Pendiente";
            }

            if (laSolicitudLimpiezaAGuardar.FechaSolicitud == null)
            {
                laSolicitudLimpiezaAGuardar.FechaSolicitud = DateTime.Now;
            }

            try
            {
                _contexto.SolicitudLimpiezaTabla.Add(laSolicitudLimpiezaAGuardar);
                _contexto.Entry(laSolicitudLimpiezaAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                // Capturar específicamente errores de base de datos
                string errorMessage = "Error al guardar la solicitud de limpieza: " + dbEx.Message;
                if (dbEx.InnerException != null)
                {
                    errorMessage += " | InnerException: " + dbEx.InnerException.Message;
                }
                
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + dbEx.StackTrace);
                
                // Re-lanzar con mensaje más descriptivo
                throw new Exception(errorMessage, dbEx);
            }
            catch (Exception ex)
            {
                // Log del error completo para debugging
                string errorMessage = "Error al guardar la Solicitud de Limpieza: " + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | InnerException: " + ex.InnerException.Message;
                }
                
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + ex.StackTrace);
                
                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw new Exception(errorMessage, ex);
            }
        }
    }
}

