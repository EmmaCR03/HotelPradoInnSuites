using HotelPrado.Abstracciones.Modelos.Colaborador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Colaborador.Editar
{
    public interface IEditarColaboradorLN
    {
        Task<int> Actualizar(ColaboradorDTO elColaboradorEnVista);
    }
}
