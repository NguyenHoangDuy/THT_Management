using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.OrmLite;
using THT.Service;

namespace THT.Models
{
    public class Auth_User
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string ProvinceID { get; set; }
        public string DistrictID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Note { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsActive { get; set; }
        public DateTime RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }
        public string AvatarPath { get; set; }
        [Ignore]
        public string Roles { get; set; }
        [Ignore]
        public string RoleIDs { get; set; }
        [Ignore]
        public string ProvinceName { get; set; }
        [Ignore]
        public string DistrictName { get; set; }
        [Ignore]
        public string BirthdayString { get; set; }
        [Ignore]
        public bool IsActiveString { get; set; }

        [Ignore]
        public string Brands { get; set; }

        public DataSourceResult GetPage(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", request.Page));
            param.Add(new SqlParameter("@PageSize", request.PageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            param.Add(new SqlParameter("@Sort", CustomModel.GetSortStringFormRequest(request)));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Auth_User_Select_By_Page", param);
            var lst = new List<Auth_User>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Auth_User();
                item.UserID = !row.IsNull("UserID") ? row["UserID"].ToString() : "";
                item.FullName = !row.IsNull("FullName") ? row["FullName"].ToString() : "";
                item.DisplayName = !row.IsNull("DisplayName") ? row["DisplayName"].ToString() : "";
                item.Birthday = !row.IsNull("Birthday") ? DateTime.Parse(row["Birthday"].ToString()) : DateTime.Parse("01/01/1900");
                item.ProvinceName = !row.IsNull("ProvinceName") ? row["ProvinceName"].ToString() : "";
                item.DistrictName = !row.IsNull("DistrictName") ? row["DistrictName"].ToString() : "";
                item.Address = !row.IsNull("Address") ? row["Address"].ToString() : "";
                item.Phone = !row.IsNull("Phone") ? row["Phone"].ToString() : "";
                item.Email = !row.IsNull("Email") ? row["Email"].ToString() : "";
                item.IsActive = !row.IsNull("IsActive") ? Convert.ToBoolean(row["IsActive"]) : false;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowCreatedBy = !row.IsNull("RowCreatedBy") ? row["RowCreatedBy"].ToString() : "";
                item.RowUpdatedAt = !row.IsNull("RowUpdatedAt") ? DateTime.Parse(row["RowUpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowUpdatedBy = !row.IsNull("RowUpdatedBy") ? row["RowUpdatedBy"].ToString() : "";
                item.Roles = !row.IsNull("Roles") ? row["Roles"].ToString() : "";
                lst.Add(item);
            }
            request.Filters = null;
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;

        }
        public List<Auth_User> GetExport(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Auth_User_Select_By_Page", param);
            var lst = new List<Auth_User>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Auth_User();
                item.UserID = !row.IsNull("UserID") ? row["UserID"].ToString() : "";
                item.FullName = !row.IsNull("FullName") ? row["FullName"].ToString() : "";
                item.DisplayName = !row.IsNull("DisplayName") ? row["DisplayName"].ToString() : "";
                item.Phone = !row.IsNull("Phone") ? row["Phone"].ToString() : "";
                item.Email = !row.IsNull("Email") ? row["Email"].ToString() : "";
                item.IsActive = !row.IsNull("IsActive") ? Convert.ToBoolean(row["IsActive"]) : false;
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowCreatedBy = !row.IsNull("RowCreatedBy") ? row["RowCreatedBy"].ToString() : "";
                item.RowUpdatedAt = !row.IsNull("RowUpdatedAt") ? DateTime.Parse(row["RowUpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowUpdatedBy = !row.IsNull("RowUpdatedBy") ? row["RowUpdatedBy"].ToString() : "";
                item.Roles = !row.IsNull("Roles") ? row["Roles"].ToString() : "";
                lst.Add(item);
            }
          return lst;
        }
        public static Auth_User GetUserNameById(string userID)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.GetByIdOrDefault<Auth_User>(userID);
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
            finally { dbConn.Close(); }
        }
    }
    public class CustomModelUS
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}