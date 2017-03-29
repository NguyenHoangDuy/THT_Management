using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using THT.Service;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;


namespace THT.Models
{
    public class GroupDocument
    {

        [AutoIncrement]
        public int ID { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
    }
}