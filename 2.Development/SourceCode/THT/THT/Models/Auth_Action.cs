using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using THT.Service;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace THT.Models
{
    public class Auth_Action
    {        
        public int RoleID { get; set; }
        public string MenuID { get; set; }
        public string Action { get; set; }        
        public bool IsAllowed { get; set; }
        public string Note { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        [Default(typeof(string), "")]
        public string RowCreatedBy { get; set; }        
        public DateTime? RowUpdatedAt { get; set; }
        [Default(typeof(string), "")]
        public string RowUpdatedBy { get; set; }


        public static bool AssetResult(Dictionary<string, bool> asset, string key)
        {
            return (asset.ContainsKey(key) && asset[key]) || !asset.ContainsKey(key);
        }

        //public List<Auth_Action> GetDisplayName()
        //{
        //    string strSQL = "SELECT	DisplayName FROM [AssetDetail] WHERE DisplayName NOT IN ('Error','Asset') GROUP BY DisplayName";
        //    DataTable dt = new SqlHelper().SelectQuery(strSQL);
        //    List<Auth_Action> lst = new List<Auth_Action>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        var item = new Auth_Action();
        //        item.MenuID = row["DisplayName"] != null ? row["DisplayName"].ToString() : "";
        //        lst.Add(item);
        //    }
        //    return lst;
        //}

        //public List<Auth_Action> GetDisplayNameByGroupID(int groupID)
        //{
        //    string strSQL = "SELECT	GroupID,DisplayName FROM [AssetDetail] WHERE GroupID = " + groupID + " AND DisplayName NOT IN ('Error','Asset') GROUP BY GroupID,DisplayName";
        //    DataTable dt = new SqlHelper().SelectQuery(strSQL);
        //    List<Auth_Action> lst = new List<Auth_Action>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        var item = new Auth_Action();
        //        item.RoleID = row["GroupID"] != null ? Convert.ToInt32(row["GroupID"]) : 0;
        //        item.MenuID = row["DisplayName"] != null ? row["DisplayName"].ToString() : "";
        //        lst.Add(item);
        //    }
        //    return lst;
        //}

        //public List<Auth_Action> GetAssetDetailByGroupID(int groupID)
        //{
        //    List<SqlParameter> param = new List<SqlParameter>();
        //    param.Add(new SqlParameter("@GroupID", groupID));
        //    DataTable dt = new SqlHelper().ExecuteQuery("p_SelectAssetDetailByGroupID", param);
        //    var lst = new List<Auth_Action>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        var item = new Auth_Action();
        //        item.RoleID = row["GroupID"] != null ? Convert.ToInt32(row["GroupID"]) : 0;
        //        item.MenuID = row["DisplayName"] != null ? row["DisplayName"].ToString() : "";
        //        item.Action = row["Asset"] != null ? row["Asset"].ToString() : "";
        //        item.IsAllowed = row["Value"] != null ? Convert.ToBoolean(row["Value"]) : false;
        //        lst.Add(item);
        //    }
        //    return lst;
        //}

        //public List<Auth_Action> GetAssetDetailByGroupIDAndDisplayName(int groupID, string displayName)
        //{
        //    List<SqlParameter> param = new List<SqlParameter>();
        //    param.Add(new SqlParameter("@GroupID", groupID));
        //    param.Add(new SqlParameter("@DisplayName", displayName));
        //    DataTable dt = new SqlHelper().ExecuteQuery("p_SelectAssetDetailByGroupIDAndDisplayName", param);
        //    var lst = new List<Auth_Action>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        var item = new Auth_Action();
        //        item.RoleID = row["GroupID"] != null ? Convert.ToInt32(row["GroupID"]) : 0;
        //        item.Action = row["Asset"] != null ? row["Asset"].ToString() : "";
        //        item.Note = row["Description"] != null ? row["Description"].ToString() : "";
        //        item.IsAllowed = row["Value"] != null ? Convert.ToBoolean(row["Value"]) : false;
        //        lst.Add(item);
        //    }
        //    return lst;
        //}

        //public bool CheckAssetExist(int groupID, string displayName, string asset)
        //{
        //    List<SqlParameter> param = new List<SqlParameter>();
        //    param.Add(new SqlParameter("@GroupID", groupID));
        //    param.Add(new SqlParameter("@DisplayName", displayName));
        //    param.Add(new SqlParameter("@Asset", asset));
        //    DataTable dt = new SqlHelper().ExecuteQuery("p_CheckAssetExist", param);
        //    return dt.Rows.Count > 0;
        //}

        public int Save()
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@RoleID", this.RoleID));
            param.Add(new SqlParameter("@MenuUD", this.MenuID));
            param.Add(new SqlParameter("@Action", this.Action));
            param.Add(new SqlParameter("@Note", this.Note));
            param.Add(new SqlParameter("@IsAllowed", this.IsAllowed));
            //param.Add(new SqlParameter("@CreatedAt", this.RowCreatedAt));
            //param.Add(new SqlParameter("@CreatedBy", this.RowCreatedBy));
            return new SqlHelper().ExecuteNoneQuery("p_Auth_Action_Save", param);
        }

        //public int Update()
        //{
        //    List<SqlParameter> param = new List<SqlParameter>();
        //    param.Add(new SqlParameter("@GroupID", this.RoleID));
        //    param.Add(new SqlParameter("@DisplayName", this.MenuID));
        //    param.Add(new SqlParameter("@Asset", this.Action));
        //    param.Add(new SqlParameter("@Description", this.Note));
        //    param.Add(new SqlParameter("@Value", this.IsAllowed));
        //    param.Add(new SqlParameter("@UpdatedAt", this.RowUpdatedAt));
        //    param.Add(new SqlParameter("@UpdatedBy", this.RowUpdatedBy));
        //    return new SqlHelper().ExecuteNoneQuery("p_UpdateAssetDetail", param);
        //}
    }
}