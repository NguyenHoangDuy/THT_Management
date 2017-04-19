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
    public class SideboardController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Sideboard/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                return View("_Sideboard", dict);
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
                request.Filters = null;
                var data = dbConn.Select<Sideboard>(whereCondition).ToList();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Sideboard> list)
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
                                if (string.IsNullOrEmpty(item.SideboardName))
                                {
                                    ModelState.AddModelError("", "Vui lòng nhập tên tủ");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }
                                string id = "";
                                var checkID = db.SingleOrDefault<Sideboard>("SELECT SideboardID, Id FROM dbo.Sideboard ORDER BY Id DESC");
                                if (checkID != null)
                                {
                                    var nextNo = int.Parse(checkID.SideboardID.Substring(2, checkID.SideboardID.Length - 2)) + 1;
                                    id = "SB" + String.Format("{0:00000}", nextNo);
                                }
                                else
                                {
                                    id = "SB00001";
                                }

                                item.SideboardID = id;
                                item.SideboardName = !string.IsNullOrEmpty(item.SideboardName) ? item.SideboardName : "";
                                item.CreatedAt = DateTime.Now;
                                item.UpdatedAt = DateTime.Now;
                                item.CreatedBy = currentUser.UserID;
                                item.UpdatedBy = currentUser.UserID;
                                db.Insert<Sideboard>(item);
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

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<Sideboard> list)
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

                            if (string.IsNullOrEmpty(item.SideboardName.Trim()))
                            {
                                ModelState.AddModelError("", "Vui lòng nhập tên tủ");
                                return Json(list.ToDataSourceResult(request, ModelState));
                            }

                            if (dbConn.Select<Sideboard>(s => s.ID == item.ID).Count() > 0)
                            {
                                //dbConn.Update<Sideboard>(set: "SideboardName = N'" + item.SideboardName + "' ,UpdatedAt = '" + DateTime.Now + "', UpdatedBy ='" +
                                //    currentUser.UserID + "'", where: "ID = " + item.ID);

                                var success = dbConn.Execute(@"UPDATE Sideboard SET SideboardName = @SideboardName,
                                            UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                                            WHERE ID = '" + item.ID + "'", new
                                          {
                                              SideboardName = !string.IsNullOrEmpty(item.SideboardName) ? item.SideboardName.Trim() : "",
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
                    int i;
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            if (CheckIDInDocument(item))
                            {
                                return Json(new { success = false, message = "Không thể xóa. Tủ này đang lưu trữ văn bản." });
                            }
                            dbConn.Delete<Sideboard>(s => s.SideboardID == item);
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

        private bool CheckIDInDocument(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();

            var data = dbConn.Select<Document>(s => s.SideboardID == id);

            if (data != null)
            {
                if (data.Count > 0)
                    return true;
            }

            return false;
        }

    }
}