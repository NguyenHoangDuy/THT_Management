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
    public class LightHeadModel
    {
        // Nhóm TK
        public string TKGroup { get; set; }

        // Trụ
        public string Head { get; set; }

        // Chiều dài
        public string Length { get; set; }

        // Độ dày
        public string Thickness { get; set; }

        // ĐK đáy
        public string BottomDiameter { get; set; }

        // ĐK ngọn
        public string TopDiameter { get; set; }

        // Kích thước đế
        public string Size { get; set; }

        // Độ dày đế
        public string Empire { get; set; }

        //Group Two
        // Tâm lỗ Bulong
        public string BoltCenter { get; set; }

        // Kích thước lỗ Bulong
        public string BoltSize { get; set; }

        // Ép mo
        public string EpMo { get; set; }

        // Chiều cao cửa trụ
        public string HeadHeight { get; set; }

        // Kích thước cửa trụ
        public string HeadSize { get; set; }

        //Kích thước gân tăng cường
        public string TendonSize { get; set; }

        // Số lượng gân tăng cường
        public string TendonNumber { get; set; }

        // Kích thước ống lót
        public string TubeSize { get; set; }

        // Kích thước liền cần
        public string RodSize { get; set; }

        // Gờ nước
        public string Hill { get; set; }

        // Bản lề
        public string Hinge { get; set; }

        // Bịt đầu
        public string MuzzleHead { get; set; }

        // Loại đế
        public string PlinthType { get; set; }

        //Bạc lót
        public string SilverLining { get; set; }
    }
}