using Kendo.Mvc.UI;
using ServiceStack.DataAnnotations;
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
    public class RequestModel
    {
        public int Id { get; set; }
        public string TypeId { get; set; }
        public string TypeValue { get; set; }
        public string Value { get; set; }
    }
}