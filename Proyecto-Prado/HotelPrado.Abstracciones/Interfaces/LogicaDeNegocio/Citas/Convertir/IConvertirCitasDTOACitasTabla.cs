using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Convertir
{
    public interface IConvertirCitasDTOACitasTabla
    {
        CitasTabla Convertir(CitasDTO laCita);
    }
}
