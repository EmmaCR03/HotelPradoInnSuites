using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelPrado.UI.Models
{
	public class UsuarioConRoles
	{
        public string id { get; set; }

        public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string cedula { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}