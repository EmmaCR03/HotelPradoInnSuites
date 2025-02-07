using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Listar;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.AccesoADatos.Departamentos.Listar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Departamentos.Listar
{
    public class ListarDepartamentoLN : IListarDepartamentosLN
    {
        IListarDepartamentosAD _listarDepartamentosAD;

        public ListarDepartamentoLN() 
        {
            _listarDepartamentosAD = new ListarDepartamentosAD();
        }

        public List<DepartamentoDTO> Listar() 
        {
            List<DepartamentoDTO> laListaDeDepartamento = _listarDepartamentosAD.Listar();

            foreach (var depto in laListaDeDepartamento)
            {
                if (string.IsNullOrEmpty(depto.NombreCliente))
                {
                    depto.NombreCliente = "No aplica";
                }
            }

            return laListaDeDepartamento;


        }
    }
}
