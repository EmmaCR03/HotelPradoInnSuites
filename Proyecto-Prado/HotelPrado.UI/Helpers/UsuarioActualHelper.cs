using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using HotelPrado.UI.Models;

namespace HotelPrado.UI.Helpers
{
    /// <summary>
    /// Obtiene el Id del usuario actual (AspNetUsers). En hosting la cookie a veces no se lee;
    /// usa Identity, Session, claims y por último búsqueda por email. Si se obtiene por email,
    /// se guarda en Session para la siguiente petición.
    /// </summary>
    public static class UsuarioActualHelper
    {
        public static string ObtenerId(HttpContextBase httpContext)
        {
            if (httpContext?.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                return null;

            // 1) Identity (cookie)
            var id = httpContext.User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(id))
                return id;

            // 2) Session (se llena en Login; en hosting suele ser más fiable que la cookie)
            var sessionId = httpContext.Session?["UserId"] as string;
            if (!string.IsNullOrEmpty(sessionId))
                return sessionId;

            // 3) Claim NameIdentifier (a veces el Id viene en el claim con otro tipo)
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var nameId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (!string.IsNullOrEmpty(nameId))
                    return nameId;
            }

            // 4) Buscar por email en AspNetUsers y guardar en Session para la próxima
            var email = (httpContext.User.Identity.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(email))
                return null;

            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var user = ctx.Users.AsNoTracking()
                        .FirstOrDefault(u => u.Email != null && u.Email.Trim().ToLower() == email.ToLower());
                    if (user != null)
                    {
                        if (httpContext.Session != null)
                            httpContext.Session["UserId"] = user.Id;
                        return user.Id;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
