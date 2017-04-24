using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Lenh_San_Xuat
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_lenh_sx { get; set; }
        public string don_vi_mua_hang { get; set; }
        public string ma_san_pham_sx { get; set; }
        public string ten_san_pham_sx { get; set; }
        public int so_luong { get; set; }
        public string ma_quy_trinh_sx { get; set; }
        public DateTime thoi_gian_du_kien { get; set; }
        [Ignore]
        public string strthoi_gian_du_kien { get; set; }
        public string ban_ve { get; set; }
        public double khoi_luong { get; set; }
        public string trang_thai { get; set; }
        public string ghi_chu { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }
    }
}