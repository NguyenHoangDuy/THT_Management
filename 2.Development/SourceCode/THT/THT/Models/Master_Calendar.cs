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
    public class Master_Calendar
    {
        public DateTime? Date { get; set; }
        public string Week { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Holiday { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        [Default (typeof(string),"")]
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        [Default(typeof(string), "")]
        public string RowUpdatedBy { get; set; }
        [Ignore]
        public string WeekDetail { get; set; }
       
        public DataSourceResult GetPage(int page, int pageSize, string whereCondition)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@Page", page));
            param.Add(new SqlParameter("@PageSize", pageSize));
            param.Add(new SqlParameter("@WhereCondition", whereCondition));
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Calendar_Select_By_Page", param);
            var lst = new List<Master_Calendar>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new Master_Calendar();
                item.Date = !row.IsNull("Date") ? DateTime.Parse(row["Date"].ToString()) : DateTime.Parse("01/01/1900");
                item.Week = !row.IsNull("Week") ? row["Week"].ToString() : "";
                item.Month = !row.IsNull("Month") ? row["Month"].ToString() : "";
                item.Year = !row.IsNull("Year") ? row["Year"].ToString() : "";
                item.Holiday = !row.IsNull("Holiday") ? row["Holiday"].ToString() : "";
                item.RowCreatedAt = !row.IsNull("RowCreatedAt") ? DateTime.Parse(row["RowCreatedAt"].ToString()) : DateTime.Parse("01/01/1900");
                item.RowCreatedBy = !row.IsNull("RowCreatedBy") ? row["RowCreatedBy"].ToString() : "";

                lst.Add(item);
            }
            DataSourceResult result = new DataSourceResult();
            result.Data = lst;
            result.Total = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RowCount"]) : 0;
            return result;
        }
       
        public List<string> GetAllMonth()
        {
            List<string> lst=new List<string>();

            List<SqlParameter> param = new List<SqlParameter>();
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Calendar_Select_All_Month", param);
            foreach (DataRow row in dt.Rows)
            {
                string item = "";
                item = !row.IsNull("Month") ? row["Month"].ToString() : "";

                lst.Add(item);
            }

            return lst;
        }
        public string GetCurrentMonth()
        {
            string result = "";
            DateTime todate = DateTime.Now;
            string mm = todate.Month < 10 ? "0" + todate.Month.ToString() : todate.Month.ToString();
            string yyyy = todate.Year.ToString();
            result = yyyy + "/" + mm;
            return result;
        }
        public List<Master_Calendar> GetAllWeek()
        {
            List<Master_Calendar> lst = new List<Master_Calendar>();

            List<SqlParameter> param = new List<SqlParameter>();
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Calendar_Select_All_Week", param);
            foreach (DataRow row in dt.Rows)
            {
                Master_Calendar item = new Master_Calendar();
                item.Week = !row.IsNull("Week") ? row["Week"].ToString() : "";
                item.WeekDetail = !row.IsNull("WeekDetail") ? row["WeekDetail"].ToString() : "";

                lst.Add(item);
            }

            return lst;
        }
        public string GetCurrentWeek()
        {
            string result = "";
            List<SqlParameter> param = new List<SqlParameter>();
            DataTable dt = new SqlHelper().ExecuteQuery("p_Master_Calendar_Select_Current_Week", param);
            foreach (DataRow row in dt.Rows)
            {
                result = !row.IsNull("Week") ? row["Week"].ToString() : "";
                break;
            }

            return result;
        }

    }
}