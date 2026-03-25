using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Listar;
using HotelPrado.Abstracciones.Modelos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Colaborador.Listar
{

    public class ListarColaboradorAD : IListarColaboradorAD
    {
        Contexto _contexto;

        public ListarColaboradorAD()
        {
            _contexto = new Contexto();
        }

        public List<ColaboradorDTO> Listar()
        {
            // Optimización: Usar AsNoTracking para solo lectura (más rápido)
            // Proyección selectiva para cargar solo los campos necesarios
            var laListaDeColaboradores = _contexto.ColaboradorTabla
                .AsNoTracking() // No rastrear cambios - mejora rendimiento
                .Select(elColaborador => new ColaboradorDTO
                {
                    IdColaborador = elColaborador.IdColaborador,
                    NombreColaborador = elColaborador.NombreColaborador,
                    PrimerApellidoColaborador = elColaborador.PrimerApellidoColaborador,
                    SegundoApellidoColaborador = elColaborador.SegundoApellidoColaborador,
                    CedulaColaborador = elColaborador.CedulaColaborador,
                    PuestoColaborador = elColaborador.PuestoColaborador,
                    EstadoLaboral = elColaborador.EstadoLaboral,
                    IngresoColaborador = elColaborador.IngresoColaborador,
                })
                .OrderBy(c => c.NombreColaborador) // Ordenar en BD, no en memoria
                .ThenBy(c => c.PrimerApellidoColaborador)
                .ToList();

            return laListaDeColaboradores;
        }
    }
}