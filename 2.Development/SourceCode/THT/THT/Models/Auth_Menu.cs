using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using THT.Service;
using System.Data.SqlClient;

namespace THT.Models
{
    public class AuthMenuViewModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get { return true; } }
        public List<AuthMenuViewModel> items { get; set; }
        public bool @checked { get; set; }
    }
    public class Auth_Menu
    {
        public string MenuID { get; set; }
        public string ParentMenuID { get; set; }
        public string MenuName { get; set; }
        public int MenuIndex { get; set; }
        public string ControllerName { get; set; }
        public bool IsVisible { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }

        public List<Auth_Menu> GetAllMenu()
        {
            IDbConnection db = new OrmliteConnection().openConn();
            var lst = db.Select<Auth_Menu>().Where(p => p.IsVisible == true).ToList();
            db.Close();
            return lst;
        }

        public List<MenuIsAllowed> GetMenuByRoleID(int roleID)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@RoleID", roleID));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Auth_Menu_Select_By_RoleID", param);
            var lst = new List<MenuIsAllowed>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new MenuIsAllowed();
                item.MenuID = !row.IsNull("MenuID") ? row["MenuID"].ToString() : "";
                item.ParentMenuID = !row.IsNull("ParentMenuID") ? row["ParentMenuID"].ToString() : "";
                item.MenuName = !row.IsNull("MenuName") ? row["MenuName"].ToString() : "";
                item.MenuIndex = !row.IsNull("MenuIndex") ? Convert.ToInt32(row["MenuIndex"]) : 0;
                item.ControllerName = !row.IsNull("ControllerName") ? row["ControllerName"].ToString() : "";
                item.IsAllowed = !row.IsNull("IsAllowed") ? Convert.ToBoolean(row["IsAllowed"]) : false;

                lst.Add(item);
            }
            return lst;
        }
    }

    public class MenuIsAllowed
    {
        public string MenuID { get; set; }
        public string ParentMenuID { get; set; }
        public string MenuName { get; set; }
        public int MenuIndex { get; set; }
        public string ControllerName { get; set; }
        public bool IsAllowed { get; set; }
    }
}