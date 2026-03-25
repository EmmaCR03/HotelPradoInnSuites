using System;
using System.Web;
using System.Web.Mvc;

namespace HotelPrado.UI.Filters
{
    /// <summary>
    /// Filtro que suprime errores relacionados con DisposableHttpContextWrapper
    /// que pueden ocurrir durante operaciones de Entity Framework
    /// </summary>
    public class SuppressHttpContextExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            // Verificar si el error está relacionado con DisposableHttpContextWrapper
            if (IsHttpContextWrapperException(filterContext.Exception))
            {
                // Suprimir el error - no interrumpir el flujo
                filterContext.ExceptionHandled = true;
                
                // Log opcional (solo en debug)
                #if DEBUG
                System.Diagnostics.Debug.WriteLine($"HttpContextWrapper exception suppressed: {filterContext.Exception.Message}");
                #endif
            }
        }

        private bool IsHttpContextWrapperException(Exception ex)
        {
            if (ex == null) return false;

            // Verificar el tipo de excepción o el mensaje
            var exceptionType = ex.GetType().FullName;
            var exceptionMessage = ex.Message ?? string.Empty;
            var stackTrace = ex.StackTrace ?? string.Empty;

            // Verificar si es el error específico de DisposableHttpContextWrapper
            if (exceptionType.Contains("DisposableHttpContextWrapper") ||
                exceptionMessage.Contains("DisposableHttpContextWrapper") ||
                exceptionMessage.Contains("SwitchContext") ||
                stackTrace.Contains("DisposableHttpContextWrapper") ||
                stackTrace.Contains("SwitchContext"))
            {
                return true;
            }

            // Verificar excepciones internas
            if (ex.InnerException != null)
            {
                return IsHttpContextWrapperException(ex.InnerException);
            }

            return false;
        }
    }
}

