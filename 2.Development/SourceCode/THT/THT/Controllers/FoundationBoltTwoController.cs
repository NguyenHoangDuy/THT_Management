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
    public class FoundationBoltTwoController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /FoundationBoltTwo/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listProductType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.ProductTYPE);
                dict["listTypeId"] = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from FoundationBoltHeader");
                LoadData();
                dbConn.Close();
                return View("_FoundationBoltTwo", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FoundationBoltTwo> list, string data)
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
                                var model = dbConn.SingleOrDefault<FoundationBoltTwo>("SELECT * FROM dbo.FoundationBoltTwo where Id=" + item.Id + " ORDER BY Id DESC");
                                if (String.IsNullOrEmpty(model.Code) && listdata.Contains(item.Id.ToString()) && !String.IsNullOrEmpty(item.Code))
                                {
                                    var model1 = dbConn.Select<FoundationBoltTwo>(fbt => fbt.Code == item.Code);

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

                                    //var checkID = dbConn.SingleOrDefault<FoundationBoltTwo>("SELECT Code, Id FROM dbo.FoundationBoltTwo where Code <>'' ORDER BY Id DESC");
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

                                    dbConn.Update<FoundationBoltTwo>(set: "Code = '" + item.Code + "'", where: "Id = " + item.Id);
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
            var data = dbConn.Select<FoundationBoltTwo>(whereCondition).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<FoundationBoltTwo> list, string data)
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
                                if (string.IsNullOrEmpty(item.BoltQuantity)
                                    || string.IsNullOrEmpty(item.HoopType)
                                    || string.IsNullOrEmpty(item.HoopQuantity)
                                    || string.IsNullOrEmpty(item.GiangType)
                                    || string.IsNullOrEmpty(item.GiangQuantity)
                                    || string.IsNullOrEmpty(item.FrameType)
                                    || string.IsNullOrEmpty(item.HeadSize)
                                    || string.IsNullOrEmpty(item.Code)
                                    )
                                {
                                    ModelState.AddModelError("", "Vui lòng chọn đầy đủ thông tin");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                var model = dbConn.Select<FoundationBoltTwo>(fbt =>
                                     fbt.BoltQuantity == item.BoltQuantity
                                     && fbt.HoopType == item.HoopType
                                     && fbt.HoopQuantity == item.HoopQuantity
                                     && fbt.GiangType == item.GiangType
                                     && fbt.GiangQuantity == item.GiangQuantity
                                     && fbt.FrameType == item.FrameType
                                     && fbt.HeadSize == item.HeadSize
                                    );

                                if (model != null)
                                {
                                    if (model.Count > 0)
                                    {
                                        ModelState.AddModelError("", "Các thông tin danh mục nhóm 2 đã tồn tại !!!");
                                        return Json(list.ToDataSourceResult(request, ModelState));
                                    }
                                }

                                model = dbConn.Select<FoundationBoltTwo>(fbt => fbt.Code == item.Code);

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

                                //var checkID = dbConn.SingleOrDefault<FoundationBoltTwo>("SELECT Code, Id FROM dbo.FoundationBoltTwo ORDER BY Id DESC");
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

                                FoundationBoltTwo dbfoundbolttwo = new FoundationBoltTwo();
                                dbfoundbolttwo.BoltQuantity = item.BoltQuantity;
                                dbfoundbolttwo.HoopType = item.HoopType;
                                dbfoundbolttwo.HoopQuantity = item.HoopQuantity;
                                dbfoundbolttwo.GiangType = item.GiangType;
                                dbfoundbolttwo.GiangQuantity = item.GiangQuantity;
                                dbfoundbolttwo.FrameType = item.FrameType;
                                dbfoundbolttwo.HeadSize = item.HeadSize;
                                dbfoundbolttwo.Code = item.Code;

                                var Id = int.Parse(dbfoundbolttwo.BoltQuantity);
                                dbfoundbolttwo.BoltQuantityValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.HoopType);
                                dbfoundbolttwo.HoopTypeValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.HoopQuantity);
                                dbfoundbolttwo.HoopQuantityValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.GiangType);
                                dbfoundbolttwo.GiangTypeValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.GiangQuantity);
                                dbfoundbolttwo.GiangQuantityValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.FrameType);
                                dbfoundbolttwo.FrameTypeValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbfoundbolttwo.HeadSize);
                                dbfoundbolttwo.HeadSizeValue = dbConn.Select<FoundationBoltOne>(fbt => fbt.Id == Id).SingleOrDefault().Name;

                                dbConn.Insert<FoundationBoltTwo>(dbfoundbolttwo);
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
                            dbConn.Delete<FoundationBoltTwo>(s => s.Id == int.Parse(item));
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

            var FoundationBoltHeaderList = dbConn.Select<FoundationBoltHeader>(fbh => fbh.HaveCode == false);

            for (int i = 0; i < FoundationBoltHeaderList.Count; i++)
            {
                var TypeId = FoundationBoltHeaderList[i].TypeId;

                var List = dbConn.Query<Utilities_Parameters>("select Id as ID,Cast(Id as nvarchar(50)) as ParamID, Name as Value from FoundationBoltOne where TypeId='" + TypeId + "' order by Id");

                DisplayName.Add(FoundationBoltHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.BoltQuantity = List;
                }
                if (i == 1)
                {
                    ViewBag.HoopType = List;
                }
                if (i == 2)
                {
                    ViewBag.HoopQuantity = List;
                }
                if (i == 3)
                {
                    ViewBag.GiangType = List;
                }
                if (i == 4)
                {
                    ViewBag.GiangQuantity = List;
                }
                if (i == 5)
                {
                    ViewBag.FrameType = List;
                }
                if (i == 6)
                {
                    ViewBag.HeadSize = List;
                }

            }
            ViewBag.DisplayName = DisplayName;
        }
    }
}