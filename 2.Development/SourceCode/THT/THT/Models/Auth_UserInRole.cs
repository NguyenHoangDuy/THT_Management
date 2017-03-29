using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using THT.Service;

namespace THT.Models
{
    public class Auth_UserInRole
    {
        public string UserID { get; set; }
        public int RoleID { get; set; }
        public DateTime? RowCreatedAt { get; set; }
        public string RowCreatedBy { get; set; }
        public DateTime? RowUpdatedAt { get; set; }
        public string RowUpdatedBy { get; set; }
    }
}