using Kendo.Mvc.UI;
using THT.Models;
using THT.Service;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace THT.Controllers
{
    public class GeneralNotificationController : CustomController
    {
        // GET: GeneralNotification
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return View("Index", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");

        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new General_Notification().GetPage(request, whereCondition);
            return Json(data);
        }

        public ActionResult GetById(int Id = 0)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var item = dbConn.GetById<General_Notification>(Id);
            if (item != null)
            {
                return Json(new { success = true, data = item });
            }
            return Json(new { success = false, error = "Error" });
        }
        [ValidateInput(false)]
        public ActionResult Create(General_Notification item)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
               

                var startDate = Request.Form["StartDate"].ToString();
                var endDate = Request.Form["EndDate"].ToString();
                DateTime dt = DateTime.Now;
                if (!DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày bắt đầu không đúng" });

                }
                item.StartDate = dt;

                if (!DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày kết thúc không đúng" });

                }
                item.EndDate = dt;

                int num;
                if (!int.TryParse(item.Orders.ToString(), out num))
                {
                    return Json(new { success = false, message = "Thứ tự tin phải là số nguyên" });
                }

                item.CreatedAt = DateTime.Now;
                item.CreatedBy = currentUser.UserID;

                dbConn.Insert<General_Notification>(item);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            finally { dbConn.Close(); }
            return Json(new { success = true });
        }
        [ValidateInput(false)]
        public ActionResult Update(General_Notification item)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                

                var startDate = Request.Form["StartDate"].ToString();
                var endDate = Request.Form["EndDate"].ToString();
                DateTime dt = DateTime.Now;


                if (!DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày bắt đầu không đúng" });

                }
                item.StartDate = dt;

                if (!DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày kết thúc không đúng" });

                }

                int num;
                if (!int.TryParse(item.Orders.ToString(), out num))
                {
                    return Json(new { success = false, message = "Thứ tự tin phải là số nguyên" });
                }

                item.EndDate = dt;
                item.CreatedAt = DateTime.Now;
                item.CreatedBy = currentUser.UserID;

                dbConn.Update<General_Notification>(item);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            finally { dbConn.Close(); }
            return Json(new { success = true });
        }

        public ActionResult UpdateStatusActive(string listUserID, int action)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var item in listdata)
                {
                    if (dbConn.Select<General_Notification>(s => s.Id == int.Parse(item)).Count() > 0)
                    {
                        var success = dbConn.Update<General_Notification>(set: "Status = 1 ,"
                             + "UpdatedAt='" + DateTime.Now + "', "
                             + "UpdatedBy='" + currentUser.UserID + "'"
                            , where: "Id = '" + item + "'") >= 1;
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
        public ActionResult UpdateStatusInActive(string listUserID, int action)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var item in listdata)
                {
                    if (dbConn.Select<General_Notification>(s => s.Id == int.Parse(item)).Count() > 0)
                    {
                        var success = dbConn.Update<General_Notification>(set: "Status = 0 ,"
                             + "UpdatedAt='" + DateTime.Now + "', "
                             + "UpdatedBy='" + currentUser.UserID + "'"
                            , where: "Id = '" + item + "'") >= 1;
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
    }
}