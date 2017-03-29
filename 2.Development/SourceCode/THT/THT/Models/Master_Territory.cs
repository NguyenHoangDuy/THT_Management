using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using THT.Service;

namespace THT.Models
{
    public class Master_Territory
    {
        public string TerritoryID { get; set; }

        public string ParentID { get; set; }
        public string Level { get; set; }

        public string Title { get; set; }
        public string TerritoryName { get; set; }
        public string Note { get; set; }

        [Default(typeof(string), "")]
        public string Latitude { get; set; }

        [Default(typeof(string), "")]
        public string Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Default(typeof(string), "")]
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Default(typeof(string), "")]
        public string UpdatedBy { get; set; }
        //reference name
        [Ignore]
        public string ParentName { get; set; }
        [Ignore]
        public string RegionID { get; set; }
        [Ignore]
        public string ProvinceID { get; set; }
        [Ignore]
        public string DistrictID { get; set; }
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Page", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        //GET Country
        public DataSourceResult GetPageCountry(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Country", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        //GET Region
        public DataSourceResult GetPageRegion(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Region", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        //GET District
        public DataSourceResult GetPageDistrict(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_District", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        //GET Province
        public DataSourceResult GetPageProvince(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Province", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
        //GET Ward
        public DataSourceResult GetPageWard(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Ward", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }

        public List<Dictionary<string, string>> GetDistrictByProvinceID(string ProvinceID)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@ProvinceID", ProvinceID));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_District_By_Province", param);
            List<Dictionary<string, string>> newLst = new List<Dictionary<string, string>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                newDict["ID"] = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                newDict["Name"] = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                newLst.Add(newDict);
            }
            return newLst;
        }

        public List<Dictionary<string, string>> GetWardByDistrictID(string DistrictID)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@DistrictID", DistrictID));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_Ward_By_District", param);
            List<Dictionary<string, string>> newLst = new List<Dictionary<string, string>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> newDict = new Dictionary<string, string>();
                newDict["ID"] = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                newDict["Name"] = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                newLst.Add(newDict);
            }
            return newLst;
        }
        // export
        public List<Master_Territory> GetExportCountry(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Country", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            return lst;
        }
        //GET Region
        public List<Master_Territory> GetExportRegion(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Region", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            return lst;
        }
        //GET District
        public List<Master_Territory> GetExportDistrict(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_District", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            return lst;
        }
        //GET Province
        public List<Master_Territory> GetExportProvince(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Province", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";

                lst.Add(item);
            }
            return lst;
        }
        //GET Ward
        public List<Master_Territory> GetExportWard(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", 1));
            param.Add(new SqlParameter("@PageSize", 99999));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Territory_Select_By_Ward", param);
            var lst = new List<Master_Territory>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Territory();
                item.TerritoryID = !row.IsNull("TerritoryID") ? row["TerritoryID"].ToString() : "";
                item.ParentID = !row.IsNull("ParentID") ? row["ParentID"].ToString() : "";
                item.Level = !row.IsNull("Level") ? row["Level"].ToString() : "";
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";
                item.TerritoryName = !row.IsNull("TerritoryName") ? row["TerritoryName"].ToString() : "";
                item.Latitude = !row.IsNull("Latitude") ? row["Latitude"].ToString() : "";
                item.Longitude = !row.IsNull("Longitude") ? row["Longitude"].ToString() : "";
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                //reference
                item.ParentName = !row.IsNull("ParentName") ? row["ParentName"].ToString() : "";
                item.Note = !row.IsNull("Note") ? row["Note"].ToString() : "";

                lst.Add(item);
            }
            return lst;
        }



    }
}