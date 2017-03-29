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
    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordModel
    {
        public string UserIDChange { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string RepeatNewPass { get; set; }

        public bool GetUserByUserIDAndPassword(string userID, string password)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@UserID", userID));
            param.Add(new SqlParameter("@Password", password));
            DataTable dt = new SqlHelper().ExecuteQuery("p_SelectUserByUserIDAndPassword", param);
            return dt.Rows.Count > 0;
        }

        public int ChangePassword()
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@UserID", this.UserIDChange));
            param.Add(new SqlParameter("@Password", this.RepeatNewPass));            
            return new SqlHelper().ExecuteNoneQuery("p_ChangePassword", param);
        }
    }
}