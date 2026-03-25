using HotelPrado.Abstracciones.Interfaces.AccesoADatos.Cliente.Listar;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cliente.Listar;
using HotelPrado.Abstracciones.Modelos.Cliente;
using HotelPrado.AccesoADatos.Cliente.Listar;
using System.Collections.Generic;

namespace HotelPrado.LN.Cliente.Listar
{
    public class ListarClientesLN : IListarClientesLN
    {
        IListarClientesAD _listarClientesAD;

        public ListarClientesLN()
        {
            _listarClientesAD = new ListarClientesAD();
        }

        public List<ClienteDTO> Listar(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true)
        {
            List<ClienteDTO> laListaDeClientes = _listarClientesAD.Listar(busqueda, ordenarPor, ordenAscendente);
            return laListaDeClientes;
        }

        public List<ClienteDTO> ListarPaginado(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true, int pagina = 1, int tamanoPagina = 50, string filtroUsuario = null)
        {
            List<ClienteDTO> laListaDeClientes = _listarClientesAD.ListarPaginado(busqueda, ordenarPor, ordenAscendente, pagina, tamanoPagina, filtroUsuario);
            return laListaDeClientes;
        }

        public int ContarTotal(string busqueda = null, string filtroUsuario = null)
        {
            return _listarClientesAD.ContarTotal(busqueda, filtroUsuario);
        }
    }
}

