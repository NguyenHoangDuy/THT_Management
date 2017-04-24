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
    public class Lenh_San_XuatController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Lenh_San_Xuat
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                using (IDbConnection dbConn = new OrmliteConnection().openConn())
                {

                    var dict = new Dictionary<string, object>();
                    dict["asset"] = userAsset;
                    dict["activestatus"] = new CommonLib().GetActiveStatus();
                    ViewBag.listStatus = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.StatusLSX);
                    ViewBag.listProcess_Production = dbConn.Select<Process_Production>(p => p.trang_thai == "A");
                    ViewBag.listDocument = dbConn.Select<Document>();
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
                var data = dbConn.Select<Lenh_San_Xuat>(whereCondition).ToList();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create(Lenh_San_Xuat item)
        {
            using (IDbConnection db = new OrmliteConnection().openConn())
            {
                try
                {
                    //using (var dbTrans = db.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        var isExist = db.SingleOrDefault<Lenh_San_Xuat>("ma_lenh_sx={0}", item.ma_lenh_sx);


                        if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.nguoi_tao == null)
                        {
                            if (isExist != null)
                                return Json(new { success = false, message = "Số lệnh sản xuất đã tồn tại" });


                            var lstProcess_Production_Job = db.Select<Process_Production_Job>(s => s.trang_thai == "A" && s.ma_quy_trinh_sx == item.ma_quy_trinh_sx).OrderBy(s => s.so_thu_tu);

                            DateTime dateserver = Helper.GetServerTime();

                            if (lstProcess_Production_Job.Count() == 0)
                                return Json(new { success = false, message = "Quy trình sản suất chưa định nghĩa các bước công việc" });

                            item.thoi_gian_du_kien = DateTime.ParseExact(item.strthoi_gian_du_kien, "dd/MM/yyyy",
                                         System.Globalization.CultureInfo.InvariantCulture);
                            item.trang_thai = "N";
                            item.ngay_tao = DateTime.Now;
                            item.ngay_cap_nhat = DateTime.Now;
                            item.nguoi_tao = currentUser.UserID;
                            item.nguoi_cap_nhat = currentUser.UserID;
                            db.Insert<Lenh_San_Xuat>(item);

                            for (int i = 1; i <= item.so_luong; ++i)
                            {
                                foreach (var job in lstProcess_Production_Job)
                                {
                                    Product newdata = new Product();
                                    newdata.ma_lenh_sx = item.ma_lenh_sx;
                                    newdata.ma_quy_trinh_sx = item.ma_quy_trinh_sx;
                                    newdata.ma_cong_viec = job.ma_cong_viec;
                                    newdata.so_thu_tu = job.so_thu_tu;
                                    newdata.ma_san_pham = item.ma_lenh_sx + dateserver.ToString("MMyy") + i.ToString("000");
                                    newdata.trang_thai = "N";
                                    newdata.ngay_tao = DateTime.Now;
                                    newdata.nguoi_tao = currentUser.UserID;
                                    newdata.ngay_cap_nhat = DateTime.Now;
                                    newdata.nguoi_cap_nhat = currentUser.UserID;
                                    db.Insert<Product>(newdata);
                                }
                            }

                            //dbTrans.Commit();
                            return Json(new { success = true, ma_lenh_sx = item.ma_lenh_sx, nguoi_tao = item.nguoi_tao, ngay_tao = item.ngay_tao, });
                        }
                        else
                            return Json(new { success = false, message = "Bạn không có quyền" });
                    }

                }
                catch (Exception e)
                {
                    log.Error("Danh mục lệnh sản xuất - Create - " + e.Message);
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
                            dbConn.Delete<Lenh_San_Xuat>(s => s.ma_lenh_sx == item);
                            dbConn.Delete<Product>(s => s.ma_lenh_sx == item);
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