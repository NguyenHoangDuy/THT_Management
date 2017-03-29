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
    public class LightHeadTwo
    {
        [AutoIncrement]
        public int Id { get; set; }
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
        // Bạc lót
        public string SilverLining { get; set; }
        // Mã nhóm 2
        public string Code { get; set; }
        // Tâm lỗ Bulong
        public string BoltCenterValue { get; set; }
        // Kích thước lỗ Bulong
        public string BoltSizeValue { get; set; }
        // Ép mo
        public string EpMoValue { get; set; }
        // Chiều cao cửa trụ
        public string HeadHeightValue { get; set; }
        // Kích thước cửa trụ
        public string HeadSizeValue { get; set; }
        //Kích thước gân tăng cường
        public string TendonSizeValue { get; set; }
        // Số lượng gân tăng cường
        public string TendonNumberValue { get; set; }
        // Kích thước ống lót
        public string TubeSizeValue { get; set; }
        // Kích thước liền cần
        public string RodSizeValue { get; set; }
        // Gờ nước
        public string HillValue { get; set; }
        // Bản lề
        public string HingeValue { get; set; }
        // Bịt đầu
        public string MuzzleHeadValue { get; set; }
        // Loại đế
        public string PlinthTypeValue { get; set; }
        public string SilverLiningValue { get; set; }
    }
}