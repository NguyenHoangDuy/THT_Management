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
    public class RodModel
    {
        // Nhóm TK
        public string TKGroup { get; set; }
        // Tên loại
        public string TypeName { get; set; }
        // STT
        public string No { get; set; }
        // SL nhánh cần
        public string RodBranchQuantity { get; set; }
        // Cao
        public string Height { get; set; }
        // Vươn
        public string Rise { get; set; }
        // Kiểu
        public string Style { get; set; }
        // ĐK đáy ống đứng
        public string TubeBottomDiameter { get; set; }
        // ĐK ngọn ống đứng
        public string TubeTopDiameter { get; set; }
        // Độ dày ống đứng
        public string TubeThickness { get; set; }
        // ĐK đáy nhánh chính
        public string PrimaryBranchBottomDiameter { get; set; }
        // ĐK ngọn nhánh chính
        public string PrimaryBranchTopDiameter { get; set; }
        // Độ dày nhánh chính
        public string PrimaryBranchThickness { get; set; }
        // ĐK đáy nhánh phụ
        public string ExtraBranchBottomDiameter { get; set; }
        // ĐK ngọn nhánh phụ
        public string ExtraBranchTopDiameter { get; set; }
        // Độ dày nhánh phụ
        public string ExtraBranchThickness { get; set; }
        // Ống lót
        public string TubeSize { get; set; }
        // Góc cần
        public string AngleRod { get; set; }
        // Cần BTLT
        public string BTLTRod { get; set; }
    }
}