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
    public class FoundationBoltTwo
    {
        [AutoIncrement]
        public int Id { get; set; }
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
        // Mã nhóm 2
        public string Code { get; set; }
        // SL Bu-long
        public string BoltQuantityValue { get; set; }
        //Loai dai
        public string HoopTypeValue { get; set; }
        //SL dai
        public string HoopQuantityValue { get; set; }
        //Loai Giang
        public string GiangTypeValue { get; set; }
        //SL Giang
        public string GiangQuantityValue { get; set; }
        //Kiểu hàn khung
        public string FrameTypeValue { get; set; }
        //Kích thước bẻ đầu
        public string HeadSizeValue { get; set; }
    }
}