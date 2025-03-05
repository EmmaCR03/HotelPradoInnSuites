using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.General.Fecha;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Mantenimiento.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Mantenimiento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Mantenimiento;
using HotelPrado.AccesoADatos.Mantenimiento.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using HotelPrado.LN.General.Fecha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelPrado.LN.Mantenimiento.Registrar
{
    public class RegistrarMantenimientoLN : IRegistrarMantenimientoLN
    {
        IRegistrarMantenimientoAD _registrarMantenimientoAD;
        IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        IFechaActual _fecha;

        public RegistrarMantenimientoLN()
        {
            _registrarMantenimientoAD = new RegistrarMantenimientoAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _fecha = new FechaActual();
        }
        public async Task<int> Guardar(MantenimientoDTO modelo)
        {
            try
            {
                // Guardar el Mantenimiento en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarMantenimientoAD.Guardar(ConvertirObjetoMantenimientoTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdMantenimiento"": {modelo.IdMantenimiento},
                    ""Descripcion"": {modelo.Descripcion},
                    ""Estado"": {modelo.Estado},
                    
                    
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró un nuevo Mantenimiento en la base de datos.",
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
                    TablaDeEvento = "ModuloMantenimiento",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar el Mantenimiento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private MantenimientoTabla ConvertirObjetoMantenimientoTabla(MantenimientoDTO losMantenimientos)
        {
            return new MantenimientoTabla
            {
                IdMantenimiento = losMantenimientos.IdMantenimiento,
                Descripcion = losMantenimientos.Descripcion,
                Estado = losMantenimientos.Estado,
            };
        }
    }
}
