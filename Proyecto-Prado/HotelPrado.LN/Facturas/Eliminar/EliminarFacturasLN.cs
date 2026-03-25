using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Facturas.Eliminar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Facturas.Eliminar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.AccesoADatos.Facturas.Eliminar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Facturas.Eliminar
{
    public class EliminarFacturasLN : IEliminarFacturasLN
    {
        private readonly IEliminarFacturasAD _eliminarFacturasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public EliminarFacturasLN()
        {
            _eliminarFacturasAD = new EliminarFacturasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Eliminar(Guid Id)
        {
            try
            {
                int cantidadDeDatosEliminados = await _eliminarFacturasAD.Eliminar(Id);

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Eliminar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Se eliminó una Factura (Id: {Id}) de la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = $"Id: {Id}",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                return cantidadDeDatosEliminados;
            }
            catch (Exception ex)
            {
                // Registrar el error en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloFacturas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al eliminar la Factura.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);
                throw;
            }
        }
    }
}

