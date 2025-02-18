using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.ObtenerPorId;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.AccesoADatos.Habitacion.Editar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.Habitaciones.Conversion;
using HotelPrado.LN.Habitaciones.ObtenerPorId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.Editar
{
    public class EditarHabitacionesLN : IEditarHabitacionesLN
    {
        private readonly ConvertirHabitacionesDTOAHabitacionesTabla _convertir;
        private readonly IEditarHabitacionesAD _editarHabitaciones;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerHabitacionesPorIdLN _obtenerHabitacionesPorIdLN;

        public EditarHabitacionesLN()
        {
            _convertir = new ConvertirHabitacionesDTOAHabitacionesTabla();
            _editarHabitaciones = new EditarHabitacionesAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerHabitacionesPorIdLN = new ObtenerHabitacionesPorIdLN();
        }

        public async Task<int> Actualizar(HabitacionesDTO laHabitacionEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = _obtenerHabitacionesPorIdLN.Obtener(laHabitacionEnVista.IdHabitacion);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
                {{
                    ""IdHabitacion"": {datosAnteriores.IdHabitacion},
                    ""NumeroHabitacion"": {datosAnteriores.NumeroHabitacion},
                    ""PrecioPorNoche1P"": ""{datosAnteriores.PrecioPorNoche1P}"",
                    ""PrecioPorNoche2P"": ""{datosAnteriores.PrecioPorNoche2P}"",
                    ""PrecioPorNoche3P"": ""{datosAnteriores.PrecioPorNoche3P}"",
                    ""PrecioPorNoche4P"": ""{datosAnteriores.PrecioPorNoche4P}"",
                    ""IdTipoHabitacion"": ""{datosAnteriores.IdTipoHabitacion}"",
                    ""Capacidad"": ""{datosAnteriores.Capacidad}"",
                    ""Estado"": ""{datosAnteriores.Estado}""
                }}";

                // Realizar la actualización
                int cantidadDeDatosActualizados = await _editarHabitaciones.Editar(_convertir.Convertir(laHabitacionEnVista));

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
                {{
                    ""IdHabitacion"": {laHabitacionEnVista.IdHabitacion},
                    ""NumeroHabitacion"": {laHabitacionEnVista.NumeroHabitacion},
                    ""PrecioPorNoche1P"": ""{laHabitacionEnVista.PrecioPorNoche1P}"",
                    ""PrecioPorNoche2P"": ""{laHabitacionEnVista.PrecioPorNoche2P}"",
                    ""PrecioPorNoche3P"": ""{laHabitacionEnVista.PrecioPorNoche3P}"",
                    ""PrecioPorNoche4P"": ""{laHabitacionEnVista.PrecioPorNoche4P}"",
                    ""IdTipoHabitacion"": {laHabitacionEnVista.IdTipoHabitacion},
                    ""Capacidad"": {laHabitacionEnVista.Capacidad},
                    ""Estado"": ""{laHabitacionEnVista.Estado}""
                }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloHabitaciones",
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
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloHabitaciones",
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
