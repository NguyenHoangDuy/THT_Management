using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using THT.Models;

namespace THT.Service
{
    public class CustomController : Controller
    {
        protected Auth_User currentUser;
        protected List<Auth_Role> currentUserRole;
        protected Dictionary<string, bool> userAsset = new Dictionary<string,bool>();        
        protected Dictionary<string, bool> dictView = new Dictionary<string, bool>();
        protected List<string> lstAssetDefault = new List<string>();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (this.User.Identity.IsAuthenticated)
            {             
                IDbConnection dbConn = new OrmliteConnection().openConn();                                
                lstAssetDefault = InitAssetDefault();
                currentUser = dbConn.GetByIdOrDefault<Auth_User>(User.Identity.Name);
                currentUserRole = dbConn.SqlList<Auth_Role>("EXEC p_Auth_UserInRole_Select_By_UserID @UserID", new { UserID = User.Identity.Name });                
                string controllerName = this.GetType().Name;
                controllerName = controllerName.Substring(0, controllerName.IndexOf("Controller"));                
                var lstAsset = new List<Auth_Action>();

                // Get MenuID from controller name                                
                string menuID = dbConn.SingleOrDefault<Auth_Menu>("ControllerName = {0}", controllerName).MenuID;
                foreach (var g in currentUserRole)
                {
                    // Get List Asset                                        
                    var temp = dbConn.Select<Auth_Action>(p => p.RoleID == g.RoleID && p.MenuID == menuID);
                    if (temp.Count > 0)                        
                        lstAsset.AddRange(temp);                    
                }                
                if(lstAsset.Count == 0)
                {
                    var item = new Auth_Action();
                    item.MenuID = menuID;
                    item.Note = "";
                    item.RowCreatedAt = DateTime.Now;
                    item.RowCreatedBy = "System";
                    if (currentUser.UserID == ConfigurationManager.AppSettings["superadmin"])
                    {
                        item.RoleID = 1;
                        item.IsAllowed = true;                        
                        foreach(var asset in lstAssetDefault)
                        {
                            item.Action = asset;
                            dbConn.Insert<Auth_Action>(item);                            
                        }
                    }
                    else
                    {
                        item.RoleID = currentUserRole.FirstOrDefault().RoleID;
                        item.IsAllowed = false;                        
                        foreach (var asset in lstAssetDefault)
                        {
                            item.Action = asset;
                            dbConn.Insert<Auth_Action>(item);     
                        }
                    }
                }
                else
                {
                    foreach (var g in currentUserRole)
                    {
                        // Asset
                        var lst = lstAsset.Where(p => p.RoleID == g.RoleID).ToList();
                        foreach(var item in lst)
                        {
                            if (!userAsset.ContainsKey(item.Action))
                                userAsset.Add(item.Action, item.IsAllowed);
                            else if(item.IsAllowed)
                            {
                                userAsset.Remove(item.Action);
                                userAsset.Add(item.Action, item.IsAllowed);
                            }
                        }
                    }
                }
                // Get Asset View Menu    
                foreach (var g in currentUserRole)
                {
                    var lstView = dbConn.Select<Auth_Action>(p => p.RoleID == g.RoleID && p.Action == "View");
                    //var lstView = new Auth_Menu().GetMenuByRoleID(g.RoleID);                    
                    foreach (var i in lstView)
                    {
                        if (!dictView.ContainsKey("menu_" + i.MenuID))
                        {
                            if(i.IsAllowed)
                            {
                                dictView.Add("menu_" + i.MenuID, true);
                            }                            
                        }                            
                    }
                }
                ViewData["menuView"] = dictView;
                dbConn.Close();
            }
        }

        public List<string> InitAssetDefault()
        {
            List<string> lst = new List<string>();
            lst.Add("View");
            lst.Add("Insert");
            lst.Add("Update");
            lst.Add("Delete");
            lst.Add("Export");
            lst.Add("Import");
            return lst;
        }


        [HttpPost]
        public ActionResult GetSystemDate()
        {

            //var data = DateTime.Now;
            //return Json(new { success = true, data = data });

            IDbConnection db = new OrmliteConnection().openConn();
            try
            {
                var data = new CustomModel().GetSystemDate();
                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
            finally { db.Close(); }
        }
    }
}