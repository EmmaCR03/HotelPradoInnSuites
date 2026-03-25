using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HotelPrado.UI.Services
{
    /// <summary>
    /// Servicio para enviar correos electrónicos
    /// Configuración desde Web.config
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            // Leer configuración desde Web.config
            _smtpServer = WebConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _smtpUsername = WebConfigurationManager.AppSettings["SmtpUsername"] ?? "";
            _smtpPassword = WebConfigurationManager.AppSettings["SmtpPassword"] ?? "";
            _enableSsl = bool.Parse(WebConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
            _fromEmail = WebConfigurationManager.AppSettings["EmailFrom"] ?? "info@pradoinn.com";
            _fromName = WebConfigurationManager.AppSettings["EmailFromName"] ?? "Hotel Prado Inn & Suites";
        }

        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true, string destinatarioCC = null, string destinatarioBCC = null)
        {
            try
            {
                using (var mensaje = new MailMessage())
                {
                    mensaje.From = new MailAddress(_fromEmail, _fromName);
                    mensaje.To.Add(destinatario);
                    mensaje.Subject = asunto;
                    mensaje.Body = cuerpo;
                    mensaje.IsBodyHtml = esHtml;

                    if (!string.IsNullOrEmpty(destinatarioCC))
                    {
                        mensaje.CC.Add(destinatarioCC);
                    }

                    if (!string.IsNullOrEmpty(destinatarioBCC))
                    {
                        mensaje.Bcc.Add(destinatarioBCC);
                    }

                    using (var cliente = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        cliente.EnableSsl = _enableSsl;
                        cliente.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        cliente.DeliveryMethod = SmtpDeliveryMethod.Network;

                        await cliente.SendMailAsync(mensaje);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error (puedes usar tu sistema de bitácora aquí)
                System.Diagnostics.Debug.WriteLine($"Error al enviar email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Envía confirmación de reserva
        /// </summary>
        public async Task<bool> EnviarConfirmacionReservaAsync(string emailCliente, string nombreCliente, string numeroHabitacion, DateTime fechaInicio, DateTime fechaFin, decimal montoTotal, int numeroReserva)
        {
            var asunto = $"Confirmación de Reserva #{numeroReserva} - Hotel Prado Inn & Suites";
            var cuerpo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #6D7BB8 0%, #8B9DC3 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #6D7BB8; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
        .btn {{ display: inline-block; padding: 12px 30px; background: #6D7BB8; color: white; text-decoration: none; border-radius: 6px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Reserva Confirmada!</h1>
            <p>Hotel Prado Inn & Suites</p>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{nombreCliente}</strong>,</p>
            <p>Su reserva ha sido confirmada exitosamente. A continuación los detalles:</p>
            
            <div class='info-box'>
                <h3>Detalles de la Reserva</h3>
                <p><strong>Número de Reserva:</strong> #{numeroReserva}</p>
                <p><strong>Habitación:</strong> {numeroHabitacion}</p>
                <p><strong>Fecha de Check-in:</strong> {fechaInicio:dd/MM/yyyy}</p>
                <p><strong>Fecha de Check-out:</strong> {fechaFin:dd/MM/yyyy}</p>
                <p><strong>Monto Total:</strong> ₡{montoTotal:N2}</p>
            </div>
            
            <p>Le esperamos y esperamos que disfrute su estadía con nosotros.</p>
            
            <div class='footer'>
                <p>Hotel Prado Inn & Suites</p>
                <p>Torre Mercedes, 300m Norte, 100m Oeste, 100m Norte, San José</p>
                <p>Tel: +506 2256-0106 | Email: info@pradoinn.com</p>
            </div>
        </div>
    </div>
</body>
</html>";

            return await EnviarEmailAsync(emailCliente, asunto, cuerpo, true);
        }

        /// <summary>
        /// Envía recordatorio de check-in
        /// </summary>
        public async Task<bool> EnviarRecordatorioCheckInAsync(string emailCliente, string nombreCliente, DateTime fechaCheckIn, string numeroHabitacion)
        {
            var asunto = $"Recordatorio: Check-in mañana - Hotel Prado Inn & Suites";
            var cuerpo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #6D7BB8 0%, #8B9DC3 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #28a745; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Recordatorio de Check-in</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{nombreCliente}</strong>,</p>
            <p>Le recordamos que su check-in está programado para <strong>{fechaCheckIn:dd/MM/yyyy}</strong>.</p>
            
            <div class='info-box'>
                <p><strong>Habitación:</strong> {numeroHabitacion}</p>
                <p><strong>Fecha de Check-in:</strong> {fechaCheckIn:dd/MM/yyyy}</p>
            </div>
            
            <p>¡Le esperamos!</p>
        </div>
    </div>
</body>
</html>";

            return await EnviarEmailAsync(emailCliente, asunto, cuerpo, true);
        }

        /// <summary>
        /// Envía notificación de cambio de estado de reserva
        /// </summary>
        public async Task<bool> EnviarNotificacionEstadoReservaAsync(string emailCliente, string nombreCliente, string estado, string numeroReserva, string motivo = null)
        {
            var asunto = $"Actualización de Reserva #{numeroReserva} - Hotel Prado Inn & Suites";
            var cuerpo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #6D7BB8 0%, #8B9DC3 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info-box {{ background: white; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #6D7BB8; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Actualización de Reserva</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{nombreCliente}</strong>,</p>
            <p>Le informamos que el estado de su reserva #{numeroReserva} ha cambiado a: <strong>{estado}</strong>.</p>
            
            {(string.IsNullOrEmpty(motivo) ? "" : $"<p><strong>Motivo:</strong> {motivo}</p>")}
            
            <p>Si tiene alguna pregunta, no dude en contactarnos.</p>
        </div>
    </div>
</body>
</html>";

            return await EnviarEmailAsync(emailCliente, asunto, cuerpo, true);
        }
    }
}

