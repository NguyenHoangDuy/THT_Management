using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THT.Service;
using THT.Models;
using THT.Helpers;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using System.Collections;
using System.Configuration;
using log4net;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.Data.SqlClient;
using Dapper;
using System.Globalization;

namespace THT.Controllers
{
    public class SearchRodController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /SearchRod/
        public ActionResult Index()
        {

            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                dbConn.Close();
                ViewBag.txtValue = "";
                LoadDataRod(0);
                return View("_SearchRod", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        private void LoadDataRod(int idTypeCode)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            if (idTypeCode == 0)
            {
                ViewBag.txTubeLength = "";
                ViewBag.txtPrimaryRodLength = "";
                ViewBag.txtExtraRodLength = "";
                ViewBag.txtOther = "";
            }
            List<String> DisplayName = new List<string>();

            var RodtHeaderList = dbConn.Select<RodHeader>("select * from RodHeader order by Id");

            for (int i = 0; i < RodtHeaderList.Count; i++)
            {
                var TypeId = RodtHeaderList[i].TypeId;

                var List = dbConn.Select<RodOne>(ro => ro.TypeId == TypeId).OrderBy(s => s.Id);

                DisplayName.Add(RodtHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.TKGroup = List;
                }
                if (i == 1)
                {
                    ViewBag.TypeName = List;
                }
                if (i == 3)
                {
                    if (idTypeCode != 0)
                    {
                        var TypeCode = dbConn.Select<RodOne>(ro => ro.Id == idTypeCode).SingleOrDefault().Code;

                        var aData = dbConn.Select<RodOne>(ro => ro.TypeCode == TypeCode);

                        ViewBag.No = aData;
                    }
                    else
                    {
                        var aData = dbConn.Select<RodOne>("select * from RodOne where 1=2");
                        ViewBag.No = aData;
                    }
                }
                if (i == 2)
                {
                    ViewBag.RodBranchQuantity = List;
                }

                if (i == 4)
                {
                    ViewBag.Height = List;
                }
                if (i == 5)
                {
                    ViewBag.Rise = List;
                }
                if (i == 6)
                {
                    ViewBag.Style = List;
                }
                if (i == 7)
                {
                    ViewBag.TubeBottomDiameter = List;
                }
                if (i == 8)
                {
                    ViewBag.TubeTopDiameter = List;
                }
                if (i == 9)
                {
                    ViewBag.TubeThickness = List;
                }
                if (i == 10)
                {
                    ViewBag.PrimaryBranchBottomDiameter = List;
                }
                if (i == 11)
                {
                    ViewBag.PrimaryBranchTopDiameter = List;
                }
                if (i == 12)
                {
                    ViewBag.PrimaryBranchThickness = List;
                }
                if (i == 13)
                {
                    ViewBag.ExtraBranchBottomDiameter = List;
                }
                if (i == 14)
                {
                    ViewBag.ExtraBranchTopDiameter = List;
                }
                if (i == 15)
                {
                    ViewBag.ExtraBranchThickness = List;
                }
                if (i == 16)
                {
                    ViewBag.TubeSize = List;
                }

                if (i == 17)
                {
                    ViewBag.AngleRod = List;
                }

                if (i == 18)
                {
                    ViewBag.BTLTRod = List;
                }

            }
            ViewBag.DisplayName = DisplayName;
        }

        public ActionResult Rod_SendMail()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();

            RodTwo rodtow = new RodTwo();
            var RodModel = TempData["RodtModel"] as RodModel ?? new RodModel();

            var Id = int.Parse(RodModel.Style);
            rodtow.Style = RodModel.Style;
            rodtow.StyleValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.TubeBottomDiameter);
            rodtow.TubeBottomDiameter = RodModel.TubeBottomDiameter;
            rodtow.TubeBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.TubeTopDiameter);
            rodtow.TubeTopDiameter = RodModel.TubeTopDiameter;
            rodtow.TubeTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.TubeThickness);
            rodtow.TubeThickness = RodModel.TubeThickness;
            rodtow.TubeThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.PrimaryBranchBottomDiameter);
            rodtow.PrimaryBranchBottomDiameter = RodModel.PrimaryBranchBottomDiameter;
            rodtow.PrimaryBranchBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.PrimaryBranchTopDiameter);
            rodtow.PrimaryBranchTopDiameter = RodModel.PrimaryBranchTopDiameter;
            rodtow.PrimaryBranchTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.PrimaryBranchThickness);
            rodtow.PrimaryBranchThickness = RodModel.PrimaryBranchThickness;
            rodtow.PrimaryBranchThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.ExtraBranchBottomDiameter);
            rodtow.ExtraBranchBottomDiameter = RodModel.ExtraBranchBottomDiameter;
            rodtow.ExtraBranchBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.ExtraBranchTopDiameter);
            rodtow.ExtraBranchTopDiameter = RodModel.ExtraBranchTopDiameter;
            rodtow.ExtraBranchTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.ExtraBranchThickness);
            rodtow.ExtraBranchThickness = RodModel.ExtraBranchThickness;
            rodtow.ExtraBranchThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.TubeSize);
            rodtow.TubeSize = RodModel.TubeSize;
            rodtow.TubeSizeValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(RodModel.AngleRod);
            rodtow.AngleRod = RodModel.AngleRod;
            rodtow.AngleRodValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            rodtow.Code = "";

            var CheckID = dbConn.Select<RodTwo>(rt =>
                                      rt.Style == rodtow.Style
                                      && rt.TubeBottomDiameter == rodtow.TubeBottomDiameter
                                      && rt.TubeTopDiameter == rodtow.TubeTopDiameter
                                      && rt.TubeThickness == rodtow.TubeThickness
                                      && rt.PrimaryBranchBottomDiameter == rodtow.PrimaryBranchBottomDiameter
                                      && rt.PrimaryBranchTopDiameter == rodtow.PrimaryBranchTopDiameter
                                      && rt.PrimaryBranchThickness == rodtow.PrimaryBranchThickness
                                      && rt.ExtraBranchBottomDiameter == rodtow.ExtraBranchBottomDiameter
                                      && rt.ExtraBranchTopDiameter == rodtow.ExtraBranchTopDiameter
                                      && rt.ExtraBranchThickness == rodtow.ExtraBranchThickness
                                      && rt.TubeSize == rodtow.TubeSize
                                      && rt.AngleRod == rodtow.AngleRod
                                    );

            if (CheckID != null)
            {
                if (CheckID.Count == 0)
                {
                    dbConn.Insert<RodTwo>(rodtow);
                }
            }

            string body = "";
            List<string> eMailTos = new List<string>();

            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();

            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }

            string Subject = "Bộ dịch cần đèn chưa được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail.txt"));
            string bodytable = "";
            bodytable += "<tr><td>Kiểu</td><td>" + rodtow.StyleValue + "</td></tr>";
            bodytable += "<tr><td>ĐK đáy ống đứng</td><td>" + rodtow.TubeBottomDiameterValue + "</td></tr>";
            bodytable += "<tr><td>ĐK ngọn ống đứng</td><td>" + rodtow.TubeTopDiameterValue + "</td></tr>";
            bodytable += "<tr><td>Độ dày ống đứng</td><td>" + rodtow.TubeThicknessValue + "</td></tr>";
            bodytable += "<tr><td>ĐK đáy nhánh chính</td><td>" + rodtow.PrimaryBranchBottomDiameterValue + "</td></tr>";
            bodytable += "<tr><td>ĐK ngọn nhánh chính</td><td>" + rodtow.PrimaryBranchTopDiameterValue + "</td></tr>";
            bodytable += "<tr><td>Độ dày nhánh chính</td><td>" + rodtow.PrimaryBranchThicknessValue + "</td></tr>";
            bodytable += "<tr><td>ĐK đáy nhánh phụ</td><td>" + rodtow.ExtraBranchBottomDiameterValue + "</td></tr>";
            bodytable += "<tr><td>ĐK ngọn nhánh phụ</td><td>" + rodtow.ExtraBranchTopDiameterValue + "</td></tr>";
            bodytable += "<tr><td>Độ dày nhánh phụ</td><td>" + rodtow.ExtraBranchThicknessValue + "</td></tr>";
            bodytable += "<tr><td>Ống lót</td><td>" + rodtow.TubeSizeValue + "</td></tr>";
            bodytable += "<tr><td>Góc cần</td><td>" + rodtow.AngleRodValue + "</td></tr>";

            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "cần đèn");
            if (eMailTos.Count > 0)
                Mail.SendMail(eMailTos, Subject, body);
            //return Json(new { Status = 0, Message = "Success" });
            return Json(new { success = true });
        }

        public ActionResult Rod(RodModel rod)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var dict = new Dictionary<string, object>();
            dict["asset"] = userAsset;
            dict["activestatus"] = new CommonLib().GetActiveStatus();
            var model = dbConn.Select<RodTwo>(rt =>
                       rt.Style == rod.Style
                         && rt.TubeBottomDiameter == rod.TubeBottomDiameter
                         && rt.TubeTopDiameter == rod.TubeTopDiameter
                         && rt.TubeThickness == rod.TubeThickness
                         && rt.PrimaryBranchBottomDiameter == rod.PrimaryBranchBottomDiameter
                         && rt.PrimaryBranchTopDiameter == rod.PrimaryBranchTopDiameter
                         && rt.PrimaryBranchThickness == rod.PrimaryBranchThickness
                         && rt.ExtraBranchBottomDiameter == rod.ExtraBranchBottomDiameter
                         && rt.ExtraBranchTopDiameter == rod.ExtraBranchTopDiameter
                         && rt.ExtraBranchThickness == rod.ExtraBranchThickness
                         && rt.TubeSize == rod.TubeSize
                         && rt.AngleRod == rod.AngleRod).SingleOrDefault();

            var ChieuCaoOngDung = Request["txTubeLength"];
            var ChieuDaiNhanhChinh = Request["txtPrimaryRodLength"];
            var ChieuDaiNhanhPhu = Request["txtExtraRodLength"];
            var Other = Request["txtOther"];

            ViewBag.txTubeLength = ChieuCaoOngDung;
            ViewBag.txtPrimaryRodLength = ChieuDaiNhanhChinh;
            ViewBag.txtExtraRodLength = ChieuDaiNhanhPhu;
            ViewBag.txtOther = Other;

            if (model == null)
            {
                TempData["RodtModel"] = rod;
                ViewBag.Result = -1;
                LoadDataRod(int.Parse(rod.TypeName.ToString()));
                return Json(new { success = false, data = "" }, JsonRequestBehavior.AllowGet);
            }

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            string FinalCode = "";
            string Description = "Cần ";

            string Parameters = "";
            //double BoltWeight = 0;
            //double Weight = 0;

            int Id = int.Parse(rod.TKGroup);
           
            FinalCode = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Code;

            Id = int.Parse(rod.TypeName);
            string TypeName = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Code;

            FinalCode += TypeName;

            Id = int.Parse(rod.No);
            string No = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            FinalCode += No;
            Id = int.Parse(rod.RodBranchQuantity);
            string RodQuan = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

            switch (RodQuan)
            {
                case "1": Description += "đơn";
                    break;
                case "2": Description += "đôi";
                    break;
                case "3": Description += "ba";
                    break;
                default: Description += "tư";
                    break;
            }
            Description += " " + TypeName + " " + No;

            Id = int.Parse(rod.BTLTRod.ToString());
            var Model = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault();

            string BTLTRodCode = "";
            if (!(Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong")) && (!Helper.ToVietnameseWithoutAccent(Model.Code.ToLower()).Equals("khong")))
            {
                Description += " " + Model.Name;
                BTLTRodCode = Model.Code;
            }

            //string BTLTRodCode = (Helper.Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong")) ? "" : Model.Code;
            //if (BTLTRodCode != "")
            //    BTLTRodCode = (Helper.Helper.ToVietnameseWithoutAccent(Model.Code.ToLower()).Equals("khong")) ? "" : Model.Code;

            FinalCode += RodQuan;
            Id = int.Parse(rod.Height);
            Model = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault();

            FinalCode += Model.Code;
            Description += "- Cao: " + Model.Name + "m";
            Id = int.Parse(rod.Rise);
            Model = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault();

            FinalCode += Model.Code;
            Description += "- Vươn: " + Model.Name + "m";
            Parameters = Description;
            double WeightTubeBottomDiameter = (Helper.ToVietnameseWithoutAccent(model.TubeBottomDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.TubeBottomDiameterValue.ToString(), culture);
            double WeightTubeTopDiameter = (Helper.ToVietnameseWithoutAccent(model.TubeTopDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.TubeTopDiameterValue.ToString(), culture);
            double WeightTubeThickness = (Helper.ToVietnameseWithoutAccent(model.TubeThicknessValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.TubeThicknessValue.ToString(), culture);
            double WeightTube = (((WeightTubeBottomDiameter + WeightTubeTopDiameter) / 2) / 1000) * 3.14 * WeightTubeThickness * double.Parse(ChieuCaoOngDung.ToString(), culture) * 7.85;

            double WeightPrimaryBranchBottomDiameter = (Helper.ToVietnameseWithoutAccent(model.PrimaryBranchBottomDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.PrimaryBranchBottomDiameterValue.ToString(), culture);
            double WeightPrimaryBranchTopDiameter = (Helper.ToVietnameseWithoutAccent(model.PrimaryBranchTopDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.PrimaryBranchTopDiameterValue.ToString(), culture);
            double WeightPrimaryBranchThickness = (Helper.ToVietnameseWithoutAccent(model.PrimaryBranchThicknessValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.PrimaryBranchThicknessValue.ToString(), culture);
            double WeightPrimaryRod = (((WeightPrimaryBranchBottomDiameter + WeightPrimaryBranchTopDiameter) / 2) / 1000) * 3.14 * WeightPrimaryBranchThickness * double.Parse(ChieuDaiNhanhChinh.ToString(), culture) * 7.85;

            double WeightExtraBranchBottomDiameter = (Helper.ToVietnameseWithoutAccent(model.ExtraBranchBottomDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.ExtraBranchBottomDiameterValue.ToString(), culture);
            double WeightExtraBranchTopDiameter = (Helper.ToVietnameseWithoutAccent(model.ExtraBranchTopDiameterValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.ExtraBranchTopDiameterValue.ToString(), culture);
            double WeightExtraBranchThickness = (Helper.ToVietnameseWithoutAccent(model.ExtraBranchThicknessValue.ToLower()).Equals("khong")) ? 0 : double.Parse(model.ExtraBranchThicknessValue.ToString(), culture);
            double WeightExtraRod = (((WeightExtraBranchBottomDiameter + WeightExtraBranchTopDiameter) / 2) / 1000) * 3.14 * WeightExtraBranchThickness * double.Parse(ChieuDaiNhanhPhu.ToString(), culture) * 7.85;



            Parameters = "- Kiểu: " + model.StyleValue;
            if (WeightTubeBottomDiameter != 0)
                Parameters += "- ĐK đáy ống đứng: ∅" + WeightTubeBottomDiameter;
            if (WeightTubeTopDiameter != 0)
                Parameters += " - ĐK ngọn ống đứng: ∅" + WeightTubeTopDiameter;
            if (WeightTubeThickness != 0)
                Parameters += " - Độ dày ống đứng: " + WeightTubeThickness + "mm";

            if (WeightPrimaryBranchBottomDiameter != 0)
                Parameters += "- ĐK đáy nhánh chính: ∅" + WeightPrimaryBranchBottomDiameter;
            if (WeightPrimaryBranchTopDiameter != 0)
                Parameters += " -ĐK ngọn nhánh chính: ∅" + WeightPrimaryBranchTopDiameter;
            if (WeightPrimaryBranchThickness != 0)
                Parameters += " - Độ dày nhánh chính: " + WeightPrimaryBranchThickness + "mm";

            if (WeightExtraBranchBottomDiameter != 0)
                Parameters += "- ĐK đáy nhánh phụ: ∅" + WeightExtraBranchBottomDiameter;
            if (WeightExtraBranchTopDiameter != 0)
                Parameters += ". -ĐK ngọn nhánh phụ: ∅" + WeightExtraBranchTopDiameter;
            if (WeightExtraBranchThickness != 0)
                Parameters += " - Độ dày nhánh phụ: " + WeightExtraBranchThickness + "mm";

            double WeightTubeSize = 0;
            Id = int.Parse(model.TubeSize.ToString());
            Model = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault();

            if (!((Helper.ToVietnameseWithoutAccent(Model.Name.ToLower())).Equals("khong")))
                WeightTubeSize = double.Parse(Model.Weight.ToString(), culture);

            Parameters += "- Ống Lót: " + Model.Name;
            Parameters += " - Gốc cần:" + model.AngleRodValue;

            double WeightOther = 0;
            if (Other != "")
                WeightOther = double.Parse(Other.ToString(), culture);

            int SL;
            Int32.TryParse(RodQuan, out SL);

            double Weight = (WeightTube + WeightPrimaryRod + WeightExtraRod + WeightTubeSize + WeightOther) * (1 + (0.8 * (SL - 1)));

            FinalCode += BTLTRodCode;

            ViewBag.Result = 1;
            ViewBag.FinalCode = FinalCode + model.Code;
            ViewBag.Description = Description;
            ViewBag.Parameters = Parameters;
            //ViewBag.WeightTube = WeightTube.ToString(culture);
            ViewBag.WeightTube = Math.Round(WeightTube, 2).ToString(culture);
            //ViewBag.WeightPrimaryRod = WeightPrimaryRod.ToString(culture);
            ViewBag.WeightPrimaryRod = Math.Round(WeightPrimaryRod, 2).ToString(culture);
            //ViewBag.WeightExtraRod = WeightExtraRod.ToString(culture);
            ViewBag.WeightExtraRod = Math.Round(WeightExtraRod, 2).ToString(culture);
            //ViewBag.Weight = Weight.ToString(culture);
            ViewBag.Weight = Math.Round(Weight, 2).ToString(culture);
            LoadDataRod(int.Parse(rod.TypeName.ToString()));
            return Json(new { success = true, FinalCode = ViewBag.FinalCode, Description = ViewBag.Description, Parameters = ViewBag.Parameters,
                              WeightTube = ViewBag.WeightTube,
                              WeightPrimaryRod = ViewBag.WeightPrimaryRod,
                              WeightExtraRod = ViewBag.WeightExtraRod,
                              Weight = ViewBag.Weight
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadNo(int Id)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var TypeCode = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Code;

                var listLoaiCan = dbConn.Select<RodOne>(ro => ro.TypeCode == TypeCode);

                return Json(new { success = true, listLoaiCan = listLoaiCan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult GetImage(int Id)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var aData = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().ImagePath;
                return Json(new { success = true, ImagePath = aData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}