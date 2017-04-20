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
    public class JobsController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Jobs
        public ActionResult Index()
        {

            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                using (IDbConnection dbConn = new OrmliteConnection().openConn())
                {
                    var dict = new Dictionary<string, object>();
                    dict["asset"] = userAsset;
                    dict["activestatus"] = new CommonLib().GetActiveStatus();
                    ViewBag.listJobType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.JobTYPE);
                    ViewBag.listStatus = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.Status);
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
                var data = dbConn.Select<Jobs>(whereCondition).ToList();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create(Jobs item)
        {
            using (IDbConnection db = new OrmliteConnection().openConn())
            {
                try
                {
                    var isExist = db.SingleOrDefault<Jobs>("ma_cong_viec={0}", item.ma_cong_viec);

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.nguoi_tao == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã công việc đã tồn tại" });
                        string id = "";
                        var checkID = db.SingleOrDefault<Jobs>("SELECT ma_cong_viec, Id FROM dbo.Jobs ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.ma_cong_viec.Substring(2, checkID.ma_cong_viec.Length - 2)) + 1;
                            id = "JB" + String.Format("{0:000000}", nextNo);
                        }
                        else
                        {
                            id = "JB000001";
                        }

                        item.ma_cong_viec = id;
                        item.ten_cong_viec = !string.IsNullOrEmpty(item.ten_cong_viec) ? item.ten_cong_viec : "";
                        item.loai_cong_viec = !string.IsNullOrEmpty(item.loai_cong_viec) ? item.loai_cong_viec : "";
                        item.dinh_muc_cong_viec = !string.IsNullOrEmpty(item.dinh_muc_cong_viec.ToString()) ? item.dinh_muc_cong_viec : 0;
                        item.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                        item.ngay_tao = DateTime.Now;
                        item.ngay_cap_nhat = DateTime.Now;
                        item.nguoi_tao = currentUser.UserID;
                        item.nguoi_cap_nhat = currentUser.UserID;
                        db.Insert<Jobs>(item);

                        return Json(new { success = true, ma_cong_viec = item.ma_cong_viec, nguoi_tao = item.nguoi_tao, ngay_tao = item.ngay_tao, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        isExist.ten_cong_viec = !string.IsNullOrEmpty(item.ten_cong_viec) ? item.ten_cong_viec : "";
                        isExist.loai_cong_viec = !string.IsNullOrEmpty(item.loai_cong_viec) ? item.loai_cong_viec : "";
                        isExist.dinh_muc_cong_viec = !string.IsNullOrEmpty(item.dinh_muc_cong_viec.ToString()) ? item.dinh_muc_cong_viec : 0;
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
            var dbConn = new OrmliteConnection().openConn();
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
                            dbConn.Delete<Process_Production_Job>(s => s.ma_cong_viec == item);
                            dbConn.Delete<Jobs>(s => s.ma_cong_viec == item);
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
}