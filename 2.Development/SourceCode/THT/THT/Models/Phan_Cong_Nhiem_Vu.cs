using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Phan_Cong_Nhiem_Vu
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_lenh_sx { get; set; }
        public string ma_quy_trinh_sx { get; set; }
        public string ma_cong_viec { get; set; }
        public string ma_nhan_vien { get; set; }
        public DateTime ngay { get; set; }
        public DateTime thoi_gian_du_kien { get; set; }
        public string trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }

    }
}