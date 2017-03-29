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
    public class GroupDocumentController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /GroupDocument/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return View("_GroupDocument", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string str = "SELECT * FROM GroupDocument";
            var data = dbConn.Select<GroupDocument>(str).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<GroupDocument> list)
        {
            var db = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                try
                {
                    using (var dbTrans = db.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        if (list != null && ModelState.IsValid)
                        {
                            foreach (var item in list)
                            {
                                if (string.IsNullOrEmpty(item.GroupName))
                                {
                                    ModelState.AddModelError("", "Vui lòng nhập tên tủ");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }
                                string id = "";
                                var checkID = db.SingleOrDefault<GroupDocument>("SELECT GroupID, ID FROM dbo.GroupDocument ORDER BY ID DESC");
                                if (checkID != null)
                                {
                                    var nextNo = int.Parse(checkID.GroupID.Substring(2, checkID.GroupID.Length - 2)) + 1;
                                    id = "GD" + String.Format("{0:00000}", nextNo);
                                }
                                else
                                {
                                    id = "GD00001";
                                }

                                item.GroupID = id;
                                item.GroupName = !string.IsNullOrEmpty(item.GroupName) ? item.GroupName : "";
                                item.CreatedAt = DateTime.Now;
                                item.UpdatedAt = DateTime.Now;
                                item.CreatedBy = currentUser.UserID;
                                item.UpdatedBy = currentUser.UserID;
                                db.Insert<GroupDocument>(item);
                            }
                            dbTrans.Commit();
                            return Json(new { success = true });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return Json(list.ToDataSourceResult(request, ModelState));
                }
                return Json(new { sussess = true });
            }
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                return Json(list.ToDataSourceResult(request, ModelState));
            }
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<GroupDocument> list)
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

                            if (string.IsNullOrEmpty(item.GroupName.Trim()))
                            {
                                ModelState.AddModelError("", "Vui lòng nhập tên");
                                return Json(list.ToDataSourceResult(request, ModelState));
                            }

                            if (dbConn.Select<GroupDocument>(s => s.ID == item.ID).Count() > 0)
                            {

                                var success = dbConn.Execute(@"UPDATE GroupDocument SET GroupName = @GroupName,
                                            UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                                            WHERE ID = '" + item.ID + "'", new
                                                          {
                                                              GroupName = !string.IsNullOrEmpty(item.GroupName) ? item.GroupName.Trim() : "",
                                                              UpdatedAt = DateTime.Now,
                                                              UpdatedBy = currentUser.UserID,
                                                          }) == 1;
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
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                return Json(list.ToDataSourceResult(request, ModelState));
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
                    //int[] ids = data.Split(new char[] { ',' }).Select(s => int.Parse(s)).ToArray();
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            dbConn.Delete<GroupDocument>(s => s.GroupID == item);
                            dbConn.Delete<GroupCategory>(s => s.GroupID == item);
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