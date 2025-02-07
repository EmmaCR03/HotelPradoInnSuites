using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Registrar
{
    public interface IRegistrarDepartamentoAD
    {
        Task<int> Guardar(DepartamentoTabla elDepartamentoAGuardar);
    }
}
