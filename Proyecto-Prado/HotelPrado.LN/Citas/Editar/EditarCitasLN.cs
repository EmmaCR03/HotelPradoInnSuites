using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Citas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Convertir;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Citas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Citas;
using HotelPrado.AccesoADatos.Citas.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Citas.Conversion;
using HotelPrado.LN.Citas.ObtenerPorId;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HotelPrado.LN.Citas.Editar
{
    public class EditarCitasLN : IEditarCitasLN
    {
        private readonly IConvertirCitasDTOACitasTabla _convertir;
        private readonly IEditarCitasAD _editarCita;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerCitaPorIdLN _obtenerCitasPorIdLN;

        public EditarCitasLN()
        {
            _convertir = new ConvertirCitasDTOACitasTabla();
            _editarCita = new EditarCitasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerCitasPorIdLN = new ObtenerCitaPorIdLN();
        }

        public async Task<int> Actualizar(CitasDTO laCitaEnVista)
        {
            try
            {
                // Obtener los datos anteriores de la cita
                var datosAnteriores = _obtenerCitasPorIdLN.Obtener(laCitaEnVista.IdCita);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
                {{
                   ""IdCita"": 
                {datosAnteriores.IdCita},
                    ""IdDepartamento"": {datosAnteriores.IdDepartamento},
                    ""IdColaborador"": {datosAnteriores.IdColaborador},
                    ""FechaHoraInicio"": ""{datosAnteriores.FechaHoraInicio}"",
                    ""FechaHoraFin"": ""{datosAnteriores.FechaHoraFin}"",
                    ""Estado"": {datosAnteriores.Estado},
                    ""Observaciones"": {datosAnteriores.Observaciones},
                    ""FechaCreacion"": ""{datosAnteriores.FechaCreacion}"",
                     ""Nombre"": ""{datosAnteriores.Nombre}"",
                    ""PrimerApellido"": {datosAnteriores.PrimerApellido},
                    ""SegundoApellido"": {datosAnteriores.SegundoApellido},
                    ""Telefono"": ""{datosAnteriores.Telefono}"",
                    ""MedioContacto"": {datosAnteriores.MedioContacto},
                    ""Correo"": ""{datosAnteriores.Correo}""
                }}";

                // Realizar la actualización de la cita
                int cantidadDeDatosActualizados = await _editarCita.Editar(_convertir.Convertir(laCitaEnVista));

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
                {{
                    ""IdCita"": {laCitaEnVista.IdCita},
                    ""IdDepartamento"": {laCitaEnVista.IdDepartamento},
                    ""IdColaborador"": {laCitaEnVista.IdColaborador},
                    ""FechaHoraInicio"": ""{laCitaEnVista.FechaHoraInicio}"",
                    ""FechaHoraFin"": ""{laCitaEnVista.FechaHoraFin}"",
                    ""Estado"": ""{laCitaEnVista.Estado}"",
                    ""Observaciones"": ""{laCitaEnVista.Observaciones}"",
                    ""FechaCreacion"": ""{laCitaEnVista.FechaCreacion}"",
                    ""Nombre"": ""{laCitaEnVista.Nombre}"",
                    ""PrimerApellido"": ""{laCitaEnVista.PrimerApellido}"",
                    ""SegundoApellido"": ""{laCitaEnVista.SegundoApellido}"",
                    ""Telefono"": ""{laCitaEnVista.Telefono}"",
                    ""Correo"": ""{laCitaEnVista.Correo}"",
                    ""MedioContacto"": ""{laCitaEnVista.MedioContacto}"",

                }}";

                // Registrar el evento en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloCitas",
                    StackTrace = "No hubo error",
                    DatosAnteriores = datosAnterioresJson,
                    DatosPosteriores = datosPosterioresJson
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                // Manejo de errores: registrar el evento en la bitácora
                var bitacoraError = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCitas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloCitas",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "N/A",
                    DatosPosteriores = "N/A"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacoraError);

                throw; // Propagar el error después de registrarlo
            }
        }
    }
}
