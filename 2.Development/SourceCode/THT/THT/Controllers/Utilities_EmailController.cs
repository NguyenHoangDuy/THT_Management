using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using THT.Models;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using System.Collections;
using System.Configuration;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;

namespace THT.Controllers
{
    [Authorize]
    [NoCache]
    public class Utilities_EmailController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dict["listMailTos"] = dbConn.Select<Auth_User>("SELECT Email FROM Auth_User ");
                // dict["listMerchant"] = dbConn.Select<DC_OCM_Merchant>("Select MerchantID,MerchantID +'-'+ MerchantName as MerchantName from DC_OCM_Merchant");
                dbConn.Close();
                return View("_Utilities_Email", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            IDbConnection dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<Utilities_Email>("Select * from Utilities_Email ");
            return Json(data.ToDataSourceResult(request),JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(Utilities_Email item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.ID.ToString()))
                {
                    var isExist = db.SingleOrDefault<Utilities_Email>("ID={0}", item.ID);
                    item.Name = !string.IsNullOrEmpty(item.Name) ? item.Name : "";
                    item.UserMail = !string.IsNullOrEmpty(item.UserMail) ? item.UserMail : "";
                    item.PasswordMail = !string.IsNullOrEmpty(item.PasswordMail) ? item.PasswordMail : "";
                    item.Host = !string.IsNullOrEmpty(item.Host) ? item.Host : "";
                    item.Port = item.Port;
                    item.ListMailCCs = !string.IsNullOrEmpty(item.ListMailCCs)? item.ListMailCCs: "";
                    item.ListMailTos = !string.IsNullOrEmpty(item.ListMailTos) ? item.ListMailTos : "";
                    item.Subject = !string.IsNullOrEmpty(item.Subject) ? item.Subject : "";
                    item.HTMlBody = !string.IsNullOrEmpty(item.HTMlBody) ? item.HTMlBody : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã cấu hình mail đã tồn tại" });
                        {
                            item.CreatedAt = DateTime.Now;
                            item.CreatedBy = currentUser.UserID;
                            item.UpdatedAt = DateTime.Parse("1900-01-01");
                            item.UpdatedBy = "";
                            db.Insert(item);
                            return Json(new { success = true, ID = item.Name, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.UpdatedAt = DateTime.Now;
                        item.UpdatedBy = currentUser.UserID;
                        db.Update(item);
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Chưa nhập đủ giá trị" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                log.Error("Utilities_Email" + item.ID + " - Create - " + e.Message);
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            finally { db.Close(); }
        }
    }
}