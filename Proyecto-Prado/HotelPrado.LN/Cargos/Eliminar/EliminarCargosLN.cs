using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Eliminar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cargos.Eliminar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.AccesoADatos.Cargos.Eliminar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Cargos.Eliminar
{
    public class EliminarCargosLN : IEliminarCargosLN
    {
        private readonly IEliminarCargosAD _eliminarCargosAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public EliminarCargosLN()
        {
            _eliminarCargosAD = new EliminarCargosAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Eliminar(int IdCargo)
        {
            try
            {
                int cantidadDeDatosEliminados = await _eliminarCargosAD.Eliminar(IdCargo);

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Eliminar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Se eliminó un Cargo (IdCargo: {IdCargo}) de la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = $"IdCargo: {IdCargo}",
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
                    TablaDeEvento = "ModuloCargos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al eliminar el Cargo.",
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

