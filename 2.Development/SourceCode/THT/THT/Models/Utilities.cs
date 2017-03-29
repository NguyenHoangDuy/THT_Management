using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using Dapper;

namespace THT.Models
{
    public class Utilities
    {
        public static void SendEmail(string UserMail, string pass,  string host, int port, string mailTos, string mailCcs, string subject, string Body)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                var listmails = mailTos.Split(';').ToList();
                var listmailscc = mailCcs.Split(';').ToList();
                if (listmails.Count() > 0)
                {
                    foreach (var item in listmails)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.To.Add(item);
                        }
                    }
                }
                if (listmailscc.Count() > 0)
                {
                    foreach (var item in listmailscc)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.CC.Add(item);
                        }
                    }
                }
                mail.Subject = subject;
                mail.From = new MailAddress(UserMail);
                mail.Body = HttpUtility.HtmlDecode(Body);
                mail.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Port = port;
                smtp.Host = host;
                smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(UserMail, pass);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }
        public static void Send_Email_With_Attachment(string UserMail, string pass, string host, int port, string mailTos, string mailCcs, string subject, string Body, string AttachmentPath)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                var listmails = mailTos.Split(';').ToList();
                var listmailscc = mailCcs.Split(';').ToList();
                if (listmails.Count() > 0)
                {
                    foreach (var item in listmails)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.To.Add(item);
                        }
                    }
                }
                if (listmailscc.Count() > 0)
                {
                    foreach (var item in listmailscc)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.CC.Add(item);
                        }
                    }
                }
                mail.Subject = subject;
                mail.From = new MailAddress(UserMail);
                mail.Body = HttpUtility.HtmlDecode(Body);
                mail.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Port = port;
                smtp.Host = host;
                smtp.EnableSsl = true;
                Attachment attach = new Attachment(AttachmentPath);
                mail.Attachments.Add(attach); 

                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(UserMail, pass);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }
        public static void Send_Email_With_BCC_Attachment(string UserMail, string pass, string host, int port, string mailTos, string SendBCC, string subject, string Body, string AttachmentPath)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                var listmails = mailTos.Split(';').ToList();
                var listmailscc = SendBCC.Split(';').ToList();
                if (listmails.Count() > 0)
                {
                    foreach (var item in listmails)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.To.Add(item);
                        }
                    }
                }
                if (listmailscc.Count() > 0)
                {
                    foreach (var item in listmailscc)
                    {
                        if (IsValidEmail(item))
                        {
                            mail.Bcc.Add(item);
                        }
                    }
                }
                mail.Subject = subject;
                mail.From = new MailAddress(UserMail);
                mail.Body = HttpUtility.HtmlDecode(Body);
                mail.IsBodyHtml = true;
                Attachment attach = new Attachment(AttachmentPath);
                mail.Attachments.Add(attach); 

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Port = port;
                smtp.Host = host;
                smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(UserMail, pass);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {

            }
        }       
        public static bool invalid = false;
        public static bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;
            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            if (invalid)
                return false;
            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

        }
        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public static void SendMailAuto(int ID, string[] para )
        {
            var dbConn = new OrmliteConnection().openConn();
            var Umail = new Utilities_Email();
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@para1", para[0]));
            param.Add(new SqlParameter("@para2", para[1]));
            param.Add(new SqlParameter("@para3", para[2]));
            param.Add(new SqlParameter("@ID",ID));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Get_TemplateMail", param);
            foreach (DataRow row in dt.Rows)
            {
                Umail.ID = int.Parse(row["ID"].ToString());
                Umail.Name = row["Name"].ToString();
                Umail.UserMail = row["UserMail"].ToString();
                Umail.PasswordMail = row["PasswordMail"].ToString();
                Umail.ListMailTos = row["ListMailTos"].ToString();
                Umail.ListMailCCs = row["ListMailCCs"].ToString();
                Umail.Subject = row["Subject"].ToString();
                Umail.HTMlBody = row["HTMlBody"].ToString();
                Umail.Port = int.Parse(row["Port"].ToString());
                Umail.Host = row["Host"].ToString();
            }
            Utilities.SendEmail(Umail.UserMail, Umail.PasswordMail, Umail.Host, Umail.Port, Umail.ListMailTos, Umail.ListMailCCs, Umail.Subject, Umail.HTMlBody);
        }
    }
    public class Utilities_Email
    {
        [AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserMail { get; set; }
        public string PasswordMail { get; set; }
        public string ListMailTos { get; set; }
        public string ListMailCCs {get; set; }
        public string Subject { get; set; }
        public string HTMlBody { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        
    }

}