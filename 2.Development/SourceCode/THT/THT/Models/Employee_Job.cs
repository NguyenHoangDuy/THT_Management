﻿using System;
using ServiceStack.DataAnnotations;

namespace THT.Models
{
    public class Employee_Job
    {
        [AutoIncrement]
        [PrimaryKey]
        public int id { get; set; }
        public string ma_nhan_vien { get; set; }
        public string ten_nhan_vien { get; set; }
        public string ma_cong_viec { get; set; }
        public DateTime ngay_tao { get; set; }
        public string nguoi_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }
        public string nguoi_cap_nhat { get; set; }

    }
}