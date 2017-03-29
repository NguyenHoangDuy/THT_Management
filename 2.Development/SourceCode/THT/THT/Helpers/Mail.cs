using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace THT.Helpers
{
    public class Mail
    {
        public static void SendMail(List<string> emailsTo, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("materialdictionary@gmail.com");
                foreach (string emailTo in emailsTo)
                    mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 25))
                {
                    smtp.Credentials = new NetworkCredential("materialdictionary@gmail.com", "@Admin123");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}