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
    public class EmployeeController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Employee
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
                    return View(dict);
                }
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
    }
}