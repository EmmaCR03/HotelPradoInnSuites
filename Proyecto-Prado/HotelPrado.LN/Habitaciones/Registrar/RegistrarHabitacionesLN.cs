using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Habitaciones.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Habitaciones.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Habitaciones;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Habitaciones;
using HotelPrado.AccesoADatos.Habitacion.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Habitaciones.Registrar
{
    public class RegistrarHabitacionesLN : IRegistrarHabitacionesLN
    {
        private readonly IRegistrarHabitacionesAD _registrarHabitacionesAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarHabitacionesLN()
        {
            _registrarHabitacionesAD = new RegistrarHabitacionesAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(HabitacionesDTO modelo)
        {
            try
            {
                // Guardar la Habitacion en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarHabitacionesAD.Guardar(ConvertirObjetoHabitacionesTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdHabitacion"": {modelo.IdHabitacion},
                    ""NumeroHabitacion"": {modelo.NumeroHabitacion},
                    ""PrecioPorNoche1P"": ""{modelo.PrecioPorNoche1P}"",
                    ""PrecioPorNoche2P"": ""{modelo.PrecioPorNoche2P}"",
                    ""PrecioPorNoche3P"": ""{modelo.PrecioPorNoche3P}"",
                    ""PrecioPorNoche4P"": ""{modelo.PrecioPorNoche4P}"",
                    ""IdTipoHabitacion"": {modelo.IdTipoHabitacion},
                    ""Capacidad"": {modelo.Capacidad},
                    ""Estado"": ""{modelo.Estado}"",
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró una nueva Habitacion en la base de datos.",
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
                    TablaDeEvento = "ModuloHabitaciones",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar la habitacion.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private HabitacionesTabla ConvertirObjetoHabitacionesTabla(HabitacionesDTO lahabitacion)
        {
            return new HabitacionesTabla
            {
                IdHabitacion = lahabitacion.IdHabitacion,
                NumeroHabitacion = lahabitacion.NumeroHabitacion,
                PrecioPorNoche1P = lahabitacion.PrecioPorNoche1P,
                PrecioPorNoche2P = lahabitacion.PrecioPorNoche2P,
                PrecioPorNoche3P = lahabitacion.PrecioPorNoche3P,
                PrecioPorNoche4P = lahabitacion.PrecioPorNoche4P,
                IdTipoHabitacion = (int)lahabitacion.IdTipoHabitacion,
                Capacidad = lahabitacion.Capacidad,
                Estado = lahabitacion.Estado,
            };
        }
    }
}

 