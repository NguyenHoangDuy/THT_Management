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
using System.Web.Helpers;
using System.Globalization;
using Dapper;
using THT.Helpers;

namespace THT.Controllers
{
    [Authorize]
    [NoCache]
    public class Auth_UserController : CustomController
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
            var data = new Auth_User().GetPage(request, whereCondition);
            return Json(data);
        }

        public bool UpdateUsersBrand(Auth_User item, string listBrandID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            bool success = true;

            db.Close();
            return success;
        }

        public bool UpdateUsersRole(Auth_User item, string listRoles)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            bool success = true;

            db.Execute(@"delete Auth_UserInRole where UserID='" + item.UserID + "'");

            if (listRoles != null)
            {
                string[] listRolesID = listRoles.Split(',');

                for (int i = 0; i < listRolesID.Count(); i++)
                {
                    Auth_UserInRole data = new Auth_UserInRole();
                    data.UserID = item.UserID;
                    data.RoleID = int.Parse(listRolesID[i]);
                    db.Insert<Auth_UserInRole>(data);
                }
            }
            db.Close();
            return success;
        }

        [HttpPost]
        public ActionResult Create(Auth_User item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var listBrandID = Request["listBrandID"];
                if (!string.IsNullOrEmpty(item.UserID) &&
                    !string.IsNullOrEmpty(item.DisplayName) &&
                    !string.IsNullOrEmpty(item.FullName))
                {
                    var isExist = db.GetByIdOrDefault<Auth_User>(item.UserID);
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.Birthday = DateTime.Parse("1900-01-01");
                    item.RowCreatedAt = DateTime.Now;
                    item.RowCreatedBy = currentUser.UserID;
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
                    {
                        if (isExist != null)
                            return Json(new { success = false, message = "Người dùng đã tồn tại." });
                        item.Password = SqlHelper.GetMd5Hash("123456");
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        item.RowUpdatedAt = DateTime.Parse("1900-01-01");
                        item.RowUpdatedBy = "";
                        db.Insert<Auth_User>(item);
                        var success = UpdateUsersBrand(item, listBrandID);
                        return Json(new { success = true, UserID = item.UserID, RowCreatedAt = item.RowCreatedAt, RowCreatedBy = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                    {
                        item.Password = isExist.Password;
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowUpdatedBy = currentUser.UserID;

                        if (isExist.RowCreatedBy != "system")
                        {
                            //db.Update<Auth_User>(item);
                            var success = db.Execute(@"UPDATE Auth_User SET IsActive = @IsActive
                                    , FullName=@FullName,DisplayName=@DisplayName,Email=@Email
                                    , Address=@Address,Phone=@Phone,
                                    RowUpdatedAt = @RowUpdatedAt, RowUpdatedBy = @RowUpdatedBy
                                    WHERE UserID = '" + item.UserID + "'", new
                            {
                                IsActive = item.IsActive,
                                FullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName.Trim() : "",
                                DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName.Trim() : "",
                                Email = !string.IsNullOrEmpty(item.Email) ? item.Email.Trim() : "",
                                Address = !string.IsNullOrEmpty(item.Address) ? item.Address.Trim() : "",
                                Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone.Trim() : "",
                                RowUpdatedAt = DateTime.Now,
                                RowUpdatedBy = currentUser.UserID,
                            }) == 1;

                            success = UpdateUsersBrand(item, listBrandID);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Dữ liệu này không cho chỉnh sửa liên hệ admin để biết thêm chi tiết" });
                        }
                        return Json(new { success = true });
                    }
                    else
                        return Json(new { success = false, message = "Bạn không có quyền" });
                }
                else
                {
                    return Json(new { success = false, message = "Chưa nhập giá trị" });
                }
            }
            catch (Exception e)
            {
                log.Error("Auth_User - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        public FileResult Export([DataSourceRequest]DataSourceRequest request)
        {
            ExcelPackage pck = new ExcelPackage(new FileInfo(Server.MapPath("~/ExportTemplate/NguoiDung.xlsx")));
            ExcelWorksheet ws = pck.Workbook.Worksheets["Data"];
            if (userAsset["Export"])
            {
                string whereCondition = "";
                if (request.Filters.Count > 0)
                {
                    whereCondition = " AND " + new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                }
                IDbConnection db = new OrmliteConnection().openConn();
                var lstResult = new Auth_User().GetExport(request, whereCondition);
                int rowNum = 2;
                foreach (var item in lstResult)
                {
                    ws.Cells["A" + rowNum].Value = item.UserID;
                    ws.Cells["B" + rowNum].Value = item.DisplayName;
                    ws.Cells["C" + rowNum].Value = item.FullName;
                    ws.Cells["D" + rowNum].Value = item.Email;
                    ws.Cells["E" + rowNum].Value = item.Phone;
                    ws.Cells["F" + rowNum].Value = item.Note;
                    ws.Cells["G" + rowNum].Value = item.IsActive ? "Đang hoạt động" : "Ngưng hoạt động";
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
                        "NguoiDung_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
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
                        DataSet ds = new SqlHelper().ExcuteQueryDataSet("p_Auth_User_Import", param);
                        if (ds.Tables.Count != 2)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = true, errorfile = ds.Tables[1].Rows[0][0].ToString() });
                        }
                        //int output = Convert.ToInt32(param[param.Count - 1].Value);
                        //if (System.IO.File.Exists(fileLocation))
                        //{
                        //    System.IO.File.Delete(fileLocation);
                        //}
                        //if (output > 0)
                        //    return Json(new { success = true, errorfile = "Loi_Nguoi_Dung.xlsx" });
                        //return Json(new { success = true });
                    }
                }
                return Json(new { success = false, message = "Không có file hoặc file không phải là Excel" });
            }
            catch (Exception e)
            {
                log.Error("Auth_User - Import - " + e.Message);
                return Json(new { success = false, message = e.Message });
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
                dict["listrole"] = new Auth_Role().GetDataForDropDownList();
                dbConn.Close();
                return View("_Auth_User", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        [HttpPost]
        public ActionResult GetUserByID(string userID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Auth_User>(userID);
                var groupUser = dbConn.Select<Auth_UserInRole>(p => p.UserID == userID);
                return Json(new { success = true, data = data, groupuser = groupUser });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        //public ActionResult getMerchantbyID(string userID)
        //{
        //    IDbConnection dbConn = new OrmliteConnection().openConn();
        //    try
        //    {
        //        var data = dbConn.Select<Auth_User_Merchant>(p => p.UserID == userID);
        //        if (data.Count == 0)
        //            return Json(new { success = false });
        //        return Json(new { success = true, data = data });

        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message });
        //    }
        //    finally { dbConn.Close(); }
        //}
        public ActionResult GetProvinceDDL(string Level)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var data = db.Select<CustomModelUS>("Select TerritoryID AS ID, TerritoryName AS NAME FROM Master_Territory Where Level='Province'");
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult GetDistrictByProvince(string id)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var data = db.Select<CustomModelUS>("Select TerritoryID AS ID, TerritoryName AS NAME FROM Master_Territory Where Level='District' AND ParentID='" + id + "'");
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        [HttpPost]
        public ActionResult AddUserToGroup(string id, string data)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                // Delete All Group of User
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Chọn người dùng trước khi thêm nhóm." });
                }
                db.DeleteById<Auth_UserInRole>(id);

                // Add User Group
                if (!string.IsNullOrEmpty(data))
                {
                    string[] arr = data.Split(',');
                    foreach (string item in arr)
                    {
                        var detail = new Auth_UserInRole();
                        detail.UserID = id;
                        detail.RoleID = int.Parse(item);
                        detail.RowCreatedAt = DateTime.Now;
                        detail.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_UserInRole>(detail);
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }


        [HttpPost]
        public ActionResult ExecPermissionData(string userID)
        {
            try
            {
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@UserID", userID));
                new SqlHelper().ExecuteQuery("p_Generate_Auth_User_AppliedFor_Detail", param);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { }
        }

        [HttpPost]
        public ActionResult ResetPasswordUser(string userID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(userID) && db.GetByIdOrDefault<Auth_User>(userID) != null)
                {
                    string pass = SqlHelper.GetMd5Hash("123456");
                    db.ExecuteSql("UPDATE [Auth_User] SET Password = '" + pass + "' WHERE [UserID] = '" + userID + "'");
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Dữ liệu trống." });
            }
            catch (Exception e)
            {
                log.Error("Auth_User - ResetPasswordUser - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult PartialEditInfor(string userID = "")
        {
            var dbConn = new OrmliteConnection().openConn();
            var dict = new Dictionary<string, object>();
            dict["asset"] = userAsset;
            dict["activestatus"] = new CommonLib().GetActiveStatus();
            dict["listrole"] = dbConn.Select<Auth_Role>(p => p.IsActive == true);
            dict["UserID"] = userID;
            return PartialView("_EditInfor", dict);
        }

        public ActionResult PartialCreateInfor()
        {
            var dbConn = new OrmliteConnection().openConn();
            var dict = new Dictionary<string, object>();
            dict["asset"] = userAsset;
            dict["activestatus"] = new CommonLib().GetActiveStatus();
            dict["listrole"] = dbConn.Select<Auth_Role>(p => p.IsActive == true);
            dict["listProvince"] = dbConn.Select<Master_Territory>("Level = {0}", "Province");
            dict["listDistrict"] = dbConn.Select<Master_Territory>("Level = {0}", "District");
            return PartialView("_CreateInfor", dict);
        }
        public ActionResult UpdateStatus(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    var detail = new Auth_User();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<Auth_User>(s => s.UserID == item).Count() > 0 && item != "administrator")
                        {
                            var success = dbConn.Update<Auth_User>(set: "IsActive = 0 ,"
                                 + "RowUpdatedAt='" + DateTime.Now + "', "
                                 + "RowUpdatedBy='" + currentUser.UserID + "'"
                                , where: "UserID = '" + item + "'") >= 1;

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
                return Json(new { success = false, message = "Không có quyền cập nhật" });
            }
        }
        public ActionResult UpdateUser(Auth_User item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
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
                            var filename = currentUser.UserID + DateTime.Now.ToString("yyyyMMdd");

                            item.AvatarPath = filename + fileexe;

                            filename = Path.Combine(Server.MapPath("~/Upload/Images/Avatar/"), filename + fileexe);
                            file.SaveAs(filename);

                            WebImage img = new WebImage(filename);
                            img.Resize(300, 300, false, false);
                            img.Save(filename);


                        }
                        else
                            item.AvatarPath = item.AvatarPath;
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                var isExist = db.GetByIdOrDefault<Auth_User>(currentUser.UserID);
                item.UserID = !string.IsNullOrEmpty(item.UserID) ? item.UserID : currentUser.UserID;
                item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                item.DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : "";
                item.FullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : "";
                item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                item.AvatarPath = !string.IsNullOrEmpty(item.AvatarPath) ? item.AvatarPath : "";
                item.ProvinceID = !string.IsNullOrEmpty(item.ProvinceID) ? item.ProvinceID : "";
                item.IsActive = isExist.IsActive;
                item.DistrictID = !string.IsNullOrEmpty(item.DistrictID) ? item.DistrictID : isExist.DistrictID;
                item.Password = isExist.Password;
                item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                //ngay thang default
                DateTime dt = DateTime.Now;
                if (!DateTime.TryParseExact(item.BirthdayString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return Json(new { success = false, message = "Định dạng ngày không đúng" });

                }
                item.Birthday = dt;
                item.RowUpdatedAt = DateTime.Now;
                item.RowUpdatedBy = currentUser.UserID;
                item.RowCreatedAt = isExist.RowCreatedAt;
                item.RowCreatedBy = isExist.RowUpdatedBy;
                db.Update<Auth_User>(item);

                var listBrandID = Request["listBrandsID"];
                var listRolesID = Request["listRolesID"];
                var success = UpdateUsersBrand(item, listBrandID);
                UpdateUsersRole(item, listRolesID);
                //if (isExist.RowCreatedBy != "system")
                //    db.Update<Auth_User>(item);
                //elses
                //    return Json(new { success = false, message = "Dữ liệu này không cho chỉnh sửa liên hệ admin để biết thêm chi tiết" });
                return Json(new { success = true, data = item });
            }
            catch (Exception e)
            {
                log.Error("Auth_User - UpdateUser - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        public ActionResult CreateUser(Auth_User item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
                {

                    var isExist = db.GetByIdOrDefault<Auth_User>(item.UserID);
                    if (isExist != null)
                        return Json(new { success = false, message = "Người dùng đã tồn tại." });

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
                            var filename = currentUser.UserID + DateTime.Now.ToString("yyyyMMdd");

                            item.AvatarPath = filename + fileexe;

                            filename = Path.Combine(Server.MapPath("~/Upload/Images/Avatar/"), filename + fileexe);
                            file.SaveAs(filename);

                            WebImage img = new WebImage(filename);
                            img.Resize(300, 300, false, false);
                            img.Save(filename);


                        }
                        else
                            item.AvatarPath = item.AvatarPath;
                    }
                    //ngay thang default
                    DateTime dt = DateTime.Now;
                    if (!DateTime.TryParseExact(item.BirthdayString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        return Json(new { success = false, message = "Định dạng ngày không đúng" });

                    }
                    item.Birthday = dt;
                    item.Password = SqlHelper.GetMd5Hash("123456");
                    item.UserID = !string.IsNullOrEmpty(item.UserID) ? item.UserID : currentUser.UserID;
                    item.Phone = !string.IsNullOrEmpty(item.Phone) ? item.Phone : "";
                    item.DisplayName = !string.IsNullOrEmpty(item.DisplayName) ? item.DisplayName : "";
                    item.FullName = !string.IsNullOrEmpty(item.FullName) ? item.FullName : "";
                    item.Email = !string.IsNullOrEmpty(item.Email) ? item.Email : "";
                    item.Note = !string.IsNullOrEmpty(item.Note) ? item.Note : "";
                    item.AvatarPath = !string.IsNullOrEmpty(item.AvatarPath) ? item.AvatarPath : "";
                    item.ProvinceID = !string.IsNullOrEmpty(item.ProvinceID) ? item.ProvinceID : "";
                    item.IsActive = true;
                    item.DistrictID = !string.IsNullOrEmpty(item.DistrictID) ? item.DistrictID : "";
                    item.Address = !string.IsNullOrEmpty(item.Address) ? item.Address : "";
                    item.RowCreatedAt = DateTime.Now;
                    item.RowCreatedBy = currentUser.UserID;
                    item.RowUpdatedAt = DateTime.Parse("1900-01-01");
                    item.RowUpdatedBy = "";
                    db.Insert<Auth_User>(item);

                    //var listBrandID = Request["listBrandsID"];
                    var listRolesID = Request["listRolesID"];
                    //var success = UpdateUsersBrand(item, listBrandID);
                    var success = UpdateUsersRole(item, listRolesID);
                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });

            }
            catch (Exception e)
            {
                log.Error("Auth_User - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
        

        public ActionResult getRolesbyUserID(string userID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var listRoleIDs = dbConn.Select<Auth_UserInRole>(@"select * from Auth_UserInRole 
                                                                    where UserID='" + userID + "'");

                return Json(new { success = true, listRoleIDs = listRoleIDs });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }
        //public ActionResult ResetPassword(string data)
        //{
        //    var dbConn = new OrmliteConnection().openConn();
        //    if (userAsset.ContainsKey("Update") && userAsset["Update"])
        //    {
        //        var result = dbConn.Update<Auth_User>(set: "Password = 'e807f1fcf82d132f9bb018ca6738a19f'", where: "UserID = '" + data + "'");
        //        if (result == 1)
        //        {
        //            var item = dbConn.SingleOrDefault<Auth_User>("UserID= {0}", data);
        //            if (item != null)
        //            {
        //                SendMail.SendEmail(item.Email, "", "Logistics - Thông tin tài khoản", "ResetPassword", item.UserID, "1234567890", item.FullName);
        //            }
        //        }
        //    }
        //    return Json(new { success = true });
        //}

        public ActionResult UpdateStatusActive(string listUserID, int action)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var item in listdata)
                {
                    if (dbConn.Select<Auth_User>(s => s.UserID == item).Count() > 0 && item != "administrator")
                    {
                        var success = dbConn.Update<Auth_User>(set: "IsActive = " + action + " ,"
                             + "RowUpdatedAt='" + DateTime.Now + "', "
                             + "RowUpdatedBy='" + currentUser.UserID + "'"
                            , where: "UserID = '" + item + "'") >= 1;
                    }
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
                    if (dbConn.Select<Auth_User>(s => s.UserID == item).Count() > 0 && item != "administrator")
                    {
                        var success = dbConn.Update<Auth_User>(set: "IsActive = 0 ,"
                             + "RowUpdatedAt='" + DateTime.Now + "', "
                             + "RowUpdatedBy='" + currentUser.UserID + "'"
                            , where: "UserID = '" + item + "'") >= 1;
                    }
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
        public ActionResult ResetPassword(string listUserID)
        {
            string[] separators = { "@@" };
            var listdata = listUserID.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                foreach (var data in listdata)
                {
                    var result = dbConn.Update<Auth_User>(set: "Password = 'e807f1fcf82d132f9bb018ca6738a19f'", where: "UserID = '" + data + "'");
                    if (result == 1)
                    {
                        var item = dbConn.SingleOrDefault<Auth_User>("UserID= {0}", data);
                        if (item != null)
                        {
                            SendMail.SendEmail(item.Email, "", "Logistics - Thông tin tài khoản", "ResetPassword", item.UserID, "1234567890", item.FullName);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }

            return Json(new { success = true });
        }
    }

}