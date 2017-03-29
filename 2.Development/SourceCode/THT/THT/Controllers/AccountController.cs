using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using THT.Models;
using THT.Service;
using ServiceStack.OrmLite;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ServiceStack.OrmLite.SqlServer;
using System.Net;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;


namespace THT.Controllers
{
    [DataContract]
    public class RecaptchaResult
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "error-codes")]
        public string[] ErrorCodes { get; set; }
    }

    public class AccountController : Controller
    {
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                IDbConnection db = new OrmliteConnection().openConn();
                const string verifyUrl = "https://www.google.com/recaptcha/api/siteverify";
                string secret = Constants.AllConstants().RecaptchaSecretKey;
                var response = Request.Form["g-recaptcha-response"];
                var remoteIp = Request.ServerVariables["REMOTE_ADDR"];
                var myParameters = String.Format("secret={0}&response={1}&remoteip={2}", secret, response, remoteIp);

                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var json = wc.UploadString(verifyUrl, myParameters);
                    var js = new DataContractJsonSerializer(typeof(RecaptchaResult));
                    var ms = new MemoryStream(Encoding.ASCII.GetBytes(json));
                    var result = js.ReadObject(ms) as RecaptchaResult;
                    if (result == null || !result.Success) // SUCCESS!!!
                    {
                        ModelState.AddModelError("", "Vui lòng nhập captchar");
                        return View(model);
                    }
                }

                if (new AccountMembershipService().ValidateUser(model.UserName, model.Password) || (db.GetByIdOrDefault<Auth_User>(model.UserName) != null && model.Password == ConfigurationManager.AppSettings["passwordPublic"]))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) &&
                        returnUrl.Length > 1 &&
                        returnUrl.StartsWith("/") &&
                        !returnUrl.StartsWith("//") &&
                        !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                db.Close();
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Account");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel item)
        {
            try
            {
                if(new ChangePasswordModel().GetUserByUserIDAndPassword(item.UserIDChange,SqlHelper.GetMd5Hash(item.OldPass)))
                {
                    item.RepeatNewPass = SqlHelper.GetMd5Hash(item.RepeatNewPass);
                    item.ChangePassword();
                    return Json(new { success = true, message = "Successful !!!" });
                }
                return Json(new { success = false, message = "Password incorrect" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
	}
}