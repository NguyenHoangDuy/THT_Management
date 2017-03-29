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

namespace THT.Controllers
{
    public class RodOneController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /RodOne/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listTypeId = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from RodHeader");
                ViewBag.listLoaiCan = dbConn.Select<Utilities_Parameters>("select id,Code as ParamID, Name as Value from rodone where TypeId='B'");
                dbConn.Close();
                return View("_RodOne", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            log4net.Config.XmlConfigurator.Configure();
            string whereCondition = "";
            if (request.Filters.Count > 0)
            {
                whereCondition = new KendoApplyFilter().ApplyFilter(request.Filters[0]);
            }
            request.Filters = null;
            var data = dbConn.Select<RodOne>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Delete(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    //int[] ids = data.Split(new char[] { ',' }).Select(s => int.Parse(s)).ToArray();
                    int i;
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            if (CheckIDInRodTwo(item))
                            {
                                return Json(new { success = false, message = "Không thể xóa. Mã này đang tồn tại trong danh mục nhóm 2." });
                            }
                            dbConn.Delete<RodOne>(s => s.Id == int.Parse(item));
                        }

                        dbTrans.Commit();
                    }
                    return Json(new { success = true });
                }

                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa dữ liệu." });
            }
        }

        private bool CheckIDInRodTwo(string id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();

            var rodtwo = dbConn.Select<RodTwo>(rt =>
                        rt.Style == id
                        || rt.TubeBottomDiameter == id
                        || rt.TubeTopDiameter == id
                        || rt.TubeThickness == id
                        || rt.PrimaryBranchBottomDiameter == id
                        || rt.PrimaryBranchTopDiameter == id
                        || rt.PrimaryBranchThickness == id
                        || rt.ExtraBranchBottomDiameter == id
                        || rt.ExtraBranchTopDiameter == id
                        || rt.ExtraBranchThickness == id
                        || rt.TubeSize == id
                        || rt.AngleRod == id);

            if (rodtwo != null)
            {
                if (rodtwo.Count > 0)
                    return true;
            }
            return false;
        }

        public string ValidateCode(string Code, string RodHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = RodHeader;

            var model = dbConn.Select<RodOne>(ro => ro.TypeId == TypeId && ro.Code == Code);

            var modelheader = dbConn.Select<RodHeader>(rh => rh.TypeId == TypeId).SingleOrDefault();

            if (modelheader.CodeIsWeight)
                if (!Regex.IsMatch(Code, @"^\d*\.?\d*$"))
                {
                    return "Vui lòng nhập " + modelheader.HeaderCode + " là số";
                }

            if (model != null)
            {
                if (model.Count > 0)
                {
                    return modelheader.HeaderCode + " đã tồn tại!";
                }
            }
            dbConn.Close();
            return "";
        }

        public string ValidateCodeEdit(string Code, string RodHeader, int Id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = RodHeader;

            var model = dbConn.Select<RodOne>(ro => ro.TypeId == TypeId && ro.Code == Code && ro.Id != Id);

            var modelheader = dbConn.Select<RodHeader>(rh => rh.TypeId == TypeId).SingleOrDefault();

            if (modelheader.CodeIsWeight)
                if (!Regex.IsMatch(Code, @"^\d*\.?\d*$"))
                {
                    return "Vui lòng nhập " + modelheader.HeaderCode + " là số!";
                }

            if (model != null)
            {
                if (model.Count > 0)
                {
                    return modelheader.HeaderCode + " đã tồn tại!";
                }
            }
            dbConn.Close();
            return "";
        }

        public string ValidateName(string Name, string RodHeader, string RodTypeHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = RodHeader;

            if (RodTypeHeader != null)
            {
                if (RodTypeHeader.ToLower() == "undefined")
                    RodTypeHeader = "";
            }
            else
                RodTypeHeader = "";

            var model = dbConn.Select<RodOne>(ro => ro.TypeId == TypeId && (RodTypeHeader == "" || RodTypeHeader == ro.TypeCode));

            var modelheader = dbConn.Select<RodHeader>(rh => rh.TypeId == TypeId).SingleOrDefault();


            if (modelheader.NameIsWeight)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$") && !(Helper.ToVietnameseWithoutAccent(Name.ToLower()).Equals("khong")))
                {
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                }

            if (modelheader.NameIsNumber)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$") && !(Helper.ToVietnameseWithoutAccent(Name.ToLower()).Equals("khong")))
                {
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                }

            if (model != null)
            {
                model = model.Where(s => (Helper.ToVietnameseWithoutAccent(s.Name.ToLower())).Equals(Helper.ToVietnameseWithoutAccent(Name.Trim().ToLower()))).ToList();
                if (model.Count > 0)
                {
                    return modelheader.HeaderName + " đã tồn tại!";
                }
            }

            dbConn.Close();
            return "";
        }

        public bool CheckFile(string FileName)
        {
            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            foreach (var item in formats)
            {
                if (FileName.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult Create(RodOne item)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            try
            {
                var isExist = dbConn.SingleOrDefault<RodOne>("Id={0}", item.Id);

                if (userAsset.ContainsKey("Insert") && userAsset["Insert"] && isExist == null)
                {

                    var SelectedHeader = dbConn.Select<RodHeader>(lho => lho.TypeId == item.TypeId).SingleOrDefault();

                    var myModel = new RodOne();
                    myModel.TypeId = item.TypeId;
                    myModel.Code = item.Code;
                    myModel.Name = item.Name;
                    myModel.Description = item.Description;

                    if (SelectedHeader.HaveImage)
                    {
                        myModel.TypeCode = item.TypeCode;
                        var File = Request.Files["FileUpload"];
                        if (File != null)
                        {
                            var Folder = "";
                            var FileName = "";
                            if (File.ContentLength > 0)
                            {
                                if (!CheckFile(File.FileName))
                                {
                                    return Json(new { success = false, message = "Vui lòng chọn file ảnh" });
                                }

                                Folder = ConfigurationManager.AppSettings["Upload"];
                                if (Folder != System.IO.Path.GetFullPath(Folder))
                                {
                                    Folder = System.Web.HttpContext.Current.Server.MapPath(Folder);
                                }

                                if (!Directory.Exists(Folder))
                                {
                                    Directory.CreateDirectory(Folder);
                                }
                                FileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
                                File.SaveAs(Path.Combine(Folder, FileName));
                                myModel.ImagePath = FileName;
                            }
                        }
                    }

                    if (SelectedHeader.HaveWeight)
                    {
                        myModel.Weight = item.Weight;
                    }
                    if (SelectedHeader.HaveCode == false)
                    {
                        myModel.Code = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                    }
                    if (SelectedHeader.HaveWeight == false)
                    {
                        myModel.Weight = "0";
                    }

                    string messgae = "";
                    messgae = ValidateCode(myModel.Code, item.TypeId);

                    if (!string.IsNullOrEmpty(messgae))
                    {
                        return Json(new { success = false, message = messgae });
                    }

                    messgae = "";
                    messgae = ValidateName(myModel.Name, item.TypeId, item.TypeCode);

                    if (!string.IsNullOrEmpty(messgae))
                    {
                        return Json(new { success = false, message = messgae });
                    }

                    dbConn.Insert<RodOne>(myModel);

                    return Json(new { success = true, Id = item.Id });
                }
                else if (userAsset.ContainsKey("Update") && userAsset["Update"] && isExist != null)
                {

                    isExist.Code = item.Code;
                    isExist.Name = item.Name;

                    var SelectedHeader = dbConn.Select<RodHeader>(lho => lho.TypeId == item.TypeId).SingleOrDefault();

                    string messgae = "";
                    messgae = ValidateCodeEdit(item.Code, item.TypeId, item.Id);

                    if (!string.IsNullOrEmpty(messgae))
                    {
                        return Json(new { success = false, message = messgae });
                    }

                    messgae = "";
                    messgae = ValidateNameEdit(item.Name, item.TypeId, item.TypeCode, item.Id);

                    if (!string.IsNullOrEmpty(messgae))
                    {
                        return Json(new { success = false, message = messgae });
                    }


                    if (SelectedHeader.HaveWeight)
                    {
                        isExist.Weight = item.Weight;
                    }

                    if (SelectedHeader.HaveImage)
                    {
                        isExist.TypeCode = item.TypeCode;
                        var File = Request.Files["FileUpload"];
                        if (File != null)
                        {
                            var Folder = "";
                            var FileName = "";
                            if (File.ContentLength > 0)
                            {
                                if (!CheckFile(File.FileName))
                                {
                                    return Json(new { success = false, message = "Vui lòng chọn file ảnh" });
                                }
                                Folder = ConfigurationManager.AppSettings["Upload"];
                                if (Folder != System.IO.Path.GetFullPath(Folder))
                                {
                                    Folder = System.Web.HttpContext.Current.Server.MapPath(Folder);
                                }
                                FileName = Guid.NewGuid() + Path.GetExtension(File.FileName);
                                File.SaveAs(Path.Combine(Folder, FileName));
                                isExist.ImagePath = FileName;
                            }
                        }
                    }
                    UpdateRodTwo(isExist);
                    dbConn.Update<RodOne>(isExist);
                    return Json(new { success = true });
                }
                else
                    return Json(new { success = false, message = "Bạn không có quyền" });

            }
            catch (Exception e)
            {
                log.Error("Customer - Create - " + e.Message);
                return Json(new { success = false, message = e.Message });
            }
            finally { dbConn.Close(); }
        }

        public string ValidateNameEdit(string Name, string RodHeader, string TypeCode, int Id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = RodHeader;

            if (TypeCode != null)
            {
                if (TypeCode.ToLower() == "undefined")
                    TypeCode = "";
            }
            else
                TypeCode = "";

            var model = dbConn.Select<RodOne>(ro => ro.TypeId == TypeId && (TypeCode == "" || TypeCode == ro.TypeCode) && ro.Id != Id);

            var modelheader = dbConn.Select<RodHeader>(rh => rh.TypeId == TypeId).SingleOrDefault();

            if (modelheader.NameIsWeight)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$"))
                {
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                }

            if (modelheader.NameIsNumber)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$") && !(Helper.ToVietnameseWithoutAccent(Name.ToLower()).Equals("khong")))
                {
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                }

            if (model != null)
            {
                model = model.Where(s => (Helper.ToVietnameseWithoutAccent(s.Name.ToLower())).Equals(Helper.ToVietnameseWithoutAccent(Name.Trim().ToLower()))).ToList();
                if (model.Count > 0)
                {
                    return modelheader.HeaderName + " đã tồn tại!";
                }
            }
            dbConn.Close();
            return "";
        }

        public string ValidateTypeCode(string TypeCode, string RodHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = RodHeader;

            var modelheader = dbConn.Select<RodHeader>(rh => rh.TypeId == TypeId).SingleOrDefault();

            if (modelheader != null)
            {
                if (modelheader.HaveImage && TypeCode.Trim() == "")
                    return "Vui lòng chọn loại cần!";
            }
            dbConn.Close();
            return "";
        }

        private void UpdateRodTwo(RodOne ro)
        {
            var dbConn = new OrmliteConnection().openConn();
            string Id = ro.Id.ToString();

            dbConn.Update<RodTwo>(set: "StyleValue='" + ro.Name + "'",
                                       where: "Style='" + Id + "'");

            dbConn.Update<RodTwo>(set: "TubeBottomDiameterValue='" + ro.Name + "'",
                                      where: "TubeBottomDiameter='" + Id + "'");

            dbConn.Update<RodTwo>(set: "TubeTopDiameterValue='" + ro.Name + "'",
                                      where: "TubeTopDiameter='" + Id + "'");

            dbConn.Update<RodTwo>(set: "TubeThicknessValue='" + ro.Name + "'",
                                      where: "TubeThickness='" + Id + "'");

            dbConn.Update<RodTwo>(set: "PrimaryBranchBottomDiameterValue='" + ro.Name + "'",
                                      where: "PrimaryBranchBottomDiameter='" + Id + "'");

            dbConn.Update<RodTwo>(set: "PrimaryBranchTopDiameterValue='" + ro.Name + "'",
                                      where: "PrimaryBranchTopDiameter='" + Id + "'");


            dbConn.Update<RodTwo>(set: "PrimaryBranchThicknessValue='" + ro.Name + "'",
                                      where: "PrimaryBranchThickness='" + Id + "'");

            dbConn.Update<RodTwo>(set: "ExtraBranchBottomDiameterValue='" + ro.Name + "'",
                                      where: "ExtraBranchBottomDiameter='" + Id + "'");

            dbConn.Update<RodTwo>(set: "ExtraBranchTopDiameterValue='" + ro.Name + "'",
                                      where: "ExtraBranchTopDiameter='" + Id + "'");

            dbConn.Update<RodTwo>(set: "ExtraBranchThicknessValue='" + ro.Name + "'",
                                      where: "ExtraBranchThickness='" + Id + "'");

            dbConn.Update<RodTwo>(set: "TubeSizeValue='" + ro.Name + "'",
                                      where: "TubeSize='" + Id + "'");

            dbConn.Update<RodTwo>(set: "AngleRodValue='" + ro.Name + "'",
                                      where: "AngleRod='" + Id + "'");

            dbConn.Close();
        }

        public ActionResult GetRodHeader(string TypeId)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<RodHeader>(s => s.TypeId == TypeId).SingleOrDefault();
                var listLoaiCan = dbConn.Select<Utilities_Parameters>("select id as ID,Code as ParamID, Name as Value from rodone where TypeId='B'");
                return Json(new { success = true, data = data, listLoaiCan = listLoaiCan }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}