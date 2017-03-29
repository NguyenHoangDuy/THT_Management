using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using THT.Service;

namespace THT.Models
{
    public class Auth_Role
    {
        [AutoIncrement]
        public int RoleID { get; set; }        
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }

        public List<Auth_Role> GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Auth_Role_Select_By_Page", param);
            var lst = new List<Auth_Role>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Auth_Role();
                item.RoleID = !row.IsNull("RoleID") ? Convert.ToInt32(row["RoleID"]) : 0;
                item.RoleName = !row.IsNull("RoleName") ? row["RoleName"].ToString() : "";                
                item.IsActive = !row.IsNull("IsActive") ? Convert.ToBoolean(row["IsActive"]) : false;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowCreatedBy = !row.IsNull("RowCreatedBy") ? row["RowCreatedBy"].ToString() : "";
                item.RowUpdatedAt = !row.IsNull("RowUpdatedAt") ? DateTime.Parse(row["RowUpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowUpdatedBy = !row.IsNull("RowUpdatedBy") ? row["RowUpdatedBy"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return lst;
        }

        public List<Auth_Role> GetDataForDropDownList()
        {
            List<SqlParameter> param = new List<SqlParameter>();
            DataTable dt = new SqlHelper().ExecuteQuery("p_Auth_Role_Select_For_DropDownList", param);
            var lst = new List<Auth_Role>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Auth_Role();
                item.RoleID = !row.IsNull("RoleID") ? Convert.ToInt32(row["RoleID"]) : 0;
                item.RoleName = !row.IsNull("RoleName") ? row["RoleName"].ToString() : "";
                lst.Add(item);
            }
            return lst;
        }        
    }
}