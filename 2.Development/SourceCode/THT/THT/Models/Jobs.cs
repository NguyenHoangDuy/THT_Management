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
    public class Jobs
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_cong_viec { get; set; }
        public string ten_cong_viec { get; set; }
        public string loai_cong_viec { get; set; }
        public double dinh_muc_cong_viec { get; set; }
        public string trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }

    }
}