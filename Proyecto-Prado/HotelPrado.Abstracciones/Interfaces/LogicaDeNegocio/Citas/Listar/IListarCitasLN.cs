using HotelPrado.Abstracciones.Modelos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Listar
{
    public interface IListarCitasLN
    {
        List<CitasDTO> Listar(int IdDepartamento);
        
        }
}
