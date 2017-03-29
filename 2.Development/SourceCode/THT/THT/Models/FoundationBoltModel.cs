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
    public class FoundationBoltModel
    {
        // Nhóm TK
        public string TKGroup { get; set; }
        // Duong kinh bu long
        public string BoltDiameter { get; set; }
        // Chieu dai
        public string Length { get; set; }
        // Kich thuoc han khung
        public string FrameSize { get; set; }
        // SL Bu-long
        public string BoltQuantity { get; set; }
        //Loai dai
        public string HoopType { get; set; }
        //SL dai
        public string HoopQuantity { get; set; }
        //Loai Giang
        public string GiangType { get; set; }
        //SL Giang
        public string GiangQuantity { get; set; }
        //Kiểu hàn khung
        public string FrameType { get; set; }
        //Kích thước bẻ đầu
        public string HeadSize { get; set; }
    }
}