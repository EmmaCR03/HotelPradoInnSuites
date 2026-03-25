using HotelPrado.Abstracciones.Modelos.Cliente;
using System.Collections.Generic;

namespace HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Cliente.Listar
{
    public interface IListarClientesLN
    {
        List<ClienteDTO> Listar(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true);
        List<ClienteDTO> ListarPaginado(string busqueda = null, string ordenarPor = "IdCliente", bool ordenAscendente = true, int pagina = 1, int tamanoPagina = 50, string filtroUsuario = null);
        int ContarTotal(string busqueda = null, string filtroUsuario = null);
    }
}

