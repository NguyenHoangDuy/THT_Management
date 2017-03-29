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
    public class SearchFoundationBoltController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /SearchFoundationBolt/
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
                LoadDataFoundationBolt();
                return View("_SearchFoundationBolt", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }


        public ActionResult FoundationBolt(FoundationBoltModel foundationbolt)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var dict = new Dictionary<string, object>();
            dict["asset"] = userAsset;
            dict["activestatus"] = new CommonLib().GetActiveStatus();
           
            var model = dbConn.Select<FoundationBoltTwo>(ldt => 
                        ldt.BoltQuantity == foundationbolt.BoltQuantity
                         && ldt.HoopType == foundationbolt.HoopType
                         && ldt.HoopQuantity == foundationbolt.HoopQuantity
                         && ldt.GiangType == foundationbolt.GiangType
                         && ldt.GiangQuantity == foundationbolt.GiangQuantity
                         && ldt.FrameType == foundationbolt.FrameType
                         && ldt.HeadSize == foundationbolt.HeadSize).SingleOrDefault();

            if (model == null)
            {
                TempData["FoundationBoltModel"] = foundationbolt;
                ViewBag.Result = -1;
                LoadDataFoundationBolt();
                return Json(new { success = false, data = "" }, JsonRequestBehavior.AllowGet);
            }

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            string FinalCode = "";
            string Description = "Bulong móng M";
            string Parameters = "";
            double BoltWeight = 0;
            double Weight = 0;
            // double C = 0;

            int Id = int.Parse(foundationbolt.TKGroup);
            //FinalCode += (from lho in db.FoundationBoltOnes
            //              where lho.Id == Id
            //              select lho).FirstOrDefault().Code;
            FinalCode += dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Code;

            FinalCode += "BLM";
            Id = int.Parse(foundationbolt.BoltDiameter);
            //var Model = (from lho in db.FoundationBoltOnes
            //             where lho.Id == Id
            //             select lho).FirstOrDefault();

            var Model = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault();

            FinalCode += Model.Code;
            Description += Model.Name + "x";
            string BoltDiameter = Model.Name;
            double BoltDiameterWeight = double.Parse(BoltDiameter, culture);
            BoltWeight += Math.Pow(double.Parse(Model.Name.ToString(), culture), 2.0) * 0.006167;

            Id = int.Parse(foundationbolt.HeadSize);
            Model = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault();

            double HeadSize = double.Parse(Model.Name, culture);

            Id = int.Parse(foundationbolt.FrameType);
            //string FrameTypeName = (from lho in db.FoundationBoltOnes
            //                        where lho.Id == Id
            //                        select lho).FirstOrDefault().Name;
            string FrameTypeName = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.Length);
            Model = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault();

            FinalCode += Model.Code;
            Description += Model.Name + " mạ đầu ren";
            Parameters = Description + " : " + model.BoltQuantityValue + " con." + " - Tán M" + BoltDiameter + " đệm phẳng: " + model.BoltQuantityValue + " con.";
            Parameters += " - Đai " + model.HoopTypeValue + ":" + model.HoopQuantityValue + " đai";
            Parameters += " - Giằng " + model.GiangTypeValue + ":" + model.GiangQuantityValue + " giằng";

            BoltWeight *= ((double.Parse(Model.Name.ToString(), culture) + HeadSize) / 1000) * double.Parse(model.BoltQuantityValue.ToString(), culture);

            Id = int.Parse(foundationbolt.FrameSize);
            Model = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault();

            FinalCode += FrameTypeName.Substring(0, 1).ToUpper() + Model.Code;

            string FrameSizeName = Model.Name;
            //   Parameters += " - Hàn thành khung: " + Model.Name;
            double HoopWeight = 0;
            double FrameSizeweight = double.Parse(Model.Code.ToString(), culture);
            //double FrameSizeweight = double.Parse(Model.Code.Substring(1, Model.Code.Length - 1).ToString(), culture);
            HoopWeight = (((FrameSizeweight + BoltDiameterWeight) * 4) / 1000) * double.Parse(model.HoopQuantityValue.ToString(), culture);

            //double GiangWeight = (Math.Pow(Math.Pow((FrameSizeweight + BoltDiameterWeight), 2.0), 0.5) * 2 / 1000) * double.Parse(model.GiangQuantityValue.ToString(), culture);
            double GiangWeight = (((FrameSizeweight + BoltDiameterWeight) * Math.Pow(2, 0.5)) / 1000) * double.Parse(model.GiangQuantityValue.ToString(), culture);

            Id = int.Parse(foundationbolt.HoopType);
            Model = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault();

            double HoopTypeWeight = double.Parse(Model.Weight.ToString(), culture);

            if (HoopTypeWeight != 0)
            {
                HoopWeight *= HoopTypeWeight;
                GiangWeight *= HoopTypeWeight;
            }

            if (Helper.ToVietnameseWithoutAccent(FrameTypeName.ToLower()).Equals("tron"))
            {
                HoopWeight = ((FrameSizeweight + BoltDiameterWeight) / 1000) * 3.14 * 1.05 * double.Parse(model.HoopQuantityValue.ToString(), culture);
                GiangWeight = ((FrameSizeweight + BoltDiameterWeight) / 1000) * 1.05 * double.Parse(model.GiangQuantityValue.ToString(), culture);
                if (HoopTypeWeight != 0)
                {
                    HoopWeight *= HoopTypeWeight;
                    GiangWeight *= HoopTypeWeight;
                }
                //FrameSizeName
                //Parameters += " - Hàn khung tròn: " + (FrameSizeweight + BoltDiameterWeight).ToString();
                Parameters += " - Hàn khung tròn: " + FrameSizeName;
            }
            else
                if (Helper.ToVietnameseWithoutAccent(FrameTypeName.ToLower()).Equals("vuong"))
                {
                    //Parameters += " - Hàn khung vuông: " + (FrameSizeweight * FrameSizeweight).ToString();
                    Parameters += " - Hàn khung vuông: " + FrameSizeName;
                }

            Weight = BoltWeight + HoopWeight + GiangWeight;
            ViewBag.Result = 1;
            ViewBag.FinalCode = FinalCode + model.Code;
            ViewBag.Description = Description;
            ViewBag.Parameters = Parameters;
            ViewBag.Weight = Math.Round(Weight, 2).ToString(culture);
            LoadDataFoundationBolt();
            return Json(new { success = true, FinalCode = ViewBag.FinalCode, Description = ViewBag.Description, Parameters = ViewBag.Parameters, Weight = ViewBag.Weight }, JsonRequestBehavior.AllowGet);
        }

        public void LoadDataFoundationBolt()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            List<String> DisplayName = new List<string>();
            var FoundationBoltHeaderList = dbConn.Select<FoundationBoltHeader>("select * from FoundationBoltHeader order by Id");

            for (int i = 0; i < FoundationBoltHeaderList.Count; i++)
            {
                var TypeId = FoundationBoltHeaderList[i].TypeId;

                var List = dbConn.Select<FoundationBoltOne>(s => s.TypeId == TypeId).OrderBy(s => s.Id);

                DisplayName.Add(FoundationBoltHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.TKGroup = List;
                }

                if (i == 1)
                {
                    ViewBag.BoltDiameter = List;
                }

                if (i == 2)
                {
                    ViewBag.Length = List;
                }

                if (i == 3)
                {
                    ViewBag.FrameSize = List;
                }

                if (i == 4)
                {
                    ViewBag.BoltQuantity = List;
                }

                if (i == 5)
                {
                    ViewBag.HoopType = List;
                }

                if (i == 6)
                {
                    ViewBag.HoopQuantity = List;
                }

                if (i == 7)
                {
                    ViewBag.GiangType = List;
                }
                //---//
                if (i == 8)
                {
                    ViewBag.GiangQuantity = List;
                }
                if (i == 9)
                {
                    ViewBag.FrameType = List;
                }
                if (i == 10)
                {
                    ViewBag.HeadSize = List;
                }
            }
            ViewBag.DisplayName = DisplayName;
            dbConn.Close();
        }

        public ActionResult FoundationBolt_SendMail()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var foundationbolt = TempData["FoundationBoltModel"] as FoundationBoltModel ?? new FoundationBoltModel();
            FoundationBoltTwo foundationbolttwo = new FoundationBoltTwo();

            var Id = int.Parse(foundationbolt.BoltQuantity);
            foundationbolttwo.BoltQuantity = foundationbolt.BoltQuantity;
            foundationbolttwo.BoltQuantityValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.HoopType);
            foundationbolttwo.HoopType = foundationbolt.HoopType;
            foundationbolttwo.HoopTypeValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.HoopQuantity);
            foundationbolttwo.HoopQuantity = foundationbolt.HoopQuantity;
            foundationbolttwo.HoopQuantityValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.GiangType);
            foundationbolttwo.GiangType = foundationbolt.GiangType;
            foundationbolttwo.GiangTypeValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.GiangQuantity);
            foundationbolttwo.GiangQuantity = foundationbolt.GiangQuantity;
            foundationbolttwo.GiangQuantityValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.FrameType);
            foundationbolttwo.FrameType = foundationbolt.FrameType;
            foundationbolttwo.FrameTypeValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            Id = int.Parse(foundationbolt.HeadSize);
            foundationbolttwo.HeadSize = foundationbolt.HeadSize;
            foundationbolttwo.HeadSizeValue = dbConn.Select<FoundationBoltOne>(lho => lho.Id == Id).SingleOrDefault().Name;

            foundationbolttwo.Code = "";

            var CheckID = dbConn.Select<FoundationBoltTwo>(fbt =>
                                      fbt.BoltQuantity == foundationbolttwo.BoltQuantity
                                      && fbt.HoopType == foundationbolttwo.HoopType
                                      && fbt.HoopQuantity == foundationbolttwo.HoopQuantity
                                      && fbt.GiangType == foundationbolttwo.GiangType
                                      && fbt.GiangQuantity == foundationbolttwo.GiangQuantity
                                      && fbt.FrameType == foundationbolttwo.FrameType
                                      && fbt.HeadSize == foundationbolttwo.HeadSize
                                     );

            if (CheckID != null)
            {
                if (CheckID.Count == 0)
                {
                    dbConn.Insert<FoundationBoltTwo>(foundationbolttwo);
                }
            }

            string body = "";
            List<string> eMailTos = new List<string>();

            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();

            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }

            string Subject = "Bộ dịch Bulong-móng chưa được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail.txt"));
            string bodytable = "";
            bodytable += "<tr><td>Số lượng bulong</td><td>" + foundationbolttwo.BoltQuantityValue + "</td></tr>";
            bodytable += "<tr><td>Loại đai</td><td>" + foundationbolttwo.HoopTypeValue + "</td></tr>";
            bodytable += "<tr><td>Số lượng đai</td><td>" + foundationbolttwo.HoopQuantityValue + "</td>";
            bodytable += "<tr><td>Loại giằng</td><td>" + foundationbolttwo.GiangTypeValue + "</td></tr>";
            bodytable += "<tr><td>Số lượng giằng</td><td>" + foundationbolttwo.GiangQuantityValue + "</td></tr>";
            bodytable += "<tr><td>Kiểu hàn khung</td><td>" + foundationbolttwo.FrameTypeValue + "</td></tr>";
            bodytable += "<tr><td>Kích thước bẻ đầu</td><td>" + foundationbolttwo.HeadSizeValue + "</td></tr>";

            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "Bulong-móng");
            if (eMailTos.Count > 0)
            {
                Mail.SendMail(eMailTos, Subject, body);
            }
            //return Json(new { Status = 0, Message = "Success" });
            return Json(new { success = true });
        }
    }
}