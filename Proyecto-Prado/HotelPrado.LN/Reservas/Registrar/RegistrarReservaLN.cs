using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Reservas.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Reservas.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Reservas;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Reservas;
using HotelPrado.AccesoADatos.Reservas.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Reservas.Registrar
{
    public class RegistrarReservaLN : IRegistrarReservaLN
    {
        private readonly IRegistrarReservaAD _registrarReservaAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarReservaLN()
        {
            _registrarReservaAD = new RegistrarReservaAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(ReservasDTO modelo)
        {
            try
            {
                // Guardar el departamento en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarReservaAD.Guardar(ConvertirObjetoReservasTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdReserva"": {modelo.IdReserva},
                    ""IdCliente"": {modelo.IdCliente},
                    ""cantidadPersonas"": ""{modelo.cantidadPersonas}"",
                    ""NombreCliente"": ""{modelo.NombreCliente}"",
                    ""IdHabitacion"": {modelo.IdHabitacion},
                    ""FechaInicio"": {modelo.FechaInicio},
                    ""FechaFinal"": ""{modelo.FechaFinal}"",
                    ""EstadoReserva"": {modelo.EstadoReserva},
                    ""MontoTotal"": {modelo.MontoTotal},
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva reserva en la base de datos.",
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
                    TablaDeEvento = "ModuloReservas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar la Reservas.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private ReservasTabla ConvertirObjetoReservasTabla(ReservasDTO laReserva)
        {
            return new ReservasTabla
            {
                IdReserva = laReserva.IdReserva,
                IdCliente = laReserva.IdCliente,
                cantidadPersonas = laReserva.cantidadPersonas,
                NombreCliente = laReserva.NombreCliente,
                IdHabitacion = laReserva.IdHabitacion,
                FechaInicio = laReserva.FechaInicio,
                FechaFinal = laReserva.FechaFinal,
                EstadoReserva = laReserva.EstadoReserva,
                MontoTotal = laReserva.MontoTotal
            };
        }
    }
}
