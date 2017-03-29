using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace THT.Models
{
    public class RodHeader
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string HeaderCode { get; set; }
        public string HeaderName { get; set; }
        public Boolean CodeIsWeight { get; set; }
        public Boolean NameIsWeight { get; set; }
        public Boolean NameIsNumber { get; set; }
        public Boolean HaveImage { get; set; }
        public Boolean HaveCode { get; set; }
        public Boolean HaveDesc { get; set; }
        public Boolean HaveWeight { get; set; }
        public string TypeId { get; set; }
    }
}