using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace THT.Helpers
{
    public class SendMail
    {
        public static async Task<string> SendEmail(string mailTos, string mailCcs, string subject, string MailType, string UserName, string Password, string fullname)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                string html = "";
                if (MailType == "CreateUser")
                {
                    html += "<p><strong>Xin ch&agrave;o " + fullname + ",</strong></p>";
                    html += "<p>T&agrave;i khoản đăng nhập v&agrave;o hệ thống Logistics của bạn l&agrave;:<br />";
                    html += "T&agrave;i khoản: " + UserName + "<br />";
                    html += "Mật khẩu: " + Password + "<br />";
                    html += "<p>Hệ thống sẽ y&ecirc;u cầu bạn đổi mật khẩu sau khi đăng nhập th&agrave;nh c&ocirc;ng ở lần đầu ti&ecirc;n</p>";
                    html += "<p><strong>Tr&acirc;n trọng,</strong><br />";
                  
                }
                else if (MailType == "ResetPassword")
                {
                    html += "<p><strong>Xin ch&agrave;o " + fullname + ",</strong></p>";
                    html += "<p>Mật khẩu của bạn vừa được cập nhật. Th&ocirc;ng tin đăng nhập của bạn v&agrave;o hệ thống Logistics l&agrave;:<br />";
                    html += "T&agrave;i khoản: " + UserName + "<br />";
                    html += "Mật khẩu: " + Password + "<br />";
                    html += "<p>Hệ thống sẽ y&ecirc;u cầu bạn đổi mật khẩu sau khi đăng nhập th&agrave;nh c&ocirc;ng ở lần đầu ti&ecirc;n</p>";
                    html += "<p><strong>Tr&acirc;n trọng,</strong><br />";

                }
               
                //var listmails = mailTos.Split(';').ToList();
                //if (listmails.Count() > 0)
                //{
                //    foreach (var item in listmails)
                //    {
                //        if (IsValidEmail(item))
                //        {
                //            mail.To.Add(item);
                //        }
                //    }
                //}

                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.From = new MailAddress("materialdictionary@gmail.com");
                mail.To.Add(mailTos);
                mail.Body = HttpUtility.HtmlDecode(html);
                mail.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential("materialdictionary@gmail.com", "@Admin123");
                smtp.Send(mail);
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        static bool invalid = false;

        public static bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
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

        public static string convertPhone(string phone)
        {
            string result = "84" + phone.Substring(1, phone.Length - 1);
            return result;
        }

    }
}