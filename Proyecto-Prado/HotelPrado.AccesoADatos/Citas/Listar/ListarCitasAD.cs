using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Listar;
using HotelPrado.Abstracciones.Modelos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Citas.Listar
{
    public class ListarCitasAD : IListarCitasAD
    {
        Contexto _contexto;

        public ListarCitasAD()
        {
            _contexto = new Contexto();
        }

        public List<CitasDTO> Listar(int IdDepartamento)
        {
            var laListaDeCitas = (from laCita in _contexto.CitasTabla
                                  join Colaborador in _contexto.ColaboradorTabla
                                  on laCita.IdColaborador equals Colaborador.IdColaborador into colaboradorNombreJoin
                                  join Departamento in _contexto.DepartamentoTabla
                                  on laCita.IdDepartamento equals Departamento.IdDepartamento into departamentoNombreJoin

                                  from colaborador in colaboradorNombreJoin.DefaultIfEmpty()
                                  from departamento in departamentoNombreJoin.DefaultIfEmpty()

                                  where laCita.IdDepartamento == IdDepartamento // Filtrar por departamento

                                  select new CitasDTO
                                  {
                                      IdCita = laCita.IdCita,
                                      Nombre = laCita.Nombre,
                                      PrimerApellido = laCita.PrimerApellido,
                                      SegundoApellido = laCita.SegundoApellido,
                                      Telefono = laCita.Telefono,
                                      Correo = laCita.Correo,
                                      MedioContacto = laCita.MedioContacto,
                                      IdDepartamento = laCita.IdDepartamento,
                                      IdColaborador = laCita.IdColaborador,
                                      FechaHoraInicio = laCita.FechaHoraInicio,
                                      FechaHoraFin = laCita.FechaHoraFin,
                                      Estado = laCita.Estado,
                                      Observaciones = laCita.Observaciones,
                                      FechaCreacion = laCita.FechaCreacion,
                                      NombreColaborador = colaborador != null ? colaborador.NombreColaborador : string.Empty,
                                      PrimerApellidoColaborador = colaborador != null ? colaborador.PrimerApellidoColaborador : string.Empty
                                  }).ToList();

            return laListaDeCitas;
        }


    }
}

