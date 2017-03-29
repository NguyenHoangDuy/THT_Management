using Kendo.Mvc.UI;
using THT.Service;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace THT.Models
{
    public class CategoryConfig
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string IsSaved { get; set; }
        public string IsExpirated { get; set; }
        public string SideboardID { get; set; }
        public string Format { get; set; }
        public int TimeSave { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
        [Ignore]
        public string ListGroup { get; set; }
    }
}