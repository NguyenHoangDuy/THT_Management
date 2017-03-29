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
    public class LightHeadHeader
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string HeaderCode { get; set; }
        public string HeaderName { get; set; }
        public Boolean CodeIsWeight { get; set; }
        public Boolean NameIsWeight { get; set; }
        public Boolean HaveCode { get; set; }
        public Boolean HaveDesc { get; set; }
        public Boolean HaveWeight { get; set; }
        public string TypeId { get; set; }
    }
}