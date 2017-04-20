using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Check_In
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_nhan_vien { get; set; }
        [Ignore]
        public string mat_khau { get; set; }
        public DateTime ngay { get; set; }
        public string trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }

    }
}