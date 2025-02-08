using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Registrar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Departamentos.Registrar
{
    public class RegistrarDepartamentoAD : IRegistrarDepartamentoAD
    {
        Contexto _contexto;

        public RegistrarDepartamentoAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(DepartamentoTabla elDepartamentoAGuardar)
        {
            try
            {

                _contexto.DepartamentoTabla.Add(elDepartamentoAGuardar);
                EntityState estado = _contexto.Entry(elDepartamentoAGuardar).State = System.Data.Entity.EntityState.Added;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar el departamento:  " + ex.Message);
                return 0;
            }
        }

    }
}
