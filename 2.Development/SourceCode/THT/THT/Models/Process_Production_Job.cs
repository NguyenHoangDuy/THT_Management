using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Process_Production_Job
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_quy_trinh_sx { get; set; }
        public string ma_cong_viec { get; set; }
        [Ignore]
        public string ma_cong_viectmp { get; set; }
        public int so_thu_tu { get; set; }
        public string trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }
    }
}