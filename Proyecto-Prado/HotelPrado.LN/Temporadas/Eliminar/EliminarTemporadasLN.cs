using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Temporadas.Eliminar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Temporadas.Eliminar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.AccesoADatos.Temporadas.Eliminar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Temporadas.Eliminar
{
    public class EliminarTemporadasLN : IEliminarTemporadasLN
    {
        private readonly IEliminarTemporadasAD _eliminarTemporadasAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public EliminarTemporadasLN()
        {
            _eliminarTemporadasAD = new EliminarTemporadasAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Eliminar(int IdTemporada)
        {
            try
            {
                int cantidadDeDatosEliminados = await _eliminarTemporadasAD.Eliminar(IdTemporada);

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Eliminar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = $"Se eliminó una Temporada (IdTemporada: {IdTemporada}) de la base de datos.",
                    StackTrace = "Sin errores",
                    DatosAnteriores = $"IdTemporada: {IdTemporada}",
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
                    TablaDeEvento = "ModuloTemporadas",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al eliminar la Temporada.",
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

