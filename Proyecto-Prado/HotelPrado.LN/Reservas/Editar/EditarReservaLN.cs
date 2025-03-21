using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Convertir;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.AccesoADatos.Reservas.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Reservas.Conversion;
using HotelPrado.LN.Reservas.ObtenerPorId;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Reservas.Editar
{
    public class EditarReservaLN : IEditarReservaLN
    {
        private readonly IConvertirReservasDTOAReservasTabla _convertir;
        private readonly IEditarReservaAD _editarReserva;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerReservaPorIdLN _obtenerReservaPorIdLN;

        public EditarReservaLN()
        {
            _convertir = new ConvertirReservasDTOAReservasTabla();
            _editarReserva = new EditarReservaAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerReservaPorIdLN = new ObtenerReservaPorIdLN();

        }

        public async Task<int> Actualizar(ReservasDTO laReservaEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = await _obtenerReservaPorIdLN.Obtener(laReservaEnVista.IdReserva);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
            {{
                ""cantidadPersonas"": {datosAnteriores.cantidadPersonas},
                ""NombreCliente"": ""{datosAnteriores.NombreCliente}"",
                ""FechaInicio"": ""{datosAnteriores.FechaInicio}"",
                ""FechaFinal"": ""{datosAnteriores.FechaFinal}"",
                ""EstadoReserva"": ""{datosAnteriores.EstadoReserva}"",
                ""MontoTotal"": ""{datosAnteriores.MontoTotal}""
            }}";

                // Convertir el DTO a la entidad para su actualización
                var reserva = _convertir.Convertir(laReservaEnVista);

                // Realizar la actualización con el método 'Update'
                int cantidadDeDatosActualizados = await _editarReserva.Editar(reserva);

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
            {{
                 ""cantidadPersonas"": {datosAnteriores.cantidadPersonas},
                ""NombreCliente"": ""{datosAnteriores.NombreCliente}"",
                ""FechaInicio"": ""{datosAnteriores.FechaInicio}"",
                ""FechaFinal"": ""{datosAnteriores.FechaFinal}"",
                ""EstadoReserva"": ""{datosAnteriores.EstadoReserva}"",
                ""MontoTotal"": ""{datosAnteriores.MontoTotal}""
            }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloReservas",
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
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloReservas",
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


