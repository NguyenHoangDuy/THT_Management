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
    public class RodTwo
    {
        [AutoIncrement]
        public int Id { get; set; }
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
        // Mã nhóm 2
        public string Code { get; set; }
        // Kiểu
        public string StyleValue { get; set; }
        // ĐK đáy ống đứng
        public string TubeBottomDiameterValue { get; set; }
        // ĐK ngọn ống đứng
        public string TubeTopDiameterValue { get; set; }
        // Độ dày ống đứng
        public string TubeThicknessValue { get; set; }
        // ĐK đáy nhánh chính
        public string PrimaryBranchBottomDiameterValue { get; set; }
        // ĐK ngọn nhánh chính
        public string PrimaryBranchTopDiameterValue { get; set; }
        // Độ dày nhánh chính
        public string PrimaryBranchThicknessValue { get; set; }
        // ĐK đáy nhánh phụ
        public string ExtraBranchBottomDiameterValue { get; set; }
        // ĐK ngọn nhánh phụ
        public string ExtraBranchTopDiameterValue { get; set; }
        // Độ dày nhánh phụ
        public string ExtraBranchThicknessValue { get; set; }
        // Ống lót
        public string TubeSizeValue { get; set; }
        // Góc cần
        public string AngleRodValue { get; set; }
    }
}