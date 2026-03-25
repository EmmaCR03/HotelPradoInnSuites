using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Cargos.Registrar
{
    public class RegistrarCargosAD : IRegistrarCargosAD
    {
        Contexto _contexto;

        public RegistrarCargosAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(CargosTabla elCargoAGuardar)
        {
            try
            {
                _contexto.CargosTabla.Add(elCargoAGuardar);
                EntityState estado = _contexto.Entry(elCargoAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar el Cargo: " + ex.Message);
                return 0;
            }
        }
    }
}

