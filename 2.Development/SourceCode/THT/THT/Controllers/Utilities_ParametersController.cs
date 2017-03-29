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
    public class Utilities_ParametersController : CustomController
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
                //dict["listrole"] = dbConn.Select<Auth_Role>("SELECT * FROM Auth_Role WHERE IsActive = 1");
                dbConn.Close();
                return View("_Parameters", dict);
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
            var data = dbConn.Select<Utilities_Parameters>(whereCondition);
            return Json(data.ToDataSourceResult(request));
        }

        //
        // GET: /DeliveryManage/Create
        public ActionResult Create(Utilities_Parameters item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(item.Type) && !string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(item.ParamID))
                {
                    var isExistName = db.SingleOrDefault<Utilities_Parameters>("Select * from Utilities_Parameters Where ParamID='" + item.ParamID + "'");
                    if (isExistName == null)
                    {
                        if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                        {
                            //if (isExist != null)
                            //    return Json(new { success = false, message = "Mã lý do đã tồn tại!" });
                            item.ParamID = !string.IsNullOrEmpty(item.ParamID) ? item.ParamID : "";
                            item.Value = !string.IsNullOrEmpty(item.Value) ? item.Value : "";
                            item.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                            item.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                            item.CreatedAt = DateTime.Now;
                            item.CreatedBy = currentUser.UserID;
                            item.UpdatedAt = DateTime.Parse("1900-01-01");
                            item.UpdatedBy = "";
                            db.Insert<Utilities_Parameters>(item);
                            long lastInsertId = db.GetLastInsertId();
                            return Json(new { success = true, ID = lastInsertId, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt });
                        }
                        else if (userAsset.ContainsKey("Update") && userAsset["Update"])
                        {
                            item.ParamID = !string.IsNullOrEmpty(item.ParamID) ? item.ParamID : "";
                            item.Value = !string.IsNullOrEmpty(item.Value) ? item.Value : "";
                            item.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                            item.Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr : "";
                            item.UpdatedAt = DateTime.Now;
                            item.UpdatedBy = currentUser.UserID;

                            var success = db.Execute(@"UPDATE Utilities_Parameters SET ParamID = @ParamID,
                                            Value = @Value,Type = @Type,Descr = @Descr,
                                            UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                                            WHERE ParamID = '" + item.ParamID + "'", new
                                                          {
                                                              ParamID = !string.IsNullOrEmpty(item.ParamID) ? item.ParamID.Trim() : "",
                                                              Value = !string.IsNullOrEmpty(item.Value) ? item.Value.Trim() : "",
                                                              Type = !string.IsNullOrEmpty(item.Type) ? item.Type.Trim() : "",
                                                              Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr.Trim() : "",
                                                              UpdatedAt = DateTime.Now,
                                                              UpdatedBy = currentUser.UserID,
                                                          }) == 1;

                            //db.Update<Utilities_Parameters>(item);
                            return Json(new { success = true });
                        }
                        else
                            return Json(new { success = false, message = "Bạn không có quyền" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Mã tham số đã tồn tại!" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Chưa nhập giá trị" });
                }
            }
            catch (Exception e)
            {
                log.Error("Parameter - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult GetReasonyCode(string ReasonID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Utilities_Parameters>(ReasonID);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        //public FileResult Export([DataSourceRequest]DataSourceRequest request)
        //{
        //    ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/Reason.xlsx")));
        //    ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
        //    if (userAsset["Export"])
        //    {
        //        string whereCondition = "";
        //        if (request.Filters.Count > 0)
        //        {
        //            whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
        //        }
        //        IDbConnection db = new OrmliteConnection().openConn();
        //       // var lstResult = db.Select<DC_Reason>(whereCondition);
        //        int rowNum = 2;
        //        foreach (var item in lstResult)
        //        {
        //            ws.Cells["A" + rowNum].Value = item.ReasonID;
        //            ws.Cells["B" + rowNum].Value = item.ReasonType;
        //            ws.Cells["C" + rowNum].Value = item.Description;
        //            ws.Cells["D" + rowNum].Value = item.IsActive ? "Đang hoạt động" : "Ngưng hoạt động";
        //            rowNum++;
        //        }
        //        db.Close();
        //    }
        //    else
        //    {
        //        ws.Cells["A2:E2"].Merge = true;
        //        ws.Cells["A2"].Value = "You don't have permission to export data.";
        //    }
        //    MemoryStream output = new MemoryStream();
        //    pck.SaveAs(output);
        //    return File(output.ToArray(), //The binary data of the XLS file
        //                "application/vnd.ms-excel", //MIME type of Excel files
        //                "Reason" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        //}
        public ActionResult UpdateDetail([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Utilities_Parameters> list)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                if (list != null)
                {
                    try
                    {
                        foreach (var item in list)
                        {

                            if (dbConn.Select<Utilities_Parameters>(s => s.ID == item.ID).Count() > 0)
                            {
                                //dbConn.Update<Utilities_Parameters>(set: "ParamID = '" + item.ParamID + "', Type = '" +
                                //    item.Type + "', Value =N'" + item.Value + "' , Descr =N'" + item.Descr + "' ,UpdatedAt = '" + DateTime.Now + "', UpdatedBy ='" +
                                //    currentUser.UserID + "'", where: "ID = " + item.ID);

                                var success = dbConn.Execute(@"UPDATE Utilities_Parameters SET ParamID = @ParamID,
                                            Value = @Value,Type = @Type,Descr = @Descr,
                                            UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                                            WHERE ID = '" + item.ID + "'", new
                                                               {
                                                                   ParamID = !string.IsNullOrEmpty(item.ParamID) ? item.ParamID.Trim() : "",
                                                                   Value = !string.IsNullOrEmpty(item.Value) ? item.Value.Trim() : "",
                                                                   Type = !string.IsNullOrEmpty(item.Type) ? item.Type.Trim() : "",
                                                                   Descr = !string.IsNullOrEmpty(item.Descr) ? item.Descr.Trim() : "",
                                                                   UpdatedAt = DateTime.Now,
                                                                   UpdatedBy = currentUser.UserID,
                                                               }) == 1;
                            }
                            else
                            {
                                var data = new Utilities_Parameters();
                                data.ParamID = item.ParamID;
                                data.Value = item.Value;
                                data.Type = item.Type;
                                data.Descr = item.Descr;
                                data.CreatedAt = DateTime.Now;
                                data.CreatedBy = currentUser.UserID;
                                data.UpdatedAt = DateTime.Parse("1900-01-01");
                                data.UpdatedBy = "";
                                dbConn.Insert<Utilities_Parameters>(data);
                            }

                        }
                        ModelState.AddModelError("Thành công!", " Cập nhật thành công.");
                        return Json(new { sussess = true });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("error", ex.Message);
                        return Json(list.ToDataSourceResult(request, ModelState));
                    }
                }
                return Json(new { sussess = true });
            }
            else if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                foreach (var item in list)
                {
                    var isExist = dbConn.GetByIdOrDefault<Utilities_Parameters>(item.ID);
                    if (isExist != null)
                    {
                        var data = new Utilities_Parameters();
                        data.ParamID = item.ParamID;
                        data.Value = item.Value;
                        data.Type = item.Type;
                        data.Descr = item.Descr;
                        data.CreatedAt = DateTime.Now;
                        data.CreatedBy = currentUser.UserID;
                        data.UpdatedAt = DateTime.Parse("1900-01-01");
                        data.UpdatedBy = "";
                        dbConn.Insert<Utilities_Parameters>(data);
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
                    var detail = new Utilities_Parameters();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<Utilities_Parameters>(s => s.ID == int.Parse(item)).Count() > 0)
                        {
                            var success = dbConn.Delete<Utilities_Parameters>(where: "ID = '" + item + "'") >= 1;

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
