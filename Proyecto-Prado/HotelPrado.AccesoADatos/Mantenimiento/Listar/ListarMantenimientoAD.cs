using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Mantenimiento.Listar
{
    public class ListarMantenimientoAD : IListarMantenimientoAD
    {
        Contexto _contexto;

        public ListarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public List<MantenimientoDTO> Listar(int IdMantenimiento)
        {
            var laListaDeMantenimiento = (from elMantenimiento in _contexto.MantenimientoTabla
                                          select new MantenimientoDTO
                                          {
                                              IdMantenimiento = elMantenimiento.IdMantenimiento,
                                              Descripcion = elMantenimiento.Descripcion,
                                              Estado = elMantenimiento.Estado,
                                              idDepartamento = elMantenimiento.idDepartamento,
                                              idHabitacion = elMantenimiento.idHabitacion
                                          }).ToList();


            return laListaDeMantenimiento;
        }
    }
}
