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
using Dapper;
using System.Globalization;

namespace THT.Controllers
{
    public class TimeScheduledController : CustomController
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
                dbConn.Close();
                return View("Index", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = dbConn.Select<TimeScheduled>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }

        //  
        public ActionResult UpdateDetail([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TimeScheduled> list)
        {
            var dbConn = new OrmliteConnection().openConn(); 
            if ((userAsset.ContainsKey("Insert") && userAsset["Insert"]) || (userAsset.ContainsKey("Update") && userAsset["Update"]))
            {
                foreach (var item in list)
                {
                    var isExist = dbConn.FirstOrDefault<TimeScheduled>(s=>s.ID==item.ID);
                    if (isExist == null)
                    {
                        var data = new TimeScheduled();
                      
                        data.TimeSheetName = !string.IsNullOrEmpty(item.TimeSheetName) ? item.TimeSheetName : "";
                        data.HousedHours = item.HousedHours != 0 ? item.HousedHours : 0;
                        data.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                        data.StartDate = item.StartDate;
                        data.EndDate = item.EndDate;
                        data.CreatedAt = DateTime.Now;
                        data.Active = item.Active;

                        data.CreatedBy = currentUser.UserID;
                        data.UpdatedAt = DateTime.Parse("1900-01-01");
                        data.UpdatedBy = "";
                        dbConn.Insert<TimeScheduled>(data);
                    }
                    else
                    {
                        isExist.TimeSheetName = !string.IsNullOrEmpty(item.TimeSheetName) ? item.TimeSheetName : "";
                        isExist.HousedHours = item.HousedHours != 0 ? item.HousedHours : 0;
                        isExist.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                        isExist.StartDate = item.StartDate;
                        isExist.EndDate = item.EndDate;
                        isExist.UpdatedAt = DateTime.Now;
                        isExist.Active = item.Active;
                        isExist.UpdatedBy = currentUser.UserID;
                        dbConn.Update<TimeScheduled>(isExist);
                    }
                }
                ModelState.AddModelError("Thành công!", " Tạo mới thành công.");
                return Json(new { sussess = true });
            }
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                return Json(list.ToDataSourceResult(request, ModelState));
            }
        }

        public ActionResult DeleteItem(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    var detail = new TimeScheduled();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<TimeScheduled>(s => s.ID == int.Parse(item)).Count() > 0)
                        {
                            var success = dbConn.Delete<TimeScheduled>(where: "ID = '" + item + "'") >= 1;

                            if (!success)
                            {
                                return Json(new { success = false, message = "Không thể lưu" });
                            }
                        }
                    }
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Không có quyền xóa." });
            }
        }
    }
}
