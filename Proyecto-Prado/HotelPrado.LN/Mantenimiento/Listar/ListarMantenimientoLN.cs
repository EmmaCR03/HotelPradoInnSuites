using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Listar;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Mantenimiento.Listar
{
    public class ListarMantenimientoLN : IListarMantenimientoLN
    {
        IListarMantenimientoAD _listarMantenimientoAD;

        public ListarMantenimientoLN()
        {
            _listarMantenimientoAD = new ListarMantenimientoAD();
        }

        public List<MantenimientoDTO> Listar()
        {
            List<MantenimientoDTO> laListaDeMantenimiento = _listarMantenimientoAD.Listar();

            return laListaDeMantenimiento;


        }
    }
}

