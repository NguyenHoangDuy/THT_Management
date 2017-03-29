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
using System.Globalization;
using System.Web.Helpers;

namespace THT.Controllers
{
    [Authorize]
    [NoCache]
    public class Utilities_AnnouncementController : CustomController
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
            var data = new Master_Announcement().GetPage(request.Page, request.PageSize, whereCondition, currentUser.UserID);
            return Json(data);
        }

    

        [HttpPost]
        public ActionResult Deactive(string data)
        {
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                try
                {
                    var newdata = data.Split(',');
                    foreach (string id in newdata)
                    {
                        dbConn.Delete<Master_Announcement>(p => p.AnnouncementID == Convert.ToInt32(id));
                    }

                    return Json(new { success = true, });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });

                }
                finally
                {
                    dbConn.Close();
                }
            }
            else
            {
                return Json(new { success = false, message = "You don't have permission" });
            }
        }


        //=====================================================================================================        

        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return View("_Utilities_AnnouncementTree", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }


        [HttpPost]
        public ActionResult GetById(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Master_Announcement>(id);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
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
                    var detail = new Master_Announcement();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<Master_Announcement>(s => s.AnnouncementID == int.Parse(item)).Count() > 0)
                        {
                            var success = dbConn.Delete<Master_Announcement>(where: "AnnouncementID = '" + item + "'") >= 1;

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
        public ActionResult UpdateStatusActive(string listUserID, int action)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var item in listdata)
                {

                    var success = dbConn.Update<Master_Announcement>(set: "Status = 1 ,"
                         + "UpdatedAt='" + DateTime.Now + "', "
                         + "UpdatedBy='" + currentUser.UserID + "'"
                        , where: "AnnouncementID = '" + item + "'") >= 1;

                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
        public ActionResult UpdateStatusInActive(string listUserID, int action)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var item in listdata)
                {
                    var success = dbConn.Update<Master_Announcement>(set: "Status = 0 ,"
                         + "UpdatedAt='" + DateTime.Now + "', "
                         + "UpdatedBy='" + currentUser.UserID + "'"
                        , where: "AnnouncementID = '" + item + "'") >= 1;

                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
        [ValidateInput(false)]
        public ActionResult Create(Master_Announcement item)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                if (Request.Files.Count > 0)
                {
                    //save avatar
                    HttpPostedFileBase file = Request.Files[0];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;

                        //check type file image
                        string fileexe = Path.GetExtension(fileName);
                        if (fileexe != ".jpg" && fileexe != ".jpeg" && fileexe != ".png")
                            return Json(new { success = false, message = "Vui lòng chọn tệp ảnh." });

                        //check size image
                        if (file.ContentLength > (1000 * 1024))
                            return Json(new { success = false, message = "Vui lòng chọn tệp ảnh dưới 1MB." });

                        //check info 
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        Random TenBienRanDom = new Random();

                        //get save file image
                        var filename = currentUser.UserID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fileName;

                        item.Image = filename;

                        filename = Path.Combine(Server.MapPath("~/Upload/Images/News/Announcement"), filename);
                        file.SaveAs(filename);

                        WebImage img = new WebImage(filename);
                        img.Resize(300, 300, false, false);
                        img.Save(filename);


                    }
                    else
                        item.Image = "";
                }

                var startDate = Request.Form["StartDate"].ToString();
                var endDate = Request.Form["EndDate"].ToString();
                DateTime dt = DateTime.Now;
                if (!DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày bắt đầu không đúng" });

                }
                item.StartDate = dt;

                if (!DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày kết thúc không đúng" });

                }
                item.EndDate = dt;

                int num;
                if (!int.TryParse(item.Orders.ToString(), out num))
                {
                    return Json(new { success = false, message = "Thứ tự tin phải là số nguyên" });
                }

                item.CreatedAt = DateTime.Now;
                item.CreatedBy = currentUser.UserID;

                dbConn.Insert<Master_Announcement>(item);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            finally { dbConn.Close(); }
            return Json(new { success = true });
        }
        [ValidateInput(false)]
        public ActionResult Update(Master_Announcement item)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                if (Request.Files.Count > 0)
                {
                    //save avatar
                    HttpPostedFileBase file = Request.Files[0];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = file.FileName;

                        //check type file image
                        string fileexe = Path.GetExtension(fileName);
                        if (fileexe != ".jpg" && fileexe != ".jpeg" && fileexe != ".png")
                            return Json(new { success = false, message = "Vui lòng chọn tệp ảnh." });

                        //check size image
                        if (file.ContentLength > (1000 * 1024))
                            return Json(new { success = false, message = "Vui lòng chọn tệp ảnh dưới 1MB." });

                        //check info 
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        Random TenBienRanDom = new Random();

                        //get save file image
                        var filename = currentUser.UserID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fileName;

                        item.Image = filename;

                        filename = Path.Combine(Server.MapPath("~/Upload/Images/News/Announcement"), filename);
                        file.SaveAs(filename);

                        WebImage img = new WebImage(filename);
                        img.Resize(300, 300, false, false);
                        img.Save(filename);


                    }

                }

                var startDate = Request.Form["StartDate"].ToString();
                var endDate = Request.Form["EndDate"].ToString();
                DateTime dt = DateTime.Now;


                if (!DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày bắt đầu không đúng" });

                }
                item.StartDate = dt;

                if (!DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày kết thúc không đúng" });

                }

                int num;
                if (!int.TryParse(item.Orders.ToString(), out num))
                {
                    return Json(new { success = false, message = "Thứ tự tin phải là số nguyên" });
                }

                item.EndDate = dt;
                item.CreatedAt = DateTime.Now;
                item.CreatedBy = currentUser.UserID;

                dbConn.Update<Master_Announcement>(item);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            finally { dbConn.Close(); }
            return Json(new { success = true });
        }
    }
}