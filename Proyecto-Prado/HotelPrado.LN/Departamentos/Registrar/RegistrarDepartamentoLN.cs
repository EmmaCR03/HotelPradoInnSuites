using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Departamentos.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Bitacora.Registrar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Registrar;
using HotelPrado.Abstracciones.Modelos.Bitacora;
using HotelPrado.Abstracciones.Modelos.Departamento;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Departamento;
using HotelPrado.AccesoADatos.Departamentos.Registrar;
using HotelPrado.LN.Bitacora.Registrar;
using System;
using System.Threading.Tasks;

namespace HotelPrado.LN.Departamentos.Registrar
{
    public class RegistrarDepartamentoLN : IRegistrarDepartamentoLN
    {
        private readonly IRegistrarDepartamentoAD _registrarDepartamentoAD;
        private readonly IRegistrarBitacoraEventosLN _registrarBitacoraEventosLN;

        public RegistrarDepartamentoLN()
        {
            _registrarDepartamentoAD = new RegistrarDepartamentoAD();
            _registrarBitacoraEventosLN = new RegistrarBitacoraEventosLN();
        }

        public async Task<int> Guardar(DepartamentoDTO modelo)
        {
            try
            {
                // Guardar el departamento en la base de datos
                int cantidadDeDatosAlmacenados = await _registrarDepartamentoAD.
                    Guardar(ConvertirObjetoDepartamentoTabla(modelo));

                // Preparar datos para registrar en la bitácora
                string datos = $@"
                {{
                    ""IdDepartamento"": {modelo.IdDepartamento},
                    ""IdCliente"": {modelo.IdCliente},
                    ""Nombre"": ""{modelo.Nombre}"",
                    ""Descripcion"": ""{modelo.Descripcion}"",
                    ""IdTipoDepartamento"": {modelo.IdTipoDepartamento},
                    ""Precio"": {modelo.Precio},
                    ""Estado"": ""{modelo.Estado}"",
                    ""NumeroHabitaciones"": {modelo.NumeroHabitaciones},
                    ""Amueblado"": {modelo.Amueblado.ToString().ToLower()}
                }}";

                var bitacora = new BitacoraEventosDTO
                {
                    IdEvento = 0,
                    TablaDeEvento = "ModuloDepartamentos",
                    TipoDeEvento = "Registrar",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Se registró un nuevo departamento en la base de datos.",
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
                    TablaDeEvento = "ModuloDepartamentos",
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now.ToString("dd-MM-yyyy"),
                    DescripcionDeEvento = "Error al registrar el departamento.",
                    StackTrace = ex.StackTrace,
                    DatosAnteriores = "NA",
                    DatosPosteriores = "NA"
                };

                await _registrarBitacoraEventosLN.RegistrarBitacora(bitacora);

                // Re-lanzar la excepción para que sea manejada en capas superiores
                throw;
            }
        }

        private DepartamentoTabla ConvertirObjetoDepartamentoTabla(DepartamentoDTO elDepartamento)
        {
            return new DepartamentoTabla
            {
                IdDepartamento = elDepartamento.IdDepartamento,
                IdCliente = elDepartamento.IdCliente,
                Nombre = elDepartamento.Nombre,
                NumeroDepartamento = elDepartamento.NumeroDepartamento ?? 0,
                Descripcion = elDepartamento.Descripcion,
                IdTipoDepartamento = elDepartamento.IdTipoDepartamento,
                Precio = elDepartamento.Precio,
                Estado = elDepartamento.Estado,
                UrlImagenes = elDepartamento.UrlImagenes
            };
        }
    }
}
