using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using THT.Models;
using THT.Helpers;
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

namespace THT.Controllers
{
    public class Check_InController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Check_In
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                using (IDbConnection dbConn = new OrmliteConnection().openConn())
                {
                    var dict = new Dictionary<string, object>();
                    dict["asset"] = userAsset;
                    dict["activestatus"] = new CommonLib().GetActiveStatus();
                    ViewBag.listStatus = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.Status);
                    ViewBag.listEmployee = dbConn.Select<Employee>();
                    return View(dict);
                }
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {

            using (var dbConn = new OrmliteConnection().openConn())
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                //request.Filters = null;
                var data = dbConn.Select<Check_In>(whereCondition).ToList();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Update(string data)
        {
            using (var dbConn = new OrmliteConnection().openConn())
            {
                if (userAsset.ContainsKey("Update") && userAsset["Update"])
                {
                    try
                    {
                        string[] separators = { "@@" };
                        var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in listdata)
                        {
                            var isExit = dbConn.FirstOrDefault<Check_In>(p => p.id == int.Parse(item));
                            isExit.trang_thai = "A";
                            isExit.ngay = DateTime.Now;
                            isExit.ngay_cap_nhat = DateTime.Now;
                            isExit.nguoi_cap_nhat = currentUser.UserID;
                            dbConn.Update<Check_In>(isExit);
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
                    return Json(new { success = false, message = "Bạn không có quyền cập nhật dữ liệu." });
                }
            }
        }
    }
}