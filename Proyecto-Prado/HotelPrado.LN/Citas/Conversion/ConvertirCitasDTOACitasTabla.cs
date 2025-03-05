using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Convertir;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.Conversion
{
    public class ConvertirCitasDTOACitasTabla : IConvertirCitasDTOACitasTabla
    {
        public CitasTabla Convertir(CitasDTO laCita)
        {
            return new CitasTabla
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
                FechaCreacion = laCita.FechaCreacion
            };
        }
    }
}
