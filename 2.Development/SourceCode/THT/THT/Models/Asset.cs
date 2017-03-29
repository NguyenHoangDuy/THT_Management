using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using Dapper;

namespace THT.Models
{
    public class Asset
    {
        public string ID { get; set; }
        public int RoleID { get; set; }
        public string MenuID { get; set; }
        public string MenuName { get; set; }
        public string ParentMenuID { get; set; }
        public int View { get; set; }
        public int Insert { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
        public int Export { get; set; }
        public int Import { get; set; }
    }
}