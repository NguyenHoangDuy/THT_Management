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
    public class DocumentController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /DocumentConfig/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listCategory = dbConn.Select<Utilities_Parameters>("select ID,CategoryID as ParamID,CategoryName as Value from CategoryConfig");
                ViewBag.listSideboard = dbConn.Select<Utilities_Parameters>("select ID,SideboardID as ParamID,SideboardName as Value from Sideboard");
                ViewBag.listStatusSaved = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.StatusSavedTYPE);
                ViewBag.listStatusExpirated = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.StatusExpiratedTYPE);
                dbConn.Close();
                return View("_Document", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            //var dbConn = new OrmliteConnection().openConn();
            //log4net.Config.XmlConfigurator.Configure();
            //string whereCondition = "";
            //if (request.Filters.Count > 0)
            //{
            //    whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            //}
            //request.Filters = null;
            //var data = dbConn.Select<Document>(whereCondition).ToList();
            //return Json(data.ToDataSourceResult(request));

            var dbConn = new OrmliteConnection().openConn();
            var data = new List<Document>();
            if (request.Filters.Any())
            {
                var whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                data = dbConn.Query<Document>("[p_get_Document]", new { WhereCondition = " And " + whereCondition }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            else
            {
                data = dbConn.Query<Document>("[p_get_Document]", new { WhereCondition = "" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            //request.Filters = null;
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create(Document item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<Document>("DocumentID={0}", item.DocumentID);

                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                {
                    if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                    {

                        if (isExist != null)
                            return Json(new { success = false, message = "Mã hồ sơ đã tồn tại" });

                        var check = db.Select<Document>(p => p.No == item.No && p.Format == item.Format);
                        if (check != null)
                            if (check.Count > 0)
                                return Json(new { success = false, message = "Số đã tồn tại" });

                        string id = "";
                        var checkID = db.SingleOrDefault<Document>("SELECT DocumentID, Id FROM dbo.Document ORDER BY Id DESC");
                        if (checkID != null)
                        {
                            var nextNo = int.Parse(checkID.DocumentID.Substring(2, checkID.DocumentID.Length - 2)) + 1;
                            id = "DC" + String.Format("{0:0000000}", nextNo);
                        }
                        else
                        {
                            id = "DC0000001";
                        }

                        item.DocumentID = id;
                        item.CategoryID = !string.IsNullOrEmpty(item.CategoryID) ? item.CategoryID : "";
                        item.DocumentName = !string.IsNullOrEmpty(item.DocumentName) ? item.DocumentName : "";
                        item.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : "";
                        item.DateSave = DateTime.ParseExact(item.strDateSave, "dd/MM/yyyy",
                                          System.Globalization.CultureInfo.InvariantCulture);
                        item.Format = !string.IsNullOrEmpty(item.Format) ? item.Format : "";
                        item.ExpirationDate = DateTime.ParseExact(item.strExpirationDate, "dd/MM/yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        item.CreatedBy = currentUser.UserID;
                        item.UpdatedBy = currentUser.UserID;

                        string FolderShare = ConfigurationManager.AppSettings["FolderShare"];
                        string DocumentType = Helper.ToVietnameseWithoutAccent(item.DocumentName);
                        DocumentType = DocumentType.Replace(" ", "");

                        var Folder = @"~/FileUpload";
                        if (Folder != System.IO.Path.GetFullPath(Folder))
                        {
                            Folder = System.Web.HttpContext.Current.Server.MapPath(Folder);
                        }

                        string fileLocation = string.Format("{0}/{1}", Folder, Request.Files["FileUpload"].FileName);

                        string FileName = Request.Files["FileUpload"].FileName;
                        string FileNameNew = FileName.Replace(Path.GetExtension(FileName), "") + "_" + currentUser.UserID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(FileName);

                        if (System.IO.File.Exists(fileLocation))
                            System.IO.File.Delete(fileLocation);
                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        string strPath = FolderShare + DocumentType + "\\" + DocumentType + "_" + DateTime.Now.ToString("MMyyyy") + "\\";
                        if (!Directory.Exists("\\\\" + strPath))
                            Directory.CreateDirectory("\\\\" + strPath);

                        if (System.IO.File.Exists("\\\\" + strPath + FileNameNew))
                            System.IO.File.Delete("\\" + strPath + FileNameNew);

                        System.IO.File.Move(fileLocation, "\\\\" + strPath + FileNameNew);
                        System.IO.File.SetAttributes("\\\\" + strPath + FileNameNew, FileAttributes.ReadOnly);

                        System.Security.AccessControl.FileSystemAccessRule accessRule = new System.Security.AccessControl.FileSystemAccessRule("Everyone", System.Security.AccessControl.FileSystemRights.Read, System.Security.AccessControl.InheritanceFlags.ContainerInherit
                        | System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.None, System.Security.AccessControl.AccessControlType.Allow);

                        DirectoryInfo directoryInfo = new DirectoryInfo("\\\\" + strPath);
                        System.Security.AccessControl.DirectorySecurity directorySec = directoryInfo.GetAccessControl();

                        directorySec.AddAccessRule(accessRule);
                        directoryInfo.SetAccessControl(directorySec);

                        item.Path = strPath;
                        item.FileNameLocal = FileName;
                        item.FileNameServer = FileNameNew;
                        db.Insert<Document>(item);
                        return Json(new { success = true, DocumentID = item.DocumentID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt, });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Vui lòng chọn file" });
                    }

                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    isExist.CategoryID = !string.IsNullOrEmpty(item.CategoryID) ? item.CategoryID : "";
                    isExist.No = item.No;
                    isExist.DocumentName = !string.IsNullOrEmpty(item.DocumentName) ? item.DocumentName : "";
                    isExist.SideboardID = !string.IsNullOrEmpty(item.SideboardID) ? item.SideboardID : "";
                    isExist.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : "";
                    isExist.Format = !string.IsNullOrEmpty(item.Format) ? item.Format : "";
                    if (isExist.IsExpirated == "HH01")
                        isExist.ExpirationDate = DateTime.ParseExact(item.strExpirationDate, "dd/MM/yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
                    isExist.UpdatedAt = DateTime.Now;
                    isExist.UpdatedBy = currentUser.UserID;
                    isExist.UpdatedBy = currentUser.UserID;

                    if (Request.Files["FileUpload"] != null && Request.Files["FileUpload"].ContentLength > 0)
                    {

                        string FolderShare = ConfigurationManager.AppSettings["FolderShare"];
                        string DocumentType = Helper.ToVietnameseWithoutAccent(item.DocumentName);
                        DocumentType = DocumentType.Replace(" ", "");

                        var Folder = @"~/FileUpload";
                        if (Folder != System.IO.Path.GetFullPath(Folder))
                        {
                            Folder = System.Web.HttpContext.Current.Server.MapPath(Folder);
                        }

                        string fileLocation = string.Format("{0}/{1}", Folder, Request.Files["FileUpload"].FileName);

                        string FileName = Request.Files["FileUpload"].FileName;
                        string FileNameNew = FileName.Replace(Path.GetExtension(FileName), "") + "_" + currentUser.UserID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(FileName);

                        if (System.IO.File.Exists(fileLocation))
                            System.IO.File.Delete(fileLocation);
                        Request.Files["FileUpload"].SaveAs(fileLocation);

                        string strPath = FolderShare + DocumentType + "\\" + DocumentType + "_" + DateTime.Now.ToString("MMyyyy") + "\\";
                        if (!Directory.Exists("\\\\" + strPath))
                            Directory.CreateDirectory("\\\\" + strPath);

                        if (System.IO.File.Exists("\\\\" + strPath + FileNameNew))
                            System.IO.File.Delete("\\\\" + strPath + FileNameNew);

                        System.IO.File.Move(fileLocation, "\\\\" + strPath + FileNameNew);
                        System.IO.File.SetAttributes("\\\\" + strPath + FileNameNew, FileAttributes.ReadOnly);

                        isExist.Path = strPath;
                        isExist.FileNameLocal = FileName;
                        isExist.FileNameServer = FileNameNew;
                    }

                    var success = db.Execute(@"UPDATE Document SET CategoryID = @CategoryID, No=@No, DocumentName=@DocumentName,SideboardID=@SideboardID,
                    Description = @Description,Format=@Format,ExpirationDate=@ExpirationDate,
                    Path=@Path, FileNameLocal=@FileNameLocal,FileNameServer=@FileNameServer,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                    WHERE DocumentID = '" + item.DocumentID + "'", new
                                       {
                                           CategoryID = !string.IsNullOrEmpty(isExist.CategoryID) ? isExist.CategoryID.Trim() : "",
                                           No = !string.IsNullOrEmpty(isExist.No) ? isExist.No.Trim() : "",
                                           DocumentName = !string.IsNullOrEmpty(isExist.DocumentName) ? isExist.DocumentName.Trim() : "",
                                           SideboardID = !string.IsNullOrEmpty(isExist.SideboardID) ? isExist.SideboardID.Trim() : "",
                                           Description = !string.IsNullOrEmpty(isExist.Description) ? isExist.Description.Trim() : "",
                                           Format = !string.IsNullOrEmpty(isExist.Format) ? isExist.Format.Trim() : "",
                                           ExpirationDate = isExist.IsExpirated == "HH01" ? DateTime.ParseExact(item.strExpirationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : isExist.ExpirationDate,
                                           Path = !string.IsNullOrEmpty(isExist.Path) ? isExist.Path.Trim() : "",
                                           FileNameLocal = !string.IsNullOrEmpty(isExist.FileNameLocal) ? isExist.FileNameLocal.Trim() : "",
                                           FileNameServer = !string.IsNullOrEmpty(isExist.FileNameServer) ? isExist.FileNameServer.Trim() : "",
                                           UpdatedAt = DateTime.Now,
                                           UpdatedBy = currentUser.UserID,
                                       }) == 1;

                    //db.Update(isExist);

                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });

            }
            catch (Exception e)
            {
                log.Error("Danh mục - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        public ActionResult GetCategory(string CategoryID)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<CategoryConfig>(s => s.CategoryID == CategoryID).SingleOrDefault();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public ActionResult UpdateStatus(string data, string status)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    var detail = new Document();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<Document>(s => s.DocumentID == item).Count() > 0 && item != "administrator")
                        {
                            var success = dbConn.Update<Document>(set: "Status = " + int.Parse(status) + " ,"
                                 + "UpdatedAt='" + DateTime.Now + "', "
                                 + "UpdatedBy='" + currentUser.UserID + "'"
                                , where: "DocumentID = '" + item + "'") >= 1;

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

        public ActionResult Delete(string data)
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
                        var doc = dbConn.Select<Document>(s => s.DocumentID == item).SingleOrDefault();

                        if (System.IO.File.Exists("\\\\" + doc.Path + doc.FileNameServer))
                        {
                            System.IO.File.SetAttributes("\\\\" + doc.Path + doc.FileNameServer, FileAttributes.Normal);
                            System.IO.File.Delete("\\\\" + doc.Path + doc.FileNameServer);
                        }

                        dbConn.Delete<Document>(s => s.DocumentID == item);
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