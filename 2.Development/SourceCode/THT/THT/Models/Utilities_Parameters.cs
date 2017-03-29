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

namespace THT.Models
{
    public class Utilities_Parameters
    {
        [AutoIncrement]
        public int ID { get; set; }
        public string ParamID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Descr { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
    public class CommonModel
    {
        public string ParamID { get; set; }
        public string Value { get; set; }
    }
 
}