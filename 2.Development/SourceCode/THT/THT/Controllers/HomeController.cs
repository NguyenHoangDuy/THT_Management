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

namespace THT.Controllers
{
    [Authorize]
    //[NoCache]
    public class HomeController : CustomController
    {        

        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listAnnouncement = dbConn.Select<Master_Announcement>().OrderBy( s => s.Orders);
                dbConn.Close();
                return View("_Home", dict);
            }
            else
                return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Announcement_Read([DataSourceRequest] DataSourceRequest request)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Select<General_Notification>();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult DetailAnnouncement(int id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var data = dbConn.GetById<Master_Announcement>(id);
            return View("_DetailAnnouncement", data);
        }
        
        public ActionResult PartialChangePass()
        {
            return PartialView("_ChangePass");
        }
    }
}