using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.AccesoADatos.Citas.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.ObtenerPorId
{
    public class ObtenerCitaPorIdLN : IObtenerCitaPorIdLN
    {
        IObtenerCitasPorIdAD _obtenerId;

        public ObtenerCitaPorIdLN() 
        {
            _obtenerId = new ObtenerCitasPorIdAD();
        }

        public CitasDTO Obtener(int IdCita)
        {
            CitasTabla citasEnBaseDeDatos = _obtenerId.Obtener(IdCita);
            CitasDTO laCitaAMostrar = ConvertirACitaAMostrar(citasEnBaseDeDatos);
            return laCitaAMostrar;
        }
        private CitasDTO ConvertirACitaAMostrar(CitasTabla citasEnBaseDeDatos)
        {
            return new CitasDTO
            {
                IdCita = citasEnBaseDeDatos.IdCita,
                Nombre = citasEnBaseDeDatos.Nombre,
                PrimerApellido = citasEnBaseDeDatos.PrimerApellido,
                SegundoApellido = citasEnBaseDeDatos.SegundoApellido,
                Telefono = citasEnBaseDeDatos.Telefono,
                Correo = citasEnBaseDeDatos.Correo,
                MedioContacto = citasEnBaseDeDatos.MedioContacto,
                IdDepartamento = citasEnBaseDeDatos.IdDepartamento,
                IdColaborador = citasEnBaseDeDatos.IdColaborador,
                FechaHoraInicio = citasEnBaseDeDatos.FechaHoraInicio,
                FechaHoraFin = citasEnBaseDeDatos.FechaHoraFin,
                Estado = citasEnBaseDeDatos.Estado,
                Observaciones = citasEnBaseDeDatos.Observaciones,
                FechaCreacion = citasEnBaseDeDatos.FechaCreacion
            };
        }

    }
}
