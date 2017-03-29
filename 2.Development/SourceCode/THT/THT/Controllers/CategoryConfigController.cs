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
    public class CategoryConfigController : CustomController
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
                ViewBag.listIsSaved = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.DocumentTYPE);
                ViewBag.listIsExpirated = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.IsExpiratedTYPE);
                ViewBag.listSideboard = dbConn.Select<Utilities_Parameters>("select ID,SideboardID as ParamID,SideboardName as Value from Sideboard");
                ViewBag.listGroupDocument = dbConn.Select<Utilities_Parameters>("select ID,GroupID as ParamID,GroupName as Value from GroupDocument");
                
                dbConn.Close();
                return View("_CategoryConfig", dict);
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
            //var data = dbConn.Select<CategoryConfig>(whereCondition).ToList();
            //return Json(data.ToDataSourceResult(request));

            var dbConn = new OrmliteConnection().openConn();
            var data = new List<CategoryConfig>();
            if (request.Filters.Any())
            {
                var whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
                data = dbConn.Query<CategoryConfig>("[p_Get_CategoryConfig]", new { WhereCondition = " And " + whereCondition }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            else
            {
                data = dbConn.Query<CategoryConfig>("[p_Get_CategoryConfig]", new { WhereCondition = "" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            //request.Filters = null;
            return Json(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetGroupDocumentByCategory(string CategoryID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = new List<GroupCategory>();
                //data = dbConn.Select<GroupCategory>("select a.*,b.GroupName from GroupCategory a join GroupDocument b on a.GroupID=b.GroupID where a.CategoryID='" + CategoryID + "'");

                data = dbConn.Select<GroupCategory>(s => s.CategoryID == CategoryID);
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        public ActionResult Create(CategoryConfig item)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var isExist = db.SingleOrDefault<CategoryConfig>("CategoryID={0}", item.CategoryID);

                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && item.CreatedAt == null && item.CreatedBy == null)
                {
                    if (isExist != null)
                        return Json(new { success = false, message = "Mã cấu hình đã tồn tại" });
                    string id = "";
                    var checkID = db.SingleOrDefault<CategoryConfig>("SELECT CategoryID, Id FROM dbo.CategoryConfig ORDER BY Id DESC");
                    if (checkID != null)
                    {
                        var nextNo = int.Parse(checkID.CategoryID.Substring(2, checkID.CategoryID.Length - 2)) + 1;
                        id = "CA" + String.Format("{0:00000}", nextNo);
                    }
                    else
                    {
                        id = "CA00001";
                    }

                    item.CategoryID = id;
                    item.CategoryName = !string.IsNullOrEmpty(item.CategoryName) ? item.CategoryName : "";
                    item.IsSaved = !string.IsNullOrEmpty(item.IsSaved) ? item.IsSaved : "";
                    item.IsExpirated = !string.IsNullOrEmpty(item.IsExpirated) ? item.IsExpirated : "";
                    item.SideboardID = !string.IsNullOrEmpty(item.SideboardID) ? item.SideboardID : "";
                    item.Format = !string.IsNullOrEmpty(item.Format) ? item.Format : "";
                    item.CreatedAt = DateTime.Now;
                    item.UpdatedAt = DateTime.Now;
                    item.CreatedBy = currentUser.UserID;
                    item.UpdatedBy = currentUser.UserID;
                    db.Insert<CategoryConfig>(item);

                    string[] separators = { "," };
                    var listdata = Request["listGroupDocument"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    db.Delete<GroupCategory>(s => s.CategoryID == item.CategoryID);

                    foreach(var i in listdata)
                    {
                        GroupCategory newdata = new GroupCategory();
                        newdata.CategoryID = item.CategoryID;
                        newdata.GroupID = i;
                        newdata.CreatedAt = DateTime.Now;
                        newdata.UpdatedAt = DateTime.Now;
                        newdata.CreatedBy = currentUser.UserID;
                        newdata.UpdatedBy = currentUser.UserID;
                        db.Insert<GroupCategory>(newdata);
                    }

                    return Json(new { success = true, CategoryID = item.CategoryID, CreatedBy = item.CreatedBy, CreatedAt = item.CreatedAt, });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {
                    item.CategoryName = !string.IsNullOrEmpty(item.CategoryName) ? item.CategoryName : "";
                    item.IsSaved = !string.IsNullOrEmpty(item.IsSaved) ? item.IsSaved : "";
                    item.IsExpirated = !string.IsNullOrEmpty(item.IsExpirated) ? item.IsExpirated : "";
                    item.SideboardID = !string.IsNullOrEmpty(item.SideboardID) ? item.SideboardID : "";
                    item.Format = !string.IsNullOrEmpty(item.Format) ? item.Format : "";
                    item.CreatedBy = isExist.CreatedBy;
                    item.CreatedAt = isExist.CreatedAt;
                    item.UpdatedAt = DateTime.Now;
                    item.UpdatedBy = currentUser.UserID;


                    var success = db.Execute(@"UPDATE CategoryConfig SET CategoryName = @CategoryName,
                                            IsSaved = @IsSaved,IsExpirated = @IsExpirated,SideboardID = @SideboardID,Format = @Format,
                                            UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                                            WHERE CategoryID = '" + item.CategoryID + "'", new
                                                          {
                                                              CategoryName = !string.IsNullOrEmpty(item.CategoryName) ? item.CategoryName.Trim() : "",
                                                              IsSaved = !string.IsNullOrEmpty(item.IsSaved) ? item.IsSaved.Trim() : "",
                                                              IsExpirated = !string.IsNullOrEmpty(item.IsExpirated) ? item.IsExpirated.Trim() : "",
                                                              SideboardID = !string.IsNullOrEmpty(item.SideboardID) ? item.SideboardID.Trim() : "",
                                                              Format = !string.IsNullOrEmpty(item.Format) ? item.Format.Trim() : "",
                                                              UpdatedAt = DateTime.Now,
                                                              UpdatedBy = currentUser.UserID,
                                                          }) == 1;

                    //db.Update(item);
                    string[] separators = { "," };
                    var listdata = Request["listGroupDocument"].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    db.Delete<GroupCategory>(s => s.CategoryID == item.CategoryID);

                    foreach (var i in listdata)
                    {
                        GroupCategory newdata = new GroupCategory();
                        newdata.CategoryID = item.CategoryID;
                        newdata.GroupID = i;
                        db.Insert<GroupCategory>(newdata);
                    }
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

        public ActionResult UpdateStatus(string data, string status)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    var detail = new CategoryConfig();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<CategoryConfig>(s => s.CategoryID == item).Count() > 0 && item != "administrator")
                        {
                            var success = dbConn.Update<CategoryConfig>(set: "Status = " + int.Parse(status) + " ,"
                                 + "UpdatedAt='" + DateTime.Now + "', "
                                 + "UpdatedBy='" + currentUser.UserID + "'"
                                , where: "CategoryID = '" + item + "'") >= 1;

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
                    string[] separators = { "@@@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            dbConn.Delete<CategoryConfig>(s => s.CategoryID == item);
                            dbConn.Delete<GroupCategory>(s => s.CategoryID == item);
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