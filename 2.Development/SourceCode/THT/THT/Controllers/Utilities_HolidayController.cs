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
    public class Utilities_HolidayController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Calendar().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }

        [HttpPost]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<THT.Models.Master_Calendar> lst)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();

            try
            {
                foreach (var item in lst)
                {
                    if (userAsset.ContainsKey("Update") && userAsset["Update"] && dbConn.GetByIdOrDefault<Master_Calendar>(item.Date) != null)
                    {
                        if (string.IsNullOrEmpty(item.Holiday))
                        {
                            item.Holiday = "";
                        }
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowUpdatedBy = currentUser.UserID;
                        dbConn.Update<Master_Calendar>(item);
                    }
                    else
                        return Json(new { success = false, message = "You don't have permission" });
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                log.Error("Utilities_Holiday - Create - " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                dbConn.Close();
            }

        }



        //=====================================================================================================        

        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                //IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listrole"] = dbConn.Select<Auth_Role>();
                //dict["distributorgroup"] = dbConn.Select<Master_DistributorGroup>();
                //dict["distributor"] = dbConn.Select<Master_Distributor>("SELECT DistributorID, DistributorName FROM Master_Distributor");
                //dbConn.Close();
                return View("_Utilities_Holiday", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }


        [HttpPost]
        public ActionResult GetByID(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Master_Calendar>(id);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        //import export
        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/Lich.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = db.Select<Master_Calendar>("SELECT * FROM [Master_Calendar] WHERE 1=1 " + whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = Convert.ToDateTime(item.Date).ToString("dd/MM/yyyy"); 
                    ws.Cells["B" + rowNum].Value = item.Week;
                    ws.Cells["C" + rowNum].Value = item.Month;
                    ws.Cells["D" + rowNum].Value = item.Year;
                    ws.Cells["E" + rowNum].Value = item.Holiday;
                    rowNum++;
                }
                db.Close();
            }
            else
            {
                ws.Cells["A2:E2"].Merge = true;
                ws.Cells["A2"].Value = "You don't have permission to export data.";
            }
            MemoryStream output = new MemoryStream();
            pck.SaveAs(output);
            return File(output.ToArray(), //The binary data of the XLS file
                        "application/vnd.ms-excel", //MIME type of Excel files
                        "Lich" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        [HttpPost]
        public ActionResult Import()
        {
            try
            {
                if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = string.Format("{0}/{1}", Server.MapPath("~/ExcelImport"), Request.Files["FileUpload"].FileName);
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        // Save file to folder                            
                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        List<SqlParameter> param = new List<SqlParameter>();
                        param.Add(new SqlParameter("@UserID", currentUser.UserID));
                        param.Add(new SqlParameter("@FilePath", fileLocation));
                        //param.Add(new SqlParameter() { ParameterName = "@Output", Direction = ParameterDirection.InputOutput, Value = 0 });
                        DataSet ds = new SqlHelper().ExcuteQueryDataSet("p_Master_Calendar_Import", param);
                        if (ds.Tables.Count != 2)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = true, errorfile = ds.Tables[1].Rows[0][0].ToString() });
                        }
                    }
                }
                return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
            }
            catch (Exception e)
            {
                log.Error("Utilities_Holiday - Import - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
        }
       
    }
}