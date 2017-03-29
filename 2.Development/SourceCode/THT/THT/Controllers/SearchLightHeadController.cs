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
    public class SearchLightHeadController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /SearchLightHead/
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
                LoadDataLightHead();
                return View("_SearchLightHead", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public void LoadDataLightHead()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            List<String> DisplayName = new List<string>();
            var LightHeadHeaderList = dbConn.Select<LightHeadHeader>("select * from LightHeadHeader order by Id");

            for (int i = 0; i < LightHeadHeaderList.Count; i++)
            {
                var TypeId = LightHeadHeaderList[i].TypeId;
                //var List = (from lho in db.LightHeadOnes
                //            where lho.TypeId == TypeId
                //            select new { Id = lho.Id, Name = lho.Name }).ToList().OrderBy(lho => lho.Id);

                var List = dbConn.Select<LightHeadOne>(s => s.TypeId == TypeId).OrderBy(s => s.Id);

                DisplayName.Add(LightHeadHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.TKGroup = List;
                }

                if (i == 1)
                {
                    ViewBag.Head = List;
                }

                if (i == 2)
                {
                    ViewBag.Length = List;
                }

                if (i == 3)
                {
                    ViewBag.Thickness = List;
                }

                if (i == 4)
                {
                    ViewBag.BottomDiameter = List;
                }

                if (i == 5)
                {
                    ViewBag.TopDiameter = List;
                }

                if (i == 6)
                {
                    ViewBag.Size = List;
                }

                if (i == 7)
                {
                    ViewBag.Empire = List;
                }
                //---//
                if (i == 8)
                {
                    ViewBag.BoltCenter = List;
                }
                if (i == 9)
                {
                    ViewBag.BoltSize = List;
                }
                if (i == 10)
                {
                    ViewBag.EpMo = List;
                }
                if (i == 11)
                {
                    ViewBag.HeadHeight = List;
                }
                if (i == 12)
                {
                    ViewBag.HeadSize = List;
                }
                if (i == 13)
                {
                    ViewBag.TendonSize = List;
                }
                if (i == 14)
                {
                    ViewBag.TendonNumber = List;
                }
                if (i == 15)
                {
                    ViewBag.TubeSize = List;
                }
                if (i == 16)
                {
                    ViewBag.RodSize = List;
                }
                if (i == 17)
                {
                    ViewBag.Hill = List;
                }
                if (i == 18)
                {
                    ViewBag.Hinge = List;
                }
                if (i == 19)
                {
                    ViewBag.MuzzleHead = List;
                }
                if (i == 20)
                {
                    ViewBag.PlinthType = List;
                }

                if (i == 21)
                {
                    ViewBag.SilverLining = List;
                }
            }
            ViewBag.DisplayName = DisplayName;
            dbConn.Close();
        }

        public ActionResult LightHead(LightHeadModel lighthead)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var dict = new Dictionary<string, object>();
            dict["asset"] = userAsset;
            dict["activestatus"] = new CommonLib().GetActiveStatus();
            //var model = (from lht in db.LightHeadTwos
            //             where lht.BoltCenter == lighthead.BoltCenter
            //             && lht.BoltSize == lighthead.BoltSize
            //             && lht.EpMo == lighthead.EpMo
            //             && lht.HeadHeight == lighthead.HeadHeight
            //             && lht.HeadSize == lighthead.HeadSize
            //             && lht.TendonSize == lighthead.TendonSize
            //             && lht.TendonNumber == lighthead.TendonNumber
            //             && lht.TubeSize == lighthead.TubeSize
            //             && lht.RodSize == lighthead.RodSize
            //             && lht.Hill == lighthead.Hill
            //             && lht.Hinge == lighthead.Hinge
            //             && lht.MuzzleHead == lighthead.MuzzleHead
            //             && lht.PlinthType == lighthead.PlinthType
            //             && lht.SilverLining == lighthead.SilverLining
            //             select lht).FirstOrDefault();

            var model = dbConn.Select<LightHeadTwo>(lht => lht.BoltCenter == lighthead.BoltCenter
                         && lht.BoltSize == lighthead.BoltSize
                         && lht.EpMo == lighthead.EpMo
                         && lht.HeadHeight == lighthead.HeadHeight
                         && lht.HeadSize == lighthead.HeadSize
                         && lht.TendonSize == lighthead.TendonSize
                         && lht.TendonNumber == lighthead.TendonNumber
                         && lht.TubeSize == lighthead.TubeSize
                         && lht.RodSize == lighthead.RodSize
                         && lht.Hill == lighthead.Hill
                         && lht.Hinge == lighthead.Hinge
                         && lht.MuzzleHead == lighthead.MuzzleHead
                         && lht.PlinthType == lighthead.PlinthType
                         && lht.SilverLining == lighthead.SilverLining).SingleOrDefault();

            var BValue = Request["txtValue"];
            ViewBag.txtValue = BValue;
            if (model == null)
            {
                //Helper.Helper.LightHead = lighthead;
                TempData["LightHeadModel"] = lighthead;
                ViewBag.Result = -1;
                LoadDataLightHead();
                //return View("_SearchLightHead", dict);
                //return View();
                return Json(new { success = false, data = "" }, JsonRequestBehavior.AllowGet);
                //return View("_SearchLightHead", dict);
            }

            string FinalCode = "";
            string Description = "";
            string Parameters = "";
            LightHeadOne Model;
            double A = 1.0;
            int Id = int.Parse(lighthead.TKGroup);
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //if (Helper.Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("bat giac"))
            //    A = 1.086;

            FinalCode += Model.Code;
            Id = int.Parse(lighthead.Head);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            FinalCode += Model.Code;
            Description += Model.Name + " ";

            if (Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("bat giac") || Model.Code.ToUpper().Trim() == "BG")
                A = 1.086;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            Id = int.Parse(lighthead.Length);
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            FinalCode += Model.Code;
            Description += Model.Name + "mx";
            double WeightLength = double.Parse(Model.Name.ToString(), culture);

            Id = int.Parse(lighthead.Thickness);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            FinalCode += Model.Code;
            Description += Model.Name + "mm (";
            double WeightThickness = double.Parse(Model.Name.ToString(), culture);

            Id = int.Parse(lighthead.BottomDiameter);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            FinalCode += Model.Code;
            Description += Model.Name + "/";
            Parameters += "-ĐK đáy: ∅ " + Model.Name;
            double WeightBottomDiameter = double.Parse(Model.Name.ToString(), culture);

            Id = int.Parse(lighthead.TopDiameter);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            FinalCode += Model.Code;
            Description += Model.Name + ") mạ nhúng";
            Parameters += " -ĐK ngọn: ∅ " + Model.Name;
            double WeightTopDiameter = double.Parse(Model.Name.ToString(), culture);

            Id = int.Parse(lighthead.Size);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();

            if (Helper.ToVietnameseWithoutAccent(model.EpMoValue.ToLower()).Equals("co"))
                Parameters += " - Đế dập " + model.PlinthTypeValue;
            else
                Parameters += " - Đế " + model.PlinthTypeValue;
            Parameters += ": " + Model.Name.ToLower();
            Id = int.Parse(lighthead.Empire);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightEmpire = double.Parse(Model.Name.ToString(), culture);
            string EmpireCode = Model.Code;
            Parameters += "x" + WeightEmpire;

            Parameters += " - Tâm lỗ Bulong: " + model.BoltCenterValue + "(" + model.BoltSizeValue + ") - Cửa trụ:" + model.HeadSizeValue + " cao " + model.HeadHeightValue;

            if (!((Helper.ToVietnameseWithoutAccent(model.TendonSizeValue.ToLower())).Equals("khong")))
                Parameters += "  - Gân tăng cường: " + model.TendonSizeValue + "mm";

            if (!Helper.ToVietnameseWithoutAccent(model.TendonNumber.ToLower()).Equals("khong"))
                Parameters += ": " + model.TendonNumberValue + ".";

            if (!((Helper.ToVietnameseWithoutAccent(model.TubeSizeValue.ToLower())).Equals("khong")))
                Parameters += " - Ống lót: ∅" + model.TubeSizeValue + "mm";

            if (!((Helper.ToVietnameseWithoutAccent(model.RodSizeValue.ToLower())).Equals("khong")))
                Parameters += " - Liền cần: " + model.RodSizeValue;

            if (((Helper.ToVietnameseWithoutAccent(model.HillValue.ToLower())).Equals("co")))
                Parameters += " - Gờ nước";

            if (((Helper.ToVietnameseWithoutAccent(model.HingeValue.ToLower())).Equals("co")))
                Parameters += " - Bản lề";

            if (((Helper.ToVietnameseWithoutAccent(model.MuzzleHeadValue.ToLower())).Equals("co")))
                Parameters += " - Bịt đầu";

            Id = int.Parse(lighthead.Size);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightSize = double.Parse(Model.Code.ToString(), culture);
            FinalCode += Model.Code;

            FinalCode += EmpireCode;

            Id = int.Parse(lighthead.RodSize);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            if (!((Helper.ToVietnameseWithoutAccent(Model.Name.ToLower())).Equals("khong")))
                Description += " liền cần";

            //kích thước gân tăng cường
            Id = int.Parse(lighthead.TendonSize);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightTendonSize = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                WeightTendonSize = double.Parse(Model.Weight.ToString(), culture);
            //---------------------------

            //Số lượng gân tăng cường
            Id = int.Parse(lighthead.TendonNumber);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double TendonNumber = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                TendonNumber = double.Parse(Model.Name.ToString(), culture);
            //---------------------
            //kích thước ống lót
            Id = int.Parse(lighthead.TubeSize);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightTubeSize = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                WeightTubeSize = double.Parse(Model.Weight.ToString(), culture);
            //
            //Bản lề
            Id = int.Parse(lighthead.Hinge);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();

            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();

            double WeightHinge = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                WeightHinge = double.Parse(Model.Weight.ToString(), culture);
            //----------------

            //Gờ nước
            Id = int.Parse(lighthead.Hill);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightHill = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                WeightHill = double.Parse(Model.Weight.ToString(), culture);
            //--------------

            //Bạc lót
            Id = int.Parse(lighthead.SilverLining);
            Model = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault();
            //Model = (from lho in db.LightHeadOnes
            //         where lho.Id == Id
            //         select lho).FirstOrDefault();
            double WeightSilverLining = 0;
            if (!Helper.ToVietnameseWithoutAccent(Model.Name.ToLower()).Equals("khong"))
                WeightSilverLining = double.Parse(Model.Weight.ToString(), culture);
            //--------------

            // double Weight = (((WeightTopDiameter + WeightBottomDiameter) / 2) / 1000) * 3.14 * WeightThickness * WeightLength * 7.85;
            //Weight += Math.Pow((WeightSize / 1000), 2) * WeightEmpire * 7.85;
            //Weight += WeightTendonSize + WeightTubeSize + WeightHill;
            //double Weight1 = (((WeightTopDiameter + WeightBottomDiameter) / 2) / 1000) * 3.14 * WeightThickness * WeightLength * 7.85;
            //double Weight2 = Math.Pow((WeightSize / 1000), 2) * WeightEmpire * 7.85;
            //double Weight3 = WeightSilverLining + WeightHinge + WeightHill + (WeightTendonSize * TendonNumber);
            double B = 0;
            if (BValue != "")
                B = double.Parse(BValue.ToString(), culture);

            double C = 0;
            if (!Helper.ToVietnameseWithoutAccent(model.EpMoValue.ToLower()).Equals("khong"))
                C = 5.0;

            double Weight = (((WeightTopDiameter + WeightBottomDiameter) / 2) / 1000) * A * 3.14 * WeightThickness * (WeightLength + B) * 7.85;
            Weight += Math.Pow(((WeightSize + C) / 1000), 2) * WeightEmpire * 7.85;
            Weight += WeightSilverLining + WeightHinge + WeightHill + (WeightTendonSize * TendonNumber) + WeightTubeSize;
            ViewBag.Result = 1;
            ViewBag.FinalCode = FinalCode + model.Code;
            ViewBag.Description = Description;
            ViewBag.Parameters = Parameters;
            ViewBag.Weight = Math.Round(Weight, 2).ToString(culture);
            //ViewBag.Weight = WeightLength.ToString(culture) + "||" + Weight1.ToString() + "||" + Weight2.ToString() + "||" + Weight3.ToString();
            LoadDataLightHead();
            dbConn.Close();
            //LoadDataLightHead();
            return Json(new { success = true, FinalCode = ViewBag.FinalCode, Description = ViewBag.Description, Parameters = ViewBag.Parameters, Weight = ViewBag.Weight }, JsonRequestBehavior.AllowGet);
            //return View("_SearchLightHead", dict);
        }

        public ActionResult LightHead_SendMail()
        {
            //Data.LightHeadTwo lightheadtwo = new Data.LightHeadTwo();
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var lightheadmodel = TempData["LightHeadModel"] as LightHeadModel ?? new LightHeadModel();
            LightHeadTwo lightheadtwo = new LightHeadTwo();

            var Id = int.Parse(lightheadmodel.BoltCenter);
            //lightheadtwo.BoltCenterValue = (from lho in db.LightHeadOnes
            //                                where lho.Id == Id
            //                                select lho).FirstOrDefault().Name;
            lightheadtwo.BoltCenter = lightheadmodel.BoltCenter;
            lightheadtwo.BoltCenterValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(lightheadmodel.BoltSize);
            //lightheadtwo.BoltSizeValue = (from lho in db.LightHeadOnes
            //                              where lho.Id == Id
            //                              select lho).FirstOrDefault().Name;
            lightheadtwo.BoltSize = lightheadmodel.BoltSize;
            lightheadtwo.BoltSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(lightheadmodel.EpMo);
            lightheadtwo.EpMo = lightheadmodel.EpMo;
            lightheadtwo.EpMoValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.EpMoValue = (from lho in db.LightHeadOnes
            //                          where lho.Id == Id
            //                          select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.HeadHeight);
            lightheadtwo.HeadHeight = lightheadmodel.HeadHeight;
            lightheadtwo.HeadHeightValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.HeadHeightValue = (from lho in db.LightHeadOnes
            //                                where lho.Id == Id
            //                                select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.HeadSize);
            lightheadtwo.HeadSize = lightheadmodel.HeadSize;
            lightheadtwo.HeadSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.HeadSizeValue = (from lho in db.LightHeadOnes
            //                              where lho.Id == Id
            //                              select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.TendonSize);
            lightheadtwo.TendonSize = lightheadmodel.TendonSize;
            lightheadtwo.TendonSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.TendonSizeValue = (from lho in db.LightHeadOnes
            //                                where lho.Id == Id
            //                                select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.TendonNumber);
            lightheadtwo.TendonNumber = lightheadmodel.TendonNumber;
            lightheadtwo.TendonNumberValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.TendonNumberValue = (from lho in db.LightHeadOnes
            //                                  where lho.Id == Id
            //                                  select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.TubeSize);
            lightheadtwo.TubeSize = lightheadmodel.TubeSize;
            lightheadtwo.TubeSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.TubeSizeValue = (from lho in db.LightHeadOnes
            //                              where lho.Id == Id
            //                              select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.RodSize);
            lightheadtwo.RodSize = lightheadmodel.RodSize;
            lightheadtwo.RodSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.RodSizeValue = (from lho in db.LightHeadOnes
            //                             where lho.Id == Id
            //                             select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.Hill);
            lightheadtwo.Hill = lightheadmodel.Hill;
            lightheadtwo.HillValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.HillValue = (from lho in db.LightHeadOnes
            //                          where lho.Id == Id
            //                          select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.Hinge);
            lightheadtwo.Hinge = lightheadmodel.Hinge;
            lightheadtwo.HingeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.HingeValue = (from lho in db.LightHeadOnes
            //                           where lho.Id == Id
            //                           select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.MuzzleHead);
            lightheadtwo.MuzzleHead = lightheadmodel.MuzzleHead;
            lightheadtwo.MuzzleHeadValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.MuzzleHeadValue = (from lho in db.LightHeadOnes
            //                                where lho.Id == Id
            //                                select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.PlinthType);
            lightheadtwo.PlinthType = lightheadmodel.PlinthType;
            lightheadtwo.PlinthTypeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.PlinthTypeValue = (from lho in db.LightHeadOnes
            //                                where lho.Id == Id
            //                                select lho).FirstOrDefault().Name;

            Id = int.Parse(lightheadmodel.SilverLining);
            lightheadtwo.SilverLining = lightheadmodel.SilverLining;
            lightheadtwo.SilverLiningValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;
            //lightheadtwo.SilverLiningValue = (from lho in db.LightHeadOnes
            //                                  where lho.Id == Id
            //                                  select lho).FirstOrDefault().Name;

            lightheadtwo.Code = "";

            var CheckID = dbConn.Select<LightHeadTwo>(lht => lht.BoltCenter == lightheadtwo.BoltCenter
                                             && lht.BoltSize == lightheadtwo.BoltSize
                                             && lht.EpMo == lightheadtwo.EpMo
                                             && lht.HeadHeight == lightheadtwo.HeadHeight
                                             && lht.HeadSize == lightheadtwo.HeadSize
                                             && lht.TendonSize == lightheadtwo.TendonSize
                                             && lht.TendonNumber == lightheadtwo.TendonNumber
                                             && lht.TubeSize == lightheadtwo.TubeSize
                                             && lht.RodSize == lightheadtwo.RodSize
                                             && lht.Hill == lightheadtwo.Hill
                                             && lht.Hinge == lightheadtwo.Hinge
                                             && lht.MuzzleHead == lightheadtwo.MuzzleHead
                                             && lht.PlinthType == lightheadtwo.PlinthType
                                             && lht.SilverLining == lightheadtwo.SilverLining
                                    );

            if (CheckID != null)
            {
                if (CheckID.Count == 0)
                {
                    dbConn.Insert<LightHeadTwo>(lightheadtwo);
                }
            }

            string body = "";
            List<string> eMailTos = new List<string>();

            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();

            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }

            string Subject = "Bộ dịch trụ đèn chưa được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail.txt"));
            string bodytable = "";
            bodytable += "<tr><td>Tâm lỗ bulong</td><td>" + lightheadtwo.BoltCenterValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước lỗ Bulong</td><td>" + lightheadtwo.BoltSizeValue + "</td></tr>";
            bodytable += "<tr><td>Ép mo</td><td>" + lightheadtwo.EpMoValue + "</td>";
            bodytable += "<tr><td>Chiều cao cửa trụ</td><td>" + lightheadtwo.HeadHeightValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước cửa trụ</td><td>" + lightheadtwo.HeadSizeValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước gân tăng cường</td><td>" + lightheadtwo.TendonSizeValue + "</td></tr>";
            bodytable += "<tr><td>Số lượng gân tăng cường</td><td>" + lightheadtwo.TendonNumberValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước ống lót</td><td>" + lightheadtwo.TubeSizeValue + "</td></tr>";
            bodytable += "<tr><td>Bạc lót</td><td>" + lightheadtwo.SilverLiningValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước liền cần</td><td>" + lightheadtwo.RodSizeValue + "</td></tr>";
            bodytable += "<tr><td>Gờ nước</td><td>" + lightheadtwo.HillValue + "</td></tr>";
            bodytable += "<tr><td>Bản lề</td><td>" + lightheadtwo.HingeValue + "</td></tr>";
            bodytable += "<tr><td>Bịt đầu</td><td>" + lightheadtwo.MuzzleHeadValue + "</td></tr>";
            bodytable += "<tr><td>Loại đế</td><td>" + lightheadtwo.PlinthTypeValue + "</td></tr>";
            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "trụ đèn");
            if (eMailTos.Count > 0)
                Mail.SendMail(eMailTos, Subject, body);
            //return Json(new { Status = 0, Message = "Success" });
            return Json(new { success = true });
        }
    }
}