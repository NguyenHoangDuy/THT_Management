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
                    ViewBag.listEmployeeType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.EmployeeTYPE);
                    ViewBag.listJobs = dbConn.Select<Jobs>(p => p.trang_thai == "A");
                    return View(dict);
                }
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {

            //using (var dbConn = new OrmliteConnection().openConn())
            //{
            //    string whereCondition = "";
            //    if (request.Filters.Count > 0)
            //    {
            //        whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            //    }
            //    request.Filters = null;
            //    var data = dbConn.Select<Employee>(whereCondition).ToList();
            //    return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            //}
            using (var dbConn = new OrmliteConnection().openConn())
            {
                var data = new List<Employee>();
                if (request.Filters.Count > 0)
                {
                    var whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                    data = dbConn.Query<Employee>("[p_Get_Employee]", new { WhereCondition = " AND " + whereCondition }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else
                {
                    data = dbConn.Query<Employee>("[p_Get_Employee]", new { WhereCondition = "" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                //request.Filters = null;
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetJobs(string ma_nhan_vien)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<Employee_Job>(s => s.ma_nhan_vien == ma_nhan_vien);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public ActionResult Create(Employee item)
        {
            using (IDbConnection db = new OrmliteConnection().openConn())
            {
                try
                {
                    var isExist = db.SingleOrDefault<Employee>("ma_nhan_vien={0}", item.ma_nhan_vien);

                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && isExist == null)
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Mã cấu hình đã tồn tại" });
                        string id = "";
                        var checkID = db.SingleOrDefault<Employee>("SELECT ma_nhan_vien, Id FROM dbo.Employee ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.ma_nhan_vien.Substring(2, checkID.ma_nhan_vien.Length - 2)) + 1;
                            id = "NV" + String.Format("{0:00000000}", nextNo);
                        }
                        else
                        {
                            id = "NV00000001";
                        }

                        item.ma_nhan_vien = id;
                        item.ten_nhan_vien = !string.IsNullOrEmpty(item.ten_nhan_vien) ? item.ten_nhan_vien : "";
                        item.birthday = DateTime.ParseExact(item.strbirthday, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        item.loai_nhan_vien = !string.IsNullOrEmpty(item.loai_nhan_vien) ? item.loai_nhan_vien : "";
                        item.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                        item.mat_khau = Helpers.RandomString.Generate(8);
                        item.ngay_tao = DateTime.Now;
                        item.nguoi_tao = currentUser.UserID;
                        item.ngay_cap_nhat = DateTime.Now;
                        item.nguoi_cap_nhat = currentUser.UserID;
                        db.Insert<Employee>(item);

                        string[] separators = { "," };
                        var listdata = Request["listJobs"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        db.Delete<Employee_Job>(s => s.ma_nhan_vien == item.ma_nhan_vien);

                        foreach (var i in listdata)
                        {
                            Employee_Job newdata = new Employee_Job();
                            newdata.ma_nhan_vien = item.ma_nhan_vien;
                            newdata.ten_nhan_vien = item.ten_nhan_vien;
                            newdata.ma_cong_viec = i;
                            newdata.ngay_tao = DateTime.Now;
                            newdata.nguoi_tao = currentUser.UserID;
                            newdata.ngay_cap_nhat = DateTime.Now;
                            newdata.nguoi_cap_nhat = currentUser.UserID;
                            db.Insert<Employee_Job>(newdata);
                        }

                        return Json(new { success = true, ma_nhan_vien = item.ma_nhan_vien, nguoi_tao = item.nguoi_tao, ngay_tao = item.ngay_tao, });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        isExist.ten_nhan_vien = !string.IsNullOrEmpty(item.ten_nhan_vien) ? item.ten_nhan_vien : "";
                        isExist.loai_nhan_vien = !string.IsNullOrEmpty(item.loai_nhan_vien) ? item.loai_nhan_vien : "";
                        isExist.trang_thai = !string.IsNullOrEmpty(item.trang_thai) ? item.trang_thai : "A";
                        isExist.birthday = DateTime.ParseExact(item.strbirthday, "dd/MM/yyyy",
                                          System.Globalization.CultureInfo.InvariantCulture);
                        isExist.ngay_cap_nhat = DateTime.Now;
                        isExist.nguoi_cap_nhat = currentUser.UserID;

                        //var success = db.Execute(@"UPDATE CategoryConfig SET CategoryName = @CategoryName,
                        //                    IsSaved = @IsSaved,IsExpirated = @IsExpirated,SideboardID = @SideboardID,Format = @Format,
                        //                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                        //                    WHERE CategoryID = '" + item.CategoryID + "'", new
                        //{
                        //    CategoryName = !string.IsNullOrEmpty(item.CategoryName) ? item.CategoryName.Trim() : "",
                        //    IsSaved = !string.IsNullOrEmpty(item.IsSaved) ? item.IsSaved.Trim() : "",
                        //    IsExpirated = !string.IsNullOrEmpty(item.IsExpirated) ? item.IsExpirated.Trim() : "",
                        //    SideboardID = !string.IsNullOrEmpty(item.SideboardID) ? item.SideboardID.Trim() : "",
                        //    Format = !string.IsNullOrEmpty(item.Format) ? item.Format.Trim() : "",
                        //    UpdatedAt = DateTime.Now,
                        //    UpdatedBy = currentUser.UserID,
                        //}) == 1;

                        db.Update(isExist);
                        string[] separators = { "," };
                        var listdata = Request["listJobs"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        db.Delete<Employee_Job>(s => s.ma_nhan_vien == isExist.ma_nhan_vien);

                        foreach (var i in listdata)
                        {
                            Employee_Job newdata = new Employee_Job();
                            newdata.ma_nhan_vien = isExist.ma_nhan_vien;
                            newdata.ten_nhan_vien = isExist.ten_nhan_vien;
                            newdata.ma_cong_viec = i;
                            newdata.ngay_tao = DateTime.Now;
                            newdata.nguoi_tao = currentUser.UserID;
                            newdata.ngay_cap_nhat = DateTime.Now;
                            newdata.nguoi_cap_nhat = currentUser.UserID;
                            db.Insert<Employee_Job>(newdata);
                        }
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" });

                }
                catch (Exception e)
                {
                    log.Error("Danh mục nhân viên - Create - " + e.Message);
                    return Json(new { success = false, message = e.Message });
                }
                finally { }
            }
        }
    }
}