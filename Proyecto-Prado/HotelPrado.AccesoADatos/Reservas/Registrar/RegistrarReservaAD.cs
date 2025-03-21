using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using System;
using System.Data.Entity;
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

                _contexto.ReservasTabla.Add(laReservaAGuardar);
                EntityState estado = _contexto.Entry(laReservaAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la reserva:  " + ex.Message);
                return 0;
            }
        }

    }
}