using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cargos.Editar;
using HotelPrado.Abstracciones.ModelosDeBaseDeDatos.Cargos;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HotelPrado.AccesoADatos.Cargos.Editar
{
    public class EditarCargosAD : IEditarCargosAD
    {
        private readonly Contexto _contexto;

        public EditarCargosAD()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Editar(CargosTabla elCargoActualizar)
        {
            CargosTabla elCargoEnBaseDeDatos = _contexto.CargosTabla
                .Where(elCargo => elCargo.IdCargo == elCargoActualizar.IdCargo)
                .FirstOrDefault();

            if (elCargoEnBaseDeDatos != null)
            {
                elCargoEnBaseDeDatos.IdCheckIn = elCargoActualizar.IdCheckIn;
                elCargoEnBaseDeDatos.CodigoExtra = elCargoActualizar.CodigoExtra;
                elCargoEnBaseDeDatos.DescripcionExtra = elCargoActualizar.DescripcionExtra;
                elCargoEnBaseDeDatos.NumeroDocumento = elCargoActualizar.NumeroDocumento;
                elCargoEnBaseDeDatos.MontoCargo = elCargoActualizar.MontoCargo;
                elCargoEnBaseDeDatos.MontoServicio = elCargoActualizar.MontoServicio;
                elCargoEnBaseDeDatos.ImpuestoVenta = elCargoActualizar.ImpuestoVenta;
                elCargoEnBaseDeDatos.ImpuestoHotel = elCargoActualizar.ImpuestoHotel;
                elCargoEnBaseDeDatos.ImpuestoServicio = elCargoActualizar.ImpuestoServicio;
                elCargoEnBaseDeDatos.MontoTotal = elCargoActualizar.MontoTotal;
                elCargoEnBaseDeDatos.QuienPaga = elCargoActualizar.QuienPaga;
                elCargoEnBaseDeDatos.FechaHora = elCargoActualizar.FechaHora;
                elCargoEnBaseDeDatos.NumeroEmpleado = elCargoActualizar.NumeroEmpleado;
                elCargoEnBaseDeDatos.Cancelado = elCargoActualizar.Cancelado;
                elCargoEnBaseDeDatos.Notas = elCargoActualizar.Notas;
                elCargoEnBaseDeDatos.EnFacturaExtras = elCargoActualizar.EnFacturaExtras;
                elCargoEnBaseDeDatos.CuentaError = elCargoActualizar.CuentaError;
                elCargoEnBaseDeDatos.NumeroCierre = elCargoActualizar.NumeroCierre;
                elCargoEnBaseDeDatos.PagoImpuesto = elCargoActualizar.PagoImpuesto;
                elCargoEnBaseDeDatos.TipoCambio = elCargoActualizar.TipoCambio;
                elCargoEnBaseDeDatos.FechaTraslado = elCargoActualizar.FechaTraslado;
                elCargoEnBaseDeDatos.Facturar = elCargoActualizar.Facturar;
                elCargoEnBaseDeDatos.Secuencia = elCargoActualizar.Secuencia;
                elCargoEnBaseDeDatos.NoContable = elCargoActualizar.NoContable;
                elCargoEnBaseDeDatos.NumeroFolioOriginal = elCargoActualizar.NumeroFolioOriginal;

                EntityState estado = _contexto.Entry(elCargoEnBaseDeDatos).State = System.Data.Entity.EntityState.Modified;
                int cantidadDeDatosAlmacenados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosAlmacenados;
            }
            return 0;
        }
    }
}

