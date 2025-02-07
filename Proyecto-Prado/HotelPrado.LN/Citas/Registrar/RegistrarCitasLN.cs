using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.General.Fecha;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Citas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.AccesoADatos.Citas.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.General.Fecha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.Registrar
{
    public class RegistrarCitasLN : IRegistrarCitasLN
    {
        IRegistrarCitasAD _registrarCitasAD;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IFechaActual _fecha;

        public RegistrarCitasLN() 
        {
            _registrarCitasAD = new RegistrarCitasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _fecha = new FechaActual();
        }
        public async Task<int> Guardar(CitasDTO modelo)
        {
            try
            {
                // Guardar el departamento en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarCitasAD.Guardar(ConvertirObjetoCitasTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdCita"": {modelo.IdCita},
                    ""IdDepartamento"": {modelo.IdDepartamento},
                    ""IdColaborador"": {modelo.IdColaborador},
                    ""FechaHoraInicio"": ""{modelo.FechaHoraInicio}"",
                    ""FechaHoraFin"": ""{modelo.FechaHoraFin}"",
                    ""Estado"": {modelo.Estado},
                    ""Observaciones"": {modelo.Observaciones},
                    ""FechaCreacion"": ""{modelo.FechaCreacion}"",
                     ""Nombre"": ""{modelo.Nombre}"",
                    ""PrimerApellido"": {modelo.PrimerApellido},
                    ""SegundoApellido"": {modelo.SegundoApellido},
                    ""Telefono"": ""{modelo.Telefono}"",
                    ""MedioContacto"": {modelo.MedioContacto},
                    ""Correo"": ""{modelo.Correo}""
                    
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva cita en la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = "NA",
                    DatosPosteriores = datos
                };

                // Registrar el evento en la bitácora
                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosAlmacenados;
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar el departamento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private CitasTabla ConvertirObjetoCitasTabla(CitasDTO lasCitas)
        {
            return new CitasTabla
            {
                IdCita = lasCitas.IdCita,
                Nombre = lasCitas.Nombre,
                PrimerApellido = lasCitas.PrimerApellido,
                SegundoApellido = lasCitas.SegundoApellido,
                Telefono = lasCitas.Telefono,
                Correo = lasCitas.Correo,
                MedioContacto = lasCitas.MedioContacto,
                IdDepartamento = lasCitas.IdDepartamento,
                IdColaborador = lasCitas.IdColaborador ?? null,
                FechaHoraInicio = lasCitas.FechaHoraInicio,
                FechaHoraFin = lasCitas.FechaHoraFin,
                Estado = lasCitas.Estado,
                Observaciones = lasCitas.Observaciones,
                FechaCreacion = _fecha.ObtenerFecha()
            };
        }
    }
}
