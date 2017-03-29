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
using System.Text;

namespace THT.Controllers
{
    [Authorize]
    [NoCache]
    public class Utilities_TerritoryController : CustomController
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
                //dict["listMaster_SKU"] = dbConn.Select<Master_SKU>();
                dbConn.Close();
                return View("_PartialOthersTerritory", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult IndexCountry()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listMaster_SKU"] = dbConn.Select<Master_SKU>();
                dbConn.Close();
                return PartialView("_PartialTreeCountry", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult IndexRegion()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listMaster_SKU"] = dbConn.Select<Master_SKU>();
                dbConn.Close();
                return PartialView("_PartialTreeRegion", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult IndexProvince()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listMaster_SKU"] = dbConn.Select<Master_SKU>();
                dbConn.Close();
                return PartialView("_PartialTreeProvince", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult IndexDistrict()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                //dict["listMaster_SKU"] = dbConn.Select<Master_SKU>();
                dbConn.Close();
                return PartialView("_PartialTreeDistrict", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Territory().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read_Country([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Territory().GetPageCountry(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read_Region([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Territory().GetPageRegion(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read_Province([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Territory().GetPageProvince(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read_District([DataSourceRequest]DataSourceRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            var data = new Master_Territory().GetPageDistrict(request.Page, request.PageSize, whereCondition);
            return Json(data);
        }


        //====================================================Index=================================================        

        [HttpPost]
        public ActionResult GetByID(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Master_Territory>(id);
                return Json(new { success = true, data = data });
                //var groupMaster_SKU = dbConn.Select<Master_SKU>(p => p.CategoryID == id);
                //return Json(new { success = true, data = data, groupMaster_SKU = groupMaster_SKU });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        
        //import export (import thuong chi 1 lan thoi nen bo qua)
        public FileResult Export_Country([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ViTriDiaLy_QuocGia.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new Master_Territory().GetExportCountry(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TerritoryID;
                    ws.Cells["B" + rowNum].Value = item.TerritoryName;
                    ws.Cells["C" + rowNum].Value = item.Title;
                    ws.Cells["D" + rowNum].Value = item.Latitude;
                    ws.Cells["E" + rowNum].Value = item.Longitude;
                    //ws.Cells["F" + rowNum].Value = item.Latitude;
                    //ws.Cells["G" + rowNum].Value = item.Longitude;
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
                        "ViTriDiaLy_QuocGia" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_Region([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ViTriDiaLy_VungMien.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new Master_Territory().GetExportRegion(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TerritoryID;
                    ws.Cells["B" + rowNum].Value = item.TerritoryName;
                    ws.Cells["C" + rowNum].Value = item.ParentName;
                    ws.Cells["D" + rowNum].Value = item.Title;
                    ws.Cells["E" + rowNum].Value = item.Latitude;
                    ws.Cells["F" + rowNum].Value = item.Longitude;
                    //ws.Cells["G" + rowNum].Value = item.Longitude;
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
                        "ViTriDiaLy_VungMien" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_Province([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ViTriDiaLy_TinhThanh.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                 IDbConnection db = new OrmliteConnection().openConn();
                 var lstResult = new Master_Territory().GetExportProvince(request, whereCondition);
                 int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TerritoryID;
                    ws.Cells["B" + rowNum].Value = item.TerritoryName;
                    ws.Cells["C" + rowNum].Value = item.ParentName;
                    ws.Cells["D" + rowNum].Value = item.Title;
                    ws.Cells["E" + rowNum].Value = item.Latitude;
                    ws.Cells["F" + rowNum].Value = item.Longitude;
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
                        "ViTriDiaLy_TinhThanh" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_District([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ViTriDiaLy_QuanHuyen.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new Master_Territory().GetExportDistrict(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TerritoryID;
                    ws.Cells["B" + rowNum].Value = item.TerritoryName;
                    ws.Cells["C" + rowNum].Value = item.ParentName;
                    ws.Cells["D" + rowNum].Value = item.Title;
                    ws.Cells["E" + rowNum].Value = item.Latitude;
                    ws.Cells["F" + rowNum].Value = item.Longitude;
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
                        "ViTriDiaLy_QuanHuyen" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
        public FileResult Export_Ward([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/ViTriDiaLy_PhuongXa.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
               IDbConnection db = new OrmliteConnection().openConn();
               var lstResult = new Master_Territory().GetExportWard(request, whereCondition);
               int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.TerritoryID;
                    ws.Cells["B" + rowNum].Value = item.TerritoryName;
                    ws.Cells["C" + rowNum].Value = item.ParentName;
                    ws.Cells["D" + rowNum].Value = item.Title;
                    ws.Cells["E" + rowNum].Value = item.Latitude;
                    ws.Cells["F" + rowNum].Value = item.Longitude;
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
                        "ViTriDiaLy_PhuongXa" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        [HttpPost]
        public ActionResult GetDistrictByProvinceID(string id)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var data = new Master_Territory().GetDistrictByProvinceID(id);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetWardByDistrictID(string id)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var data = new Master_Territory().GetWardByDistrictID(id);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
    }
}