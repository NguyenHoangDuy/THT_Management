using Kendo.Mvc;
using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using THT.Service;

namespace THT.Models
{
    public class CustomModel
    {
        //lay gio he thong
        public DateTime GetSystemDate()
        {
            DateTime dateTime=new DateTime();
            List<SqlParameter> param = new List<SqlParameter>();
            DataTable dt = new SqlHelper().ExecuteQuery("p_Lib_GetCurrentDate", param);
            List<Dictionary<string, string>> newLst = new List<Dictionary<string, string>>();
            foreach (DataRow row in dt.Rows)
            {
                dateTime = !row.IsNull("CurrentDate") ? DateTime.Parse(row["CurrentDate"].ToString()) : DateTime.Parse("01/01/1900");
                break;
            }
            return dateTime;
        }
        //lay gia tri sort tu request
        public static string GetSortStringFormRequest(DataSourceRequest request)
        {
            string result = "", sortCont = "";
            if (request.Sorts.Count > 0)
            {
                foreach (SortDescriptor itemSort in request.Sorts)
                {
                    if (itemSort.SortDirection == System.ComponentModel.ListSortDirection.Ascending)
                    {
                        sortCont += itemSort.Member + " ASC, ";
                    }
                    else
                    {
                        sortCont += itemSort.Member + " DESC, ";
                    }
                }
                sortCont = sortCont.Substring(0, sortCont.Length - 2);
                result = " ORDER By " + sortCont;
            }
            return result;
        }

        public static string ConvertToUnsign(string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

    }


}