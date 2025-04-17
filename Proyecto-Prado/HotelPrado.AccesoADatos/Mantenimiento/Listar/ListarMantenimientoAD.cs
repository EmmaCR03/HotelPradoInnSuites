using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using System.Collections.Generic;
using System.Linq;

namespace HotelPrado.AccesoADatos.Mantenimiento.Listar
{
    public class ListarMantenimientoAD : IListarMantenimientoAD
    {
        Contexto _contexto;

        public ListarMantenimientoAD()
        {
            _contexto = new Contexto();
        }

        public List<MantenimientoDTO> Listar()
        {
            var laListaDeMantenimientos = (from elMantenimiento in _contexto.MantenimientoTabla
                                          select new MantenimientoDTO
                                          {
                                              IdMantenimiento = elMantenimiento.IdMantenimiento,
                                              Descripcion = elMantenimiento.Descripcion,
                                              Estado = elMantenimiento.Estado,
                                              idDepartamento = elMantenimiento.idDepartamento,
                                              DepartamentoNombre = elMantenimiento.DepartamentoNombre,
                                          }).ToList();


            return laListaDeMantenimientos;
        }
    }
}