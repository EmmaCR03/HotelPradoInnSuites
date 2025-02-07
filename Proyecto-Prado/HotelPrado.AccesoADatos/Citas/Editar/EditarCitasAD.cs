using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Citas.Editar
{
    public class EditarCitasAD : IEditarCitasAD
    {
        Contexto _contexto;

        public EditarCitasAD()
        {
            _contexto = new Contexto();
        }
        public async Task<int> Editar(CitasTabla lasCitasAActualizar)
        {
            CitasTabla laCitaEnBaseDeDatos = _contexto.CitasTabla
               .Where(laCita => laCita.IdCita == lasCitasAActualizar.IdCita)
               .FirstOrDefault();
            laCitaEnBaseDeDatos.FechaHoraInicio = lasCitasAActualizar.FechaHoraInicio;
            laCitaEnBaseDeDatos.FechaHoraFin = lasCitasAActualizar.FechaHoraFin;
            laCitaEnBaseDeDatos.IdColaborador = lasCitasAActualizar.IdColaborador;
            EntityState estado = _contexto.Entry(laCitaEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
            int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAlmacenados;
        }
    }
}
