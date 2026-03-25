using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Temporadas;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Temporadas.Registrar
{
    public class RegistrarTemporadasAD : IRegistrarTemporadasAD
    {
        Contexto _contexto;

        public RegistrarTemporadasAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(TemporadasTabla laTemporadaAGuardar)
        {
            try
            {
                _contexto.TemporadasTabla.Add(laTemporadaAGuardar);
                EntityState estado = _contexto.Entry(laTemporadaAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la Temporada: " + ex.Message);
                return 0;
            }
        }
    }
}

