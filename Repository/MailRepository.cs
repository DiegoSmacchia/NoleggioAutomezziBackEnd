using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Repository
{
    public class MailRepository
    {
        public MailRepository() { }
        public void SendMail(string destinatario, string titolo, string testo)
        {
            try
            {
                var smtpClient = new SmtpClient("in-v3.mailjet.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("9ec6082c6c2bf41f6e49a315a94e3ddd", "c0ea3c3a31e481532002d019577971c8"),
                    EnableSsl = true,
                };

                smtpClient.Send("diegosmacchia@gmail.com", destinatario, titolo, testo);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}