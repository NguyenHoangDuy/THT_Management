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
    public class Process_ProductionController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Process_Production
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
                    //ViewBag.listJobs = dbConn.Select<Jobs>(p => p.trang_thai == "A");

                    ViewBag.listJobs = dbConn.Select<DropListDown>("Select ma_cong_viec as Value, ten_cong_viec as Text from Jobs where trang_thai ='A'");
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
                var data = dbConn.Select<Process_Production>(whereCondition).ToList();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create(Process_Production item)
        {
            using (IDbConnection db = new OrmliteConnection().openConn())
            {
                try
                {
                    var isExist = db.SingleOrDefault<Process_Production>("ma_quy_trinh_sx={0}", item.ma_quy_trinh_sx);

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.nguoi_tao == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã công việc đã tồn tại" });
                        string id = "";
                        var checkID = db.SingleOrDefault<Process_Production>("SELECT ma_quy_trinh_sx, Id FROM dbo.Process_Production ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.ma_quy_trinh_sx.Substring(2, checkID.ma_quy_trinh_sx.Length - 2)) + 1;
                            id = "QT" + String.Format("{0:000000000}", nextNo);
                        }
                        else
                        {
                            id = "QT000000001";
                        }

                        item.ma_quy_trinh_sx = id;
                        item.ten_quy_trinh_sx = !string.IsNullOrEmpty(item.ten_quy_trinh_sx) ? item.ten_quy_trinh_sx : "";
                        item.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                        item.ngay_tao = DateTime.Now;
                        item.ngay_cap_nhat = DateTime.Now;
                        item.nguoi_tao = currentUser.UserID;
                        item.nguoi_cap_nhat = currentUser.UserID;
                        db.Insert<Process_Production>(item);

                        return Json(new { success = true, ma_quy_trinh_sx = item.ma_quy_trinh_sx, nguoi_tao = item.nguoi_tao, ngay_tao = item.ngay_tao, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        isExist.ten_quy_trinh_sx = !string.IsNullOrEmpty(item.ten_quy_trinh_sx) ? item.ten_quy_trinh_sx : "";
                        isExist.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                        isExist.ngay_cap_nhat = DateTime.Now;
                        isExist.nguoi_cap_nhat = currentUser.UserID;
                        db.Update(isExist);
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" });

                }
                catch (Exception e)
                {
                    log.Error("Danh mục công việc - Create - " + e.Message);
                    return Json(new { success = false, message = e.Message });
                }
                finally { db.Close(); }
            }
        }

        public ActionResult Delete(string data)
        {
            using (var dbConn = new OrmliteConnection().openConn())
            {
                if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
                {
                    try
                    {
                        string[] separators = { "@@" };
                        var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                        {
                            foreach (var item in listdata)
                            {
                                dbConn.Delete<Process_Production_Job>(s => s.ma_quy_trinh_sx == item);
                                dbConn.Delete<Process_Production>(s => s.ma_quy_trinh_sx == item);
                            }
                            dbTrans.Commit();
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
                    return Json(new { success = false, message = "Bạn không có quyền xóa dữ liệu." });
                }
            }
        }

        public ActionResult ReadJob([DataSourceRequest]DataSourceRequest request, string ma_quy_trinh_sx)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            var data = db.Select<Process_Production_Job>("SELECT * FROM Process_Production_Job WHERE ma_quy_trinh_sx = '" + ma_quy_trinh_sx + "'").ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateJob([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Process_Production_Job> list, string ma_quy_trinh_sx)
        {
            using (var dbConn = new OrmliteConnection().openConn())
            {
                if (userAsset.ContainsKey("Update") && userAsset["Update"])
                {
                    if (list != null)
                    {
                        try
                        {
                            dbConn.Delete<Process_Production_Job>(s => s.ma_quy_trinh_sx == ma_quy_trinh_sx);
                            foreach (Process_Production_Job item in list)
                            {
                                Process_Production_Job newdata = new Process_Production_Job();
                                newdata.ma_quy_trinh_sx = ma_quy_trinh_sx;
                                newdata.ma_cong_viec = item.ma_cong_viec;
                                newdata.so_thu_tu = item.so_thu_tu != 0 ? item.so_thu_tu : 1;
                                newdata.trang_thai = "A";
                                newdata.ngay_tao = DateTime.Now;
                                newdata.ngay_cap_nhat = DateTime.Now;
                                newdata.nguoi_tao = currentUser.UserID;
                                newdata.nguoi_cap_nhat = currentUser.UserID;
                                dbConn.Insert<Process_Production_Job>(newdata);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("error", ex.Message);
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }
                    }
                    return Json(new { sussess = true });
                }
                else
                {
                    ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                    return Json(list.ToDataSourceResult(request, ModelState));
                }
            }
        }

        public ActionResult DeleteJob(string data, string ma_quy_trinh_sx)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in listdata)
                    {
                        dbConn.Delete<Process_Production_Job>(s => s.ma_quy_trinh_sx == ma_quy_trinh_sx && s.ma_cong_viec == item);
                    }
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    log.Error(" Process_Production - DeleteJob - " + e.Message);
                    return RedirectToAction("NoAccess", "Error");
                    //return Json(new { success = false, message = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Không có quyền cập nhật" });
            }
        }
    }
}