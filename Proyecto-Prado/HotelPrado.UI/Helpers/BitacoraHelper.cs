using System;
using System.Web;
using Microsoft.AspNet.Identity;

namespace HotelPrado.UI.Helpers
{
    public static class BitacoraHelper
    {
        /// <summary>
        /// Obtiene el nombre de usuario actual del contexto HTTP
        /// </summary>
        /// <returns>Nombre de usuario o "Sistema" si no hay usuario autenticado</returns>
        public static string ObtenerUsuarioActual()
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return HttpContext.Current.User.Identity.GetUserName() ?? "Usuario Anónimo";
                }
                return "Sistema";
            }
            catch
            {
                return "Sistema";
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual
        /// </summary>
        /// <returns>ID del usuario o null si no está autenticado</returns>
        public static string ObtenerUsuarioId()
        {
            try
            {
                if (HttpContext.Current == null) return null;
                return UsuarioActualHelper.ObtenerId(new HttpContextWrapper(HttpContext.Current));
            }
            catch
            {
                return null;
            }
        }
    }
}











