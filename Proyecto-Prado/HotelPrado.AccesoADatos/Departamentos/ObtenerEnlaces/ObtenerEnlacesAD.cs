using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerEnlaces;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Departamentos.ObtenerEnlaces
{
        public class ObtenerEnlacesAD : IObtenerEnlacesAD
        {
            private readonly Contexto _contexto;

            public ObtenerEnlacesAD()
            {
                _contexto = new Contexto();
            }

            public List<DepartamentoTabla> ObtenerDepartamentos()
            {
                return _contexto.DepartamentoTabla.ToList();
            }
        }
    }



