using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using THT.Service;
using System.Data;
using ServiceStack.OrmLite;
using System.Configuration;
using System.Data.SqlClient;

namespace THT.Helpers
{
    public static class Helper
    {
        public static string ToVietnameseWithoutAccent(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = text.Normalize(System.Text.NormalizationForm.FormD);
                string result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                return result;
            }
            return text;
        }

        public static DateTime GetServerTime()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                DataTable dt = new DataTable();
                SqlCommand _command = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();

                _command.Connection = Connection;
                _command.CommandTimeout = 60000;
                _command.CommandType = CommandType.Text;
                _command.CommandText = "SELECT GETDATE()";
                da.SelectCommand = _command;
                dt.Dispose();
                dt = new System.Data.DataTable("tbResult");
                da.Fill(dt);
                Connection.Close();
                return DateTime.Parse(dt.Rows[0][0].ToString());
            }
        }
    }
}