using HotelPrado.Abstracciones.Modelos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Registrar
{
    public interface IRegistrarCitasLN
    {
        Task<int> Guardar(CitasDTO modelo);
    }
}
