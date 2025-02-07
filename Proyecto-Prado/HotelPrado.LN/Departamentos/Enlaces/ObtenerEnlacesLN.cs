
using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.ObtenerEnlaces;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Enlaces;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.AccesoADatos.Departamentos.ObtenerEnlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Departamentos.Enlaces
{
    public class ObtenerEnlacesLN : IObtenerEnlacesLN
    {
        IObtenerEnlacesAD _obtenerEnlaces;

        public ObtenerEnlacesLN()
        {
            _obtenerEnlaces = new ObtenerEnlacesAD();
        }

        public List<DepartamentoDTO> ObtenerCitasConEnlaces()
        {
            var departamentos = _obtenerEnlaces.ObtenerDepartamentos();

            return departamentos.Select(depto => new DepartamentoDTO
            {
                IdDepartamento = depto.IdDepartamento,
                Estado = depto.Estado,
                NumeroEmpresa = !string.IsNullOrEmpty(depto.NumeroEmpresa) && depto.NumeroEmpresa.Length == 8
                    ? $"https://wa.me/506{depto.NumeroEmpresa}?text={Uri.EscapeDataString($"Hola, estoy interesado en una cotización del departamento {depto.Nombre}")}"
                    : "https://wa.me/50685406105?text=Hola,%20estoy%20interesado%20en%20una%20cotización",
                CorreoEmpresa = !string.IsNullOrEmpty(depto.CorreoEmpresa)
                    ? $"mailto:{depto.CorreoEmpresa}?subject=Solicitud%20de%20cotización&body=Hola,%20estoy%20interesado%20en%20una%20cotización%20del%20departamento%20{Uri.EscapeDataString(depto.Nombre)}"
                    : "mailto:info@pradoinn.com?subject=Solicitud%20de%20cotización&body=Hola,%20estoy%20interesado%20en%20una%20cotización."
            }).ToList();
        }



    }
}