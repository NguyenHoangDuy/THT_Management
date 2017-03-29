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
    public class LightHeadTwoController : CustomController
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
                LoadData();
                dbConn.Close();
                return View("_LightHeadTwo", dict);
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
            var data = dbConn.Select<LightHeadTwo>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LightHeadTwo> list, string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Insert") && userAsset["Insert"])
            {
                string[] separators = { "@@" };
                var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (list != null && listdata.Length > 0)
                {
                    try
                    {
                        using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                        {
                            foreach (var item in list)
                            {
                                var model = dbConn.SingleOrDefault<LightHeadTwo>("SELECT * FROM dbo.LightHeadTwo where Id=" + item.Id + " ORDER BY Id DESC");
                                if (String.IsNullOrEmpty(model.Code) && listdata.Contains(item.Id.ToString()) && !String.IsNullOrEmpty(item.Code))
                                {
                                    var model1 = dbConn.Select<LightHeadTwo>(lht => lht.Code == item.Code);

                                    if (model1 != null)
                                    {
                                        if (model1.Count > 0)
                                        {
                                            ViewBag.Result = -1;
                                            ViewBag.Message = "Mã nhóm 2 đã tồn tại !!!";
                                            ModelState.AddModelError("", "Mã nhóm 2 đã tồn tại !!!");
                                            return Json(list.ToDataSourceResult(request, ModelState));
                                        }
                                    }

                                    //var checkID = dbConn.SingleOrDefault<LightHeadTwo>("SELECT Code, Id FROM dbo.LightHeadTwo where Code <>'' ORDER BY Id DESC");
                                    //if (checkID != null)
                                    //{
                                    //    int nextNo;
                                    //    bool result = Int32.TryParse(checkID.Code, out nextNo);
                                    //    if (result)
                                    //    {
                                    //        nextNo++;
                                    //        item.Code = nextNo.ToString();
                                    //    }
                                    //    else
                                    //    {
                                    //        nextNo = int.Parse(checkID.Code.Substring(1, checkID.Code.Length - 1)) + 1;
                                    //        if (nextNo <= 999)
                                    //            item.Code = "J" + String.Format("{0:000}", nextNo);
                                    //        else
                                    //            item.Code = nextNo.ToString();
                                    //    }
                                    //}
                                    //else
                                    //    item.Code = "J001";

                                    dbConn.Update<LightHeadTwo>(set: "Code = '" + item.Code + "'", where: "Id = " + item.Id);

                                }
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

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<LightHeadTwo> list, string data)
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

                                if (string.IsNullOrEmpty(item.BoltCenter)
                                    || string.IsNullOrEmpty(item.BoltSize)
                                    || string.IsNullOrEmpty(item.EpMo)
                                    || string.IsNullOrEmpty(item.HeadHeight)
                                    || string.IsNullOrEmpty(item.HeadSize)
                                    || string.IsNullOrEmpty(item.TendonSize)
                                    || string.IsNullOrEmpty(item.TendonNumber)
                                    || string.IsNullOrEmpty(item.TubeSize)
                                    || string.IsNullOrEmpty(item.RodSize)
                                    || string.IsNullOrEmpty(item.Hill)
                                    || string.IsNullOrEmpty(item.Hinge)
                                    || string.IsNullOrEmpty(item.MuzzleHead)
                                    || string.IsNullOrEmpty(item.PlinthType)
                                    || string.IsNullOrEmpty(item.SilverLining)
                                    || string.IsNullOrEmpty(item.Code)
                                    )
                                {
                                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                var model = dbConn.Select<LightHeadTwo>(lht => lht.BoltCenter == item.BoltCenter
                                             && lht.BoltSize == item.BoltSize
                                             && lht.EpMo == item.EpMo
                                             && lht.HeadHeight == item.HeadHeight
                                             && lht.HeadSize == item.HeadSize
                                             && lht.TendonSize == item.TendonSize
                                             && lht.TendonNumber == item.TendonNumber
                                             && lht.TubeSize == item.TubeSize
                                             && lht.RodSize == item.RodSize
                                             && lht.Hill == item.Hill
                                             && lht.Hinge == item.Hinge
                                             && lht.MuzzleHead == item.MuzzleHead
                                             && lht.PlinthType == item.PlinthType
                                             && lht.SilverLining == item.SilverLining
                                    );

                                if (model != null)
                                {
                                    if (model.Count > 0)
                                    {
                                        ModelState.AddModelError("", "Các thông tin danh mục nhóm 2 đã tồn tại !!!");
                                        return Json(list.ToDataSourceResult(request, ModelState));
                                    }
                                }

                                model = dbConn.Select<LightHeadTwo>(lht => lht.Code == item.Code);

                                if (model != null)
                                {
                                    if (model.Count > 0)
                                    {
                                        ViewBag.Result = -1;
                                        ViewBag.Message = "Mã nhóm 2 đã tồn tại !!!";
                                        ModelState.AddModelError("", "Mã nhóm 2 đã tồn tại !!!");
                                        return Json(list.ToDataSourceResult(request, ModelState));
                                    }
                                }

                                //var checkID = dbConn.SingleOrDefault<LightHeadTwo>("SELECT Code, Id FROM dbo.LightHeadTwo where code <>'' ORDER BY Id DESC");
                                //if (checkID != null)
                                //{
                                //    int nextNo;
                                //    bool result = Int32.TryParse(checkID.Code, out nextNo);
                                //    if (result)
                                //    {
                                //        nextNo++;
                                //        item.Code = nextNo.ToString();
                                //    }
                                //    else
                                //    {
                                //        nextNo = int.Parse(checkID.Code.Substring(1, checkID.Code.Length - 1)) + 1;
                                //        if (nextNo <= 999)
                                //            item.Code = "J" + String.Format("{0:000}", nextNo);
                                //        else
                                //            item.Code = nextNo.ToString();
                                //    }
                                //}
                                //else
                                //    item.Code = "J001";

                                LightHeadTwo dblightheadtwo = new LightHeadTwo();
                                dblightheadtwo.BoltCenter = item.BoltCenter;
                                dblightheadtwo.BoltSize = item.BoltSize;
                                dblightheadtwo.EpMo = item.EpMo;
                                dblightheadtwo.HeadHeight = item.HeadHeight;
                                dblightheadtwo.HeadSize = item.HeadSize;
                                dblightheadtwo.TendonSize = item.TendonSize;
                                dblightheadtwo.TendonNumber = item.TendonNumber;
                                dblightheadtwo.TubeSize = item.TubeSize;
                                dblightheadtwo.RodSize = item.RodSize;
                                dblightheadtwo.Hill = item.Hill;
                                dblightheadtwo.Hinge = item.Hinge;
                                dblightheadtwo.MuzzleHead = item.MuzzleHead;
                                dblightheadtwo.PlinthType = item.PlinthType;
                                dblightheadtwo.SilverLining = item.SilverLining;
                                dblightheadtwo.Code = item.Code;

                                var Id = int.Parse(dblightheadtwo.BoltCenter);
                                dblightheadtwo.BoltCenterValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.BoltSize);
                                dblightheadtwo.BoltSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.EpMo);
                                dblightheadtwo.EpMoValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.HeadHeight);
                                dblightheadtwo.HeadHeightValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.HeadSize);
                                dblightheadtwo.HeadSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.TendonSize);
                                dblightheadtwo.TendonSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.TendonNumber);
                                dblightheadtwo.TendonNumberValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.TubeSize);
                                dblightheadtwo.TubeSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.RodSize);
                                dblightheadtwo.RodSizeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.Hill);
                                dblightheadtwo.HillValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.Hinge);
                                dblightheadtwo.HingeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.MuzzleHead);
                                dblightheadtwo.MuzzleHeadValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.PlinthType);
                                dblightheadtwo.PlinthTypeValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dblightheadtwo.SilverLining);
                                dblightheadtwo.SilverLiningValue = dbConn.Select<LightHeadOne>(lho => lho.Id == Id).SingleOrDefault().Name;

                                dbConn.Insert<LightHeadTwo>(dblightheadtwo);
                            }
                            dbTrans.Commit();
                        }
                        return Json(new { sussess = true });
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

        public ActionResult Delete(string data)
        {
            var dbConn = new OrmliteConnection().openConn();
            if (userAsset.ContainsKey("Delete") && userAsset["Delete"])
            {
                try
                {
                    string[] separators = { "@@" };
                    var listdata = data.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            dbConn.Delete<LightHeadTwo>(s => s.Id == int.Parse(item));
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

        public void LoadData()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            List<String> DisplayName = new List<string>();

            var LightHeadHeaderList = dbConn.Select<LightHeadHeader>(lhh => lhh.HaveCode == false);

            for (int i = 0; i < LightHeadHeaderList.Count; i++)
            {
                var TypeId = LightHeadHeaderList[i].TypeId;

                var List = dbConn.Query<Utilities_Parameters>("select Id as ID,Cast(Id as nvarchar(50)) as ParamID, Name as Value from LightHeadOne where TypeId='" + TypeId + "' order by Id");

                DisplayName.Add(LightHeadHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.BoltCenter = List;
                }
                if (i == 1)
                {
                    ViewBag.BoltSize = List;
                }
                if (i == 2)
                {
                    ViewBag.EpMo = List;
                }
                if (i == 3)
                {
                    ViewBag.HeadHeight = List;
                }
                if (i == 4)
                {
                    ViewBag.HeadSize = List;
                }
                if (i == 5)
                {
                    ViewBag.TendonSize = List;
                }
                if (i == 6)
                {
                    ViewBag.TendonNumber = List;
                }
                if (i == 7)
                {
                    ViewBag.TubeSize = List;
                }
                if (i == 8)
                {
                    ViewBag.RodSize = List;
                }
                if (i == 9)
                {
                    ViewBag.Hill = List;
                }
                if (i == 10)
                {
                    ViewBag.Hinge = List;
                }
                if (i == 11)
                {
                    ViewBag.MuzzleHead = List;
                }
                if (i == 12)
                {
                    ViewBag.PlinthType = List;
                }
                if (i == 13)
                {
                    ViewBag.SilverLining = List;
                }
            }
            ViewBag.DisplayName = DisplayName;
        }
    }
}