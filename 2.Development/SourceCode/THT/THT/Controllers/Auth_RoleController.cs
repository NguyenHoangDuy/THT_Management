using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using THT.Models;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;
using Dapper;

namespace THT.Controllers
{
    [Authorize]
    [NoCache]
    public class Auth_RoleController : CustomController
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
            var data = new Auth_Role().GetPage(request.Page, request.PageSize, whereCondition);
            return Json(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (!string.IsNullOrEmpty(form["RoleName"]))
                {
                    var item = new Auth_Role();
                    item.RoleName = form["RoleName"];
                    item.IsActive = form["IsActive"] != null ? Convert.ToBoolean(form["IsActive"]) : false;
                    item.Note = !string.IsNullOrEmpty(form["Note"]) ? form["Note"] : "";
                    if (userAsset.ContainsKey("Insert") && userAsset["Insert"] &&
                        string.IsNullOrEmpty(form["RoleID"]))    // Tạo mới
                    {
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_Role>(item);
                        long lastID = db.GetLastInsertId();
                        if (lastID > 0)
                        {
                            // Thêm Role vào Auth_Action
                            //db.ExecuteSql("EXEC p_Auth_Role_GenerateAction_By_RoleID " + lastID + "," + currentUser.UserID);
                        }
                        return Json(new { success = true, insert = true, RoleID = lastID, createdat = item.RowCreatedAt, createdby = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Insert") && userAsset["Insert"] &&
                            Convert.ToInt32(form["RoleID"]) > 0 &&
                            Convert.ToInt32(form["IsCopy"]) == 1)  // Sao chép
                    {
                        item.RoleID = Convert.ToInt32(form["RoleID"]);
                        item.RowCreatedAt = DateTime.Now;
                        item.RowCreatedBy = currentUser.UserID;
                        db.Insert<Auth_Role>(item);
                        long lastID = db.GetLastInsertId();
                        if (lastID > 0)
                        {
                            // Sao chép Action RoleID đã chọn vào RoleID vừa tạo                            
                            db.ExecuteSql("p_Auth_Role_CopyAction_By_RoleID " + item.RoleID + "," + lastID + "," + currentUser.UserID);
                        }
                        return Json(new { success = true, insert = true, RoleID = lastID, createdat = item.RowCreatedAt, createdby = item.RowCreatedBy });
                    }
                    else if (userAsset.ContainsKey("Update") && userAsset["Update"] &&
                            Convert.ToInt32(form["RoleID"]) > 0)    // Cập nhật
                    {
                        item.RoleID = Convert.ToInt32(form["RoleID"]);
                        //item.RowCreatedAt = DateTime.Parse(form["RowCreatedAt"]);
                        //item.RowCreatedBy = form["RowCreatedBy"];
                        item.RowUpdatedAt = DateTime.Now;
                        item.RowUpdatedBy = currentUser.UserID;
                        if (item.RowCreatedBy != "system")
                        {
                            //db.Update<Auth_Role>(item);
                            var success = db.Execute(@"UPDATE Auth_Role SET RoleName = @RoleName,
                                        RowUpdatedAt = @RowUpdatedAt, RowUpdatedBy = @RowUpdatedBy
                                        WHERE RoleID = '" + item.RoleID + "'", new
                                                          {
                                                              RoleName = !string.IsNullOrEmpty(item.RoleName) ? item.RoleName.Trim() : "",
                                                              RowUpdatedAt = DateTime.Now,
                                                              RowUpdatedBy = currentUser.UserID,
                                                          }) == 1;
                        }



                        return Json(new { success = true, RoleID = item.RoleID });
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
                log.Error("Auth_Role - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        //========================================== Tab Phân quyền chức năng =================================

        public ActionResult ReadAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid)
        {
            var data = new List<Auth_Action>();
            if (roleid > 0 && !string.IsNullOrEmpty(menuid))
            {
                IDbConnection db = new OrmliteConnection().openConn();
                data = db.Select<Auth_Action>(p => p.RoleID == roleid && p.MenuID == menuid &&
                                                    p.Action != "View" &&
                                                    p.Action != "Insert" &&
                                                    p.Action != "Update" &&
                                                    p.Action != "Delete" &&
                                                    p.Action != "Export" &&
                                                    p.Action != "Import");
                db.Close();
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadRole([DataSourceRequest]DataSourceRequest request, string roleid)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            var select = "";
            if (string.IsNullOrEmpty(roleid))
            {
                select = @"select 
                                ID
		                       ,MenuID
		                       ,MenuName
                               ,ParentMenuID
	                           ,0 as [View]
	                           ,0 as [Insert]
	                           ,0 as [Update]
		                       ,0 as [Delete]
	                           ,0 as [Import]
	                           ,0 as [Export] 
                        FROM Auth_Menu a 
                        Order by ID";
            }
            else
            {
                select = @"SELECT 
                               a.ID
                              ,a.MenuID
                              ,a.MenuName
							  ,b.ParentMenuID
                              ,[View]
                              ,[Update]
                              ,[Delete]
                              ,[Insert]
                              ,[Export]
                              ,[Import]
                            FROM vw_List_RoleByID a join Auth_Menu b on a.MenuID=b.MenuID
                            WHERE RoleID = '" + roleid + "'" +
                            "ORDER BY RoleID,ID";

            }

            var data = db.Select<Asset>(select).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid, [Bind(Prefix = "models")]IEnumerable<Auth_Action> lst)
        {
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                IDbConnection db = new OrmliteConnection().openConn();
                foreach (var item in lst)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.Action))
                        {
                            ModelState.AddModelError("er", "Xin nhập quyền");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        var isExist = db.SingleOrDefault<Auth_Action>("RoleID = {0} AND MenuID = {1} AND Action = {2}", roleid, menuid, item.Action);
                        if (isExist != null)
                        {
                            ModelState.AddModelError("er", "Quyền đã tồn tại");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        try
                        {
                            item.RoleID = roleid;
                            item.MenuID = menuid;
                            item.Note = "";
                            item.RowCreatedAt = DateTime.Now;
                            item.RowCreatedBy = currentUser.UserID;
                            db.Insert<Auth_Action>(item);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("er", ex.Message);
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                db.Close();
                return Json(lst.ToDataSourceResult(request));
            }
            else
            {
                ModelState.AddModelError("er", "Bạn không có quyền tạo mới");
                return Json(lst.ToDataSourceResult(request, ModelState));
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateAction([DataSourceRequest]DataSourceRequest request, int roleid, string menuid, [Bind(Prefix = "models")]IEnumerable<Auth_Action> lst)
        {
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                IDbConnection db = new OrmliteConnection().openConn();
                foreach (var item in lst)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrEmpty(item.Action))
                        {
                            ModelState.AddModelError("er", "Xin nhập quyền");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                        var isExist = db.SingleOrDefault<Auth_Action>("RoleID = {0} AND MenuID = {1} AND Action = {2}", roleid, menuid, item.Action);
                        if (isExist != null)
                        {
                            try
                            {
                                item.RoleID = roleid;
                                item.MenuID = menuid;
                                item.Note = "";
                                item.RowCreatedAt = isExist.RowCreatedAt;
                                item.RowCreatedBy = isExist.RowCreatedBy;
                                item.RowUpdatedAt = DateTime.Now;
                                item.RowUpdatedBy = currentUser.UserID;
                                db.Update<Auth_Action>(item, p => p.RoleID == roleid && p.MenuID == menuid && p.Action == item.Action);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("er", ex.Message);
                                return Json(lst.ToDataSourceResult(request, ModelState));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("er", "Quyền không tồn tại");
                            return Json(lst.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                db.Close();
                return Json(lst.ToDataSourceResult(request));
            }
            else
            {
                ModelState.AddModelError("er", "Bạn không có quyền cập nhật");
                return Json(lst.ToDataSourceResult(request, ModelState));
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
                dict["user"] = dbConn.Select<Auth_User>(p => p.IsActive == true);
                dict["function"] = dbConn.Select<Utilities_Parameters>("select ID, MenuID as ParamID,MenuName as Value from Auth_Menu where IsVisible=1 and ParentMenuID='' and MenuID <> 'Home'");
                dbConn.Close();
                return View("_Auth_Role", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        [HttpPost]
        public ActionResult GetByID(int id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Auth_Role>(id);
                var listUserRole = dbConn.Select<Auth_UserInRole>(p => p.RoleID == id);
                return Json(new { success = true, data = data, listuser = listUserRole });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        [HttpPost]
        public ActionResult GetMenuID(int id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<Auth_Menu>(@"select distinct 1 as ID, ParentMenuID as MenuID from Auth_Action a join Auth_Menu b on 
                                                    a.MenuID=b.MenuID where RoleID='" + id + "'").ToList();
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        [HttpPost]
        public ActionResult AddUserToGroup(int id, string data)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                // Delete All User in Role
                db.Delete<Auth_UserInRole>(p => p.RoleID == id);

                // Add User Role
                if (!string.IsNullOrEmpty(data))
                {
                    string[] arr = data.Split(',');
                    foreach (string item in arr)
                    {
                        var detail = new Auth_UserInRole();
                        detail.UserID = item;
                        detail.RoleID = id;
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

        //public JsonResult GetMenu([DataSourceRequest] DataSourceRequest request)
        //{
        //    IDbConnection db = new OrmliteConnection().openConn();
        //    try
        //    {
        //        var listMenu = db.Select<Auth_Menu>(p => p.IsVisible == true).OrderBy(p => p.MenuIndex).ToList();
        //        DataSourceResult dsr = new DataSourceResult();
        //        dsr.Data = listMenu;
        //        return Json(dsr, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, message = e.Message });
        //    }
        //    finally { db.Close(); }
        //}

        [HttpPost]
        public JsonResult GetMenu(string action, int roleID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                //select list menu cha
                List<Auth_Menu> lstFirstMenu = db.Select<Auth_Menu>("IsVisible = 1 AND ParentMenuID ='' AND MenuID <> 'Home'").OrderBy(p => p.MenuIndex).ToList();
                List<Auth_Menu> allAuthMenu = db.Select<Auth_Menu>("IsVisible = 1  AND MenuID <> 'Home'").OrderBy(p => p.MenuIndex).ToList();

                var listAction = new List<Auth_Menu>();
                if (!string.IsNullOrEmpty(action))
                {
                    listAction = db.SqlList<Auth_Menu>("p_Auth_Menu_Select_By_Action '" + action + "', " + roleID);
                }

                List<AuthMenuViewModel> lstMenuView = new List<AuthMenuViewModel>();
                foreach (Auth_Menu der in lstFirstMenu)
                {
                    AuthMenuViewModel node = new AuthMenuViewModel();
                    node.id = der.MenuID;
                    node.text = der.MenuName;
                    node.items = new List<AuthMenuViewModel>();
                    AddChidrenNode(ref node, allAuthMenu, listAction);
                    lstMenuView.Add(node);
                }
                return Json(new { success = true, Data = lstMenuView });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }

        private void AddChidrenNode(ref AuthMenuViewModel node, List<Auth_Menu> allAuthMenu, List<Auth_Menu> listAction)
        {
            try
            {
                string parentID = node.id;
                var obj = listAction.Find(p => p.MenuID == parentID);
                if (obj != null)
                {
                    node.@checked = true;
                }
                List<Auth_Menu> lstChildMenu = allAuthMenu.Where(p => p.ParentMenuID == parentID).ToList();// db.Select<Auth_Menu>("IsVisible = {0} AND ParentMenuID ={1}", true, parentID).OrderBy(p => p.MenuIndex).ToList();//Danh sách menu con của parentID 
                foreach (Auth_Menu der in lstChildMenu)
                {
                    AuthMenuViewModel n = new AuthMenuViewModel();
                    n.id = der.MenuID;
                    n.text = der.MenuName;

                    var check = listAction.Find(p => p.MenuID == der.MenuID);
                    if (check != null)
                    {
                        n.@checked = true;
                    }

                    n.items = new List<AuthMenuViewModel>();
                    AddChidrenNode(ref n, allAuthMenu, listAction);
                    node.items.Add(n);
                }
            }
            catch (Exception)
            {

            }
        }

        [HttpPost]
        public ActionResult SavePermission(int RoleID, string data, string FunctionID)
        {
            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                if (string.IsNullOrEmpty(FunctionID))
                    db.Delete<Auth_Action>("RoleID={0}", RoleID);
                else
                    db.Execute(@"delete a from Auth_Action a
                                        join Auth_Menu b
                                        on a.MenuID=b.MenuID
                                        where b.ParentMenuID in (" + FunctionID + ") and RoleID='" + RoleID + "'");

                if (string.IsNullOrEmpty(data))
                {
                    //db.Delete<Auth_Action>("RoleID={0}", RoleID);
                    //return Json(new { success = true});
                }
                else
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    //db.Delete<Auth_Action>("RoleID={0}", RoleID);
                    foreach (var item in listdata)
                    {
                        string[] sep = { "_TT" };
                        string[] listAction = item.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        var newAction = new Auth_Action();
                        newAction.RoleID = RoleID;
                        newAction.MenuID = listAction[0];
                        newAction.Action = listAction[1];
                        newAction.IsAllowed = true;
                        newAction.RowCreatedAt = DateTime.Now;
                        newAction.RowCreatedBy = currentUser.UserID;
                        newAction.RowUpdatedBy = "";
                        db.Insert(newAction);

                        Auth_Menu menu = db.Select<Auth_Menu>(s => s.MenuID == newAction.MenuID).FirstOrDefault();
                        var parentaction = db.Select<Auth_Action>(s => s.MenuID == menu.ParentMenuID && s.Action == newAction.Action && s.RoleID == RoleID);
                        if (parentaction.Count == 0 && !string.IsNullOrEmpty(menu.ParentMenuID.Trim()))
                        {
                            newAction = new Auth_Action();
                            newAction.RoleID = RoleID;
                            newAction.MenuID = menu.ParentMenuID;
                            newAction.Action = listAction[1];
                            newAction.IsAllowed = true;
                            newAction.RowCreatedAt = DateTime.Now;
                            newAction.RowCreatedBy = currentUser.UserID;
                            newAction.RowUpdatedBy = "";
                            db.Insert(newAction);
                        }
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception e) { return Json(new { success = false, message = e.Message }); }
            finally { db.Close(); }
        }
        public ActionResult UpdateStatus(string data, int status)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Update") && userAsset["Update"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    var detail = new Auth_Role();
                    foreach (var item in listdata)
                    {
                        if (dbConn.Select<Auth_Role>(s => s.RoleID == int.Parse(item)).Count() > 0 && int.Parse(item) != 1)
                        {
                            var success = dbConn.Update<Auth_Role>(set: "IsActive = " + status + " ,"
                                 + "RowUpdatedAt='" + DateTime.Now + "', "
                                 + "RowUpdatedBy='" + currentUser.UserID + "'"
                                , where: "RoleID = '" + item + "'") >= 1;

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
    }
}