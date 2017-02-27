using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KingHostManager
{
    /// <summary>
    /// Classe para envio de e-mails.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Envia e-mail.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void Send(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Nenhum endereço de e-mail foi informado.");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Nenhum assunto foi informado.");

            using (SmtpClient client = new SmtpClient())
            {
                client.Port = 587;
                client.Host = "smtp.agenciawd7.com.br";
                client.EnableSsl = false;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("financeiro@agenciawd7.com.br", "bomber14");

                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress("financeiro@agenciawd7.com.br");

                List<string> emails = to.Split(';').ToList();

                bool first = true;

                foreach (string email in emails)
                {
                    if (first)
                    {
                        mailMessage.To.Add(email);
                        first = false;
                    }
                    else
                        mailMessage.CC.Add(email);
                }

                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mailMessage);
            }
        }
    }
}
