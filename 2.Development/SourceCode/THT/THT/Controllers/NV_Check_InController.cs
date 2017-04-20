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
    public class NV_Check_InController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: NV_Check_In
        public ActionResult Index()
        {
            using (IDbConnection dbConn = new OrmliteConnection().openConn())
            {
                var dict = new Dictionary<string, object>();
                ViewBag.listStatus = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.Status);
                return View(dict);
            }
        }

        public ActionResult Create(Check_In item)
        {
            using (IDbConnection db = new OrmliteConnection().openConn())
            {
                try
                {

                    var isExistNV = db.FirstOrDefault<Employee>(p => p.ma_nhan_vien == item.ma_nhan_vien && p.mat_khau == item.mat_khau);

                    if (isExistNV == null)
                        return Json(new { success = false, message = "Mã nhân viên hoặc mật khẩu không hợp lệ tồn tại" });

                    var isExistCheck_In = db.Select<Check_In>("select * from Check_In where DATEDIFF(D,ngay,GETDATE())=0 AND ma_nhan_vien={0}".Params(item.ma_nhan_vien)).FirstOrDefault();

                    if (isExistCheck_In != null)
                        return Json(new { success = false, message = "Mã nhân viên đã kiểm tra thiết bị đầu ngày" });

                    item.ma_nhan_vien = item.ma_nhan_vien;
                    item.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                    item.ngay = DateTime.Now;
                    item.ngay_tao = DateTime.Now;
                    item.nguoi_tao = item.ma_nhan_vien;
                    item.ngay_cap_nhat = DateTime.Now;
                    item.nguoi_cap_nhat = item.ma_nhan_vien;


                    //var success = db.Execute(@"INSERT INTO Check_In (ma_nhan_vien, ngay, trang_thai, ngay_tao, nguoi_tao, ngay_cap_nhat, nguoi_cap_nhat)
                    //                        VALUES(@ma_nhan_vien, @ngay, @trang_thai, @ngay_tao, @nguoi_tao, @ngay_cap_nhat, @nguoi_cap_nhat)", 
                    //                    new
                    //                    {
                    //                        ma_nhan_vien = item.ma_nhan_vien,
                    //                        ngay = item.ngay,
                    //                        trang_thai = item.trang_thai,
                    //                        ngay_tao = item.ngay_tao,
                    //                        nguoi_tao = item.nguoi_tao,
                    //                        ngay_cap_nhat = item.ngay_cap_nhat,
                    //                        nguoi_cap_nhat = item.nguoi_cap_nhat,
                    //                    }) == 1;
                    db.Insert<Check_In>(item);

                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    log.Error(" NV_Check_In - Create - " + e.Message);
                    return Json(new { success = false, message = e.Message });
                }
                finally { }
            }
        }
    }
}