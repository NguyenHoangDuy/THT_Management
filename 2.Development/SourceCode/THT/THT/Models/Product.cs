using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Product
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_lenh_sx { get; set; }
        public string ma_quy_trinh_sx { get; set; }
        public string ma_cong_viec { get; set; }
        public int so_thu_tu { get; set; }
        public string ma_san_pham { get; set; }
        public DateTime thoi_gian_bat_dau { get; set; }
        public DateTime thoi_gian_ket_thuc { get; set; }
        public string ma_nhan_vien_quet { get; set; }
        public string trang_thai { get; set; }
        public string ghi_chu { get; set; }
        public string vi_tri_loi { get; set; }
        public string muc_do_nghiem_trong { get; set; }
        public string hinh_anh_loi { get; set; }
        public string mieu_ta_loi { get; set; }
        public string he_so_chat_luong { get; set; }
        public string nhan_vien_qc { get; set; }
        public DateTime ngay_quet_qc { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }

    }
}