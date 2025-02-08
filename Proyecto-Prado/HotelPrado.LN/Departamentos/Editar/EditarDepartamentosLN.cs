using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Conversion;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Editar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.AccesoADatos.Departamentos.Editar;
using HotelPrado.LN.Departamentos.Conversion;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.ObtenerPorId;
using HotelPrado.LN.Departamentos.ObtenerPorId;

namespace HotelPrado.LN.Departamentos.Editar
{
    public class EditarDepartamentosLN : IEditarDepartamentosLN
    {
        private readonly IConvertirDepartamentoDTOADepartamentoTabla _convertir;
        private readonly IEditarDepartamentoAD _editarDepartamento;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;
        private readonly IObtenerDepartamentoPorIdLN _obtenerDepartamentoPorIdLN;

        public EditarDepartamentosLN()
        {
            _convertir = new ConvertirDepartamentoDTOADepartamentoTabla();
            _editarDepartamento = new EditarDepartamentoAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
            _obtenerDepartamentoPorIdLN = new ObtenerDepartamentoPorIdLN();
        }

        public async Task<int> Actualizar(DepartamentoDTO elDepartamentoEnVista)
        {
            try
            {
                // Obtener los datos anteriores
                var datosAnteriores = _obtenerDepartamentoPorIdLN.Obtener(elDepartamentoEnVista.IdDepartamento);

                // Convertir los datos anteriores a JSON
                string datosAnterioresJson = $@"
                {{
                    ""IdDepartamento"": {datosAnteriores.IdDepartamento},
                    ""IdCliente"": {datosAnteriores.IdCliente},
                    ""Nombre"": ""{datosAnteriores.Nombre}"",
""Descripcion"": ""{datosAnteriores.Descripcion}"",
                    ""IdTipoDepartamento"": {datosAnteriores.IdTipoDepartamento},
                    ""Precio"": {datosAnteriores.Precio},
                    ""Estado"": ""{datosAnteriores.Estado}""
                }}";

                // Realizar la actualización
                int cantidadDeDatosActualizados = await _editarDepartamento.Editar(_convertir.Convertir(elDepartamentoEnVista));

                // Convertir los datos actualizados a JSON
                string datosPosterioresJson = $@"
                {{
                    ""IdDepartamento"": {elDepartamentoEnVista.IdDepartamento},
                    ""IdCliente"": {elDepartamentoEnVista.IdCliente},
                    ""Nombre"": ""{elDepartamentoEnVista.Nombre}"",
""Descripcion"": ""{datosAnteriores.Descripcion}"",

                    ""IdTipoDepartamento"": {elDepartamentoEnVista.IdTipoDepartamento},
                    ""Precio"": {elDepartamentoEnVista.Precio},
                    ""Estado"": ""{elDepartamentoEnVista.Estado}""
                }}";

                // Registrar en la bitácora
                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamentos",
                    TipoDeEvento = "Editar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se editó un registro en la tabla ModuloDepartamentos",
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
                    TablaDeEvento = "ModuloDepartamentos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al editar un registro en la tabla ModuloDepartamentos",
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
