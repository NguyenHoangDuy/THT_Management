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
    public class LightHeadOneController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /LightHeadOne/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listProductType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.ProductTYPE);
                dict["listTypeId"] = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from LightHeadHeader");
                dbConn.Close();
                return View("_LightHeadOne", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public string ValidateWeight(string Weight, string LightHeadHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = LightHeadHeader;

            var modelheader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();

            if (modelheader.HaveWeight)
                if (!Regex.IsMatch(Weight, @"^\d*\.?\d*$"))
                {
                    //return Json("Vui lòng nhập số!", JsonRequestBehavior.AllowGet);
                    return "Vui lòng nhập trọng lượng là số!";
                }
            return "";
        }

        public string ValidateName(string Name, string LightHeadHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = LightHeadHeader;

            //var model = (from lho in db.LightHeadOnes
            //             where lho.TypeId == TypeId
            //             select lho).ToList();

            var model = dbConn.Select<LightHeadOne>(lho => lho.TypeId == TypeId);

            //var modelheader = (from lho in db.LightHeadHeaders
            //                   where lho.TypeId == TypeId
            //                   select lho).FirstOrDefault();

            var modelheader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();


            if (modelheader.NameIsWeight)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$") && !(Helper.ToVietnameseWithoutAccent(Name.ToLower()).Equals("khong")))
                {
                    //return Json("Vui lòng nhập số!", JsonRequestBehavior.AllowGet);
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                }

            if (model != null)
            {
                model = model.Where(s => (Helper.ToVietnameseWithoutAccent(s.Name.ToLower())).Equals(Helper.ToVietnameseWithoutAccent(Name.Trim().ToLower()))).ToList();
                if (model.Count > 0)
                {
                    return modelheader.HeaderName + " đã tồn tại!";
                    //return Json("Tên đã tồn tại!", JsonRequestBehavior.AllowGet);
                }
            }

            dbConn.Close();
            //return Json(true, JsonRequestBehavior.AllowGet);
            return "";
        }

        public string ValidateCode(string Code, string LightHeadHeader)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = LightHeadHeader;

            //var model = (from lho in db.LightHeadOnes
            //             where lho.TypeId == TypeId && lho.Code == Code
            //             select lho).ToList();

            var model = dbConn.Select<LightHeadOne>(lho => lho.TypeId == TypeId && lho.Code == Code);

            //var modelheader = (from lho in db.LightHeadHeaders
            //                   where lho.TypeId == TypeId
            //                   select lho).FirstOrDefault();

            var modelheader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();

            if (modelheader.CodeIsWeight)
                if (!Regex.IsMatch(Code, @"^\d*\.?\d*$"))
                {
                    return "Vui lòng nhập " + modelheader.HeaderCode + " là số";
                    //return Json("Vui lòng nhập số!", JsonRequestBehavior.AllowGet);
                }

            if (model != null)
            {
                if (model.Count > 0)
                {
                    return modelheader.HeaderCode + " đã tồn tại!";
                    //return Json("Mã đã tồn tại!", JsonRequestBehavior.AllowGet);
                }
            }
            dbConn.Close();
            return "";
            //return Json(true, JsonRequestBehavior.AllowGet);
        }

        public string ValidateCodeEdit(string Code, string LightHeadHeader, int Id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = LightHeadHeader;

            //var model = (from lho in db.LightHeadOnes
            //             where lho.TypeId == TypeId && lho.Code == Code && lho.Id != Id
            //             select lho).ToList();

            var model = dbConn.Select<LightHeadOne>(lho => lho.TypeId == TypeId && lho.Code == Code && lho.Id != Id);

            //var modelheader = (from lho in db.LightHeadHeaders
            //                   where lho.TypeId == TypeId
            //                   select lho).FirstOrDefault();

            var modelheader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();

            if (modelheader.CodeIsWeight)
                if (!Regex.IsMatch(Code, @"^\d*\.?\d*$"))
                {
                    //return Json("Vui lòng nhập số!", JsonRequestBehavior.AllowGet);
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

        public string ValidateNameEdit(string Name, string LightHeadHeader, int Id)
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var TypeId = LightHeadHeader;

            //var model = (from lho in db.LightHeadOnes
            //             where lho.TypeId == TypeId && lho.Id != Id
            //             select lho).ToList();
            var model = dbConn.Select<LightHeadOne>(lho => lho.TypeId == TypeId && lho.Id != Id);

            //var modelheader = (from lho in db.LightHeadHeaders
            //                   where lho.TypeId == TypeId
            //                   select lho).FirstOrDefault();

            var modelheader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();

            if (modelheader.NameIsWeight)
                if (!Regex.IsMatch(Name, @"^\d*\.?\d*$"))
                {
                    return "Vui lòng nhập " + modelheader.HeaderName + " là số!";
                    //return Json("Vui lòng nhập số!", JsonRequestBehavior.AllowGet);
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

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LightHeadOne> list, string TypeId)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                if (list != null)
                {
                    try
                    {
                        using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                        {
                            foreach (var item in list)
                            {
                                var SelectedHeader = dbConn.Select<LightHeadHeader>(lho => lho.TypeId == TypeId).SingleOrDefault();

                                var myModel = new LightHeadOne();
                                myModel.TypeId = TypeId;
                                myModel.Code = item.Code;
                                myModel.Name = item.Name;
                                myModel.Description = item.Description;
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
                                messgae = ValidateCode(myModel.Code, TypeId);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                messgae = "";
                                messgae = ValidateName(myModel.Name, TypeId);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                messgae = "";
                                messgae = ValidateWeight(myModel.Weight, TypeId);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                dbConn.Insert<LightHeadOne>(myModel);
                            }
                            dbTrans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("error", ex.Message);
                        return Json(list.ToDataSourceResult(request, ModelState));
                    }
                }
                dbConn.Close();
                return Json(new { sussess = true });
            }
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền thêm mới.");
                return Json(list.ToDataSourceResult(request, ModelState));
            }
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LightHeadOne> list, string TypeId)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                if (list != null)
                {
                    try
                    {
                        using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                        {
                            foreach (var item in list)
                            {
                                string messgae = "";
                                messgae = ValidateCodeEdit(item.Code, TypeId, item.Id);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                messgae = "";
                                messgae = ValidateNameEdit(item.Name, TypeId, item.Id);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                messgae = "";
                                messgae = ValidateWeight(item.Weight, TypeId);

                                if (!string.IsNullOrEmpty(messgae))
                                {
                                    ModelState.AddModelError("", messgae);
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                dbConn.Update<LightHeadOne>(item);
                                UpdateLightHeadTwo(item);
                            }
                            dbTrans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("error", ex.Message);
                        return Json(list.ToDataSourceResult(request, ModelState));
                    }
                }
                dbConn.Close();
                return Json(new { sussess = true });
            }
            else
            {
                ModelState.AddModelError("error", "Bạn không có quyền cập nhật.");
                return Json(list.ToDataSourceResult(request, ModelState));
            }
        }

        private void UpdateLightHeadTwo(LightHeadOne lho)
        {
            var dbConn = new OrmliteConnection().openConn();
            string Id = lho.Id.ToString();

            dbConn.Update<LightHeadTwo>(set: "BoltCenterValue='" + lho.Name + "'",
                                        where: "BoltCenter='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "BoltSizeValue='" + lho.Name + "'",
                                       where: "BoltSize='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "EpMoValue='" + lho.Name + "'",
                                      where: "EpMo='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "HeadHeightValue='" + lho.Name + "'",
                                      where: "HeadHeight='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "HeadSizeValue='" + lho.Name + "'",
                                                  where: "HeadSize='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "TendonSizeValue='" + lho.Name + "'",
                                                  where: "TendonSize='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "TendonNumberValue='" + lho.Name + "'",
                                                  where: "TendonNumber='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "TubeSizeValue='" + lho.Name + "'",
                                                  where: "TubeSize='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "RodSizeValue='" + lho.Name + "'",
                                                  where: "RodSize='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "HillValue='" + lho.Name + "'",
                                                  where: "Hill='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "HingeValue='" + lho.Name + "'",
                                                  where: "Hinge='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "MuzzleHeadValue='" + lho.Name + "'",
                                                  where: "MuzzleHead='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "PlinthTypeValue='" + lho.Name + "'",
                                                  where: "PlinthType='" + Id + "'");

            dbConn.Update<LightHeadTwo>(set: "SilverLiningValue='" + lho.Name + "'",
                                                  where: "SilverLining='" + Id + "'");
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
            var data = dbConn.Select<LightHeadOne>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult GetLightHeadHeader(string TypeId)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = dbConn.Select<LightHeadHeader>(s => s.TypeId == TypeId).SingleOrDefault();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
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
                            if (CheckIDInLightHeadTwo(item))
                            {
                                return Json(new { success = false, message = "Không thể xóa. Mã này đang tồn tại trong danh mục nhóm 2." });
                            }
                            dbConn.Delete<LightHeadOne>(s => s.Id == int.Parse(item));
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

        private bool CheckIDInLightHeadTwo(string id)
        {
            var dbConn = new OrmliteConnection().openConn();

            var lightheadtwo = dbConn.Select<LightHeadTwo>(lht => lht.BoltCenter == id
                                || lht.BoltCenter == id
                                || lht.BoltSize == id
                                || lht.EpMo == id
                                || lht.HeadHeight == id
                                || lht.HeadSize == id
                                || lht.TendonSize == id
                                || lht.TendonNumber == id
                                || lht.TubeSize == id
                                || lht.RodSize == id
                                || lht.Hill == id
                                || lht.Hinge == id
                                || lht.MuzzleHead == id
                                || lht.PlinthType == id);
            if (lightheadtwo != null)
            {
                if (lightheadtwo.Count > 0)
                    return true;
            }
            dbConn.Close();
            return false;
        }

    }
}