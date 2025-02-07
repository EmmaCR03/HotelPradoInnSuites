using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Colaborador;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Colaborador.Registrar
{
    public interface IRegistrarColaboradorAD
    {
        Task<int> Guardar(ColaboradorTabla elColaboradorAGuardar);
    }
}
