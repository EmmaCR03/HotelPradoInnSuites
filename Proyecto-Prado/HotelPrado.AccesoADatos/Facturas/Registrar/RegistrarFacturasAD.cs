using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Facturas;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Facturas.Registrar
{
    public class RegistrarFacturasAD : IRegistrarFacturasAD
    {
        Contexto _contexto;

        public RegistrarFacturasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(FacturasTabla laFacturaAGuardar)
        {
            try
            {
                // Generar nuevo GUID si no existe
                if (laFacturaAGuardar.Id == Guid.Empty)
                {
                    laFacturaAGuardar.Id = Guid.NewGuid();
                }

                // Establecer fechas si no están establecidas
                if (!laFacturaAGuardar.FechaCreacion.HasValue)
                {
                    laFacturaAGuardar.FechaCreacion = DateTime.Now;
                }
                if (!laFacturaAGuardar.FechaModificacion.HasValue)
                {
                    laFacturaAGuardar.FechaModificacion = DateTime.Now;
                }

                _contexto.FacturasTabla.Add(laFacturaAGuardar);
                EntityState estado = _contexto.Entry(laFacturaAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la Factura: " + ex.Message);
                return 0;
            }
        }
    }
}

