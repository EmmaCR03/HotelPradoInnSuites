using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Listar;
using HotelPrado.Abstracciones.Modelos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Departamentos.Listar
{
    public class ListarDepartamentosAD : IListarDepartamentosAD
    {
        public List<DepartamentoDTO> Listar()
        {
            // Usar using para liberar recursos automáticamente
            using (var contexto = new Contexto())
            {
                // Optimizar consulta: solo seleccionar campos necesarios y usar AsNoTracking para mejor rendimiento
                var laListaDeDepartamento = (from elDepartamento in contexto.DepartamentoTabla.AsNoTracking()
                                             join tipoDepto in contexto.TipoDepartamentoTabla.AsNoTracking()
                                             on elDepartamento.IdTipoDepartamento equals tipoDepto.IdTipoDepartamento into tipoDeptoJoin
                                             from tipo in tipoDeptoJoin.DefaultIfEmpty()  // LEFT JOIN
                                             select new DepartamentoDTO
                                             {
                                                 IdDepartamento = elDepartamento.IdDepartamento,
                                                 Nombre = elDepartamento.Nombre,
                                                 NumeroDepartamento = elDepartamento.NumeroDepartamento,
                                                 Descripcion = elDepartamento.Descripcion,
                                                 Precio = elDepartamento.Precio,
                                                 Estado = elDepartamento.Estado,
                                                 UrlImagenes = elDepartamento.UrlImagenes,
                                                 IdTipoDepartamento = elDepartamento.IdTipoDepartamento,
                                                 NumeroHabitaciones = tipo != null ? tipo.NumeroHabitaciones : 0,  // Usar valor por defecto si es nulo
                                                 Amueblado = tipo != null ? tipo.Amueblado : false  // Usar valor por defecto si es nulo
                                             }).ToList();

                return laListaDeDepartamento;
            }
        }
    }
}

