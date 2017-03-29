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
    public class Document
    {
        public string DocumentID { get; set; }
        public string CategoryID { get; set; }
        public string DocumentName { get; set; }
        public string No { get; set; }
        [Ignore]
        public string strDateSave { get; set; }
        public DateTime? DateSave { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public string SideboardID { get; set; }
        [Ignore]
        public string strExpirationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string FileNameLocal { get; set; }
        public string FileNameServer { get; set; }
        public string Path { get; set; }
        public int TimeSave { get; set; }
        public string IsSaved { get; set; }
        public string IsExpirated { get; set; }
        [Ignore]
        public string StatusSaved { get; set; }
        [Ignore]
        public string StatusExpirated { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
    }
}