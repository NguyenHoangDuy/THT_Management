using Kendo.Mvc.UI;
using THT.Service;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace THT.Models
{
    public class General_Notification
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Orders { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }
        public string HTMLBody { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public DataSourceResult GetPage(DataSourceRequest request, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", request.Page));
            param.Add(new SqlParameter("@PageSize", request.PageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            param.Add(new SqlParameter("@Sort", CustomModel.GetSortStringFormRequest(request)));
            DataTable dt = new SqlHelper().ExecuteQuery("p_General_Notification_All", param);
            var lst = new List<General_Notification>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new General_Notification();
                item.Id = !row.IsNull("Id") ? int.Parse(row["Id"].ToString()) : 0;
                item.Title = !row.IsNull("Title") ? row["Title"].ToString() : "";           
                item.Type = !row.IsNull("Type") ? row["Type"].ToString() : "";
                item.Status = !row.IsNull("Status") ? Convert.ToBoolean(row["Status"]) : false;
                item.Orders = !row.IsNull("Orders") ? int.Parse(row["Orders"].ToString()) : 0;
                item.HTMLBody = !row.IsNull("HTMLBody") ? row["HTMLBody"].ToString() : "";
                item.StartDate = !row.IsNull("StartDate") ? DateTime.Parse(row["StartDate"].ToString()) : DateTime.Parse("01/01/1900");
                item.EndDate = !row.IsNull("EndDate") ? DateTime.Parse(row["EndDate"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedAt = !row.IsNull("CreatedAt") ? DateTime.Parse(row["CreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.CreatedBy = !row.IsNull("CreatedBy") ? row["CreatedBy"].ToString() : "";
                item.UpdatedAt = !row.IsNull("UpdatedAt") ? DateTime.Parse(row["UpdatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.UpdatedBy = !row.IsNull("UpdatedBy") ? row["UpdatedBy"].ToString() : "";
                lst.Add(item);
            }

            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;

        }
    }
}