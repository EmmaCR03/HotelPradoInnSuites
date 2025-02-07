using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.Registrar
{
    public class RegistrarColaboradorAD : IRegistrarColaboradorAD
    {
        Contexto _contexto;

        public RegistrarColaboradorAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(ColaboradorTabla elColaboradorAGuardar)
        {
            try
            {

                _contexto.ColaboradorTabla.Add(elColaboradorAGuardar);
                EntityState estado = _contexto.Entry(elColaboradorAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar el Colaborador:  " + ex.Message);
                return 0;
            }
        }

    }
}
