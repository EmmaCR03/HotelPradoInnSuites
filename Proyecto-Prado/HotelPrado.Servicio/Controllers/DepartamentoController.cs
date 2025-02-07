using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HotelPrado.Abstracciones.Interfaces.LogicaDeNegocio.Departamentos.Listar;

using HotelPrado.LN.Departamentos.Listar;

namespace HotelPrado.Servicio.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    public class DepartamentoController : ApiController
    {
        IListarDepartamentosLN _listarDepartamenosLN;

        public DepartamentoController() 
        {
            _listarDepartamenosLN = new ListarDepartamentoLN();
        }

        // GET: api/Cliente
        [HttpGet]
        public IHttpActionResult Get()
        {
            var clientes = _listarDepartamenosLN.Listar();
            return Ok(clientes);
        }
    }
}
