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
    public class RodTwoController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /RodTwo/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listProductType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.ProductTYPE);
                dict["listTypeId"] = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from RodHeader");
                LoadData();
                dbConn.Close();
                return View("_RodTwo", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<RodTwo> list, string data)
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
                                var model = dbConn.SingleOrDefault<RodTwo>("SELECT * FROM dbo.RodTwo where Id=" + item.Id + " ORDER BY Id DESC");
                                if (String.IsNullOrEmpty(model.Code) && listdata.Contains(item.Id.ToString()) && !String.IsNullOrEmpty(item.Code))
                                {
                                    var model1 = dbConn.Select<RodTwo>(rt => rt.Code == item.Code);

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

                                    //var checkID = dbConn.SingleOrDefault<RodTwo>("SELECT Code, Id FROM dbo.RodTwo where Code <>'' ORDER BY Id DESC");
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

                                    dbConn.Update<RodTwo>(set: "Code = '" + item.Code + "'", where: "Id = " + item.Id);
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

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<RodTwo> list, string data)
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
                                if (string.IsNullOrEmpty(item.Style)
                                    || string.IsNullOrEmpty(item.TubeBottomDiameter)
                                    || string.IsNullOrEmpty(item.TubeTopDiameter)
                                    || string.IsNullOrEmpty(item.TubeThickness)
                                    || string.IsNullOrEmpty(item.PrimaryBranchBottomDiameter)
                                    || string.IsNullOrEmpty(item.PrimaryBranchTopDiameter)
                                    || string.IsNullOrEmpty(item.PrimaryBranchThickness)
                                    || string.IsNullOrEmpty(item.ExtraBranchBottomDiameter)
                                    || string.IsNullOrEmpty(item.ExtraBranchTopDiameter)
                                    || string.IsNullOrEmpty(item.ExtraBranchThickness)
                                    || string.IsNullOrEmpty(item.TubeSize)
                                    || string.IsNullOrEmpty(item.AngleRod)
                                    || string.IsNullOrEmpty(item.Code)
                                    )
                                {
                                    ModelState.AddModelError("", "Vui lòng chọn đầy đủ thông tin");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                var model = dbConn.Select<RodTwo>(rt =>
                                      rt.Style == item.Style
                                      && rt.TubeBottomDiameter == item.TubeBottomDiameter
                                      && rt.TubeTopDiameter == item.TubeTopDiameter
                                      && rt.TubeThickness == item.TubeThickness
                                      && rt.PrimaryBranchBottomDiameter == item.PrimaryBranchBottomDiameter
                                      && rt.PrimaryBranchTopDiameter == item.PrimaryBranchTopDiameter
                                      && rt.PrimaryBranchThickness == item.PrimaryBranchThickness
                                      && rt.ExtraBranchBottomDiameter == item.ExtraBranchBottomDiameter
                                      && rt.ExtraBranchTopDiameter == item.ExtraBranchTopDiameter
                                      && rt.ExtraBranchThickness == item.ExtraBranchThickness
                                      && rt.TubeSize == item.TubeSize
                                      && rt.AngleRod == item.AngleRod
                                    );

                                if (model != null)
                                {
                                    if (model.Count > 0)
                                    {
                                        ModelState.AddModelError("", "Các thông tin danh mục nhóm 2 đã tồn tại !!!");
                                        return Json(list.ToDataSourceResult(request, ModelState));
                                    }
                                }

                                model = dbConn.Select<RodTwo>(rt => rt.Code == item.Code);

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

                                //var checkID = dbConn.SingleOrDefault<RodTwo>("SELECT Code, Id FROM dbo.RodTwo ORDER BY Id DESC");
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

                                RodTwo dbrodtwo = new RodTwo();
                                dbrodtwo.Style = item.Style;
                                dbrodtwo.TubeBottomDiameter = item.TubeBottomDiameter;
                                dbrodtwo.TubeTopDiameter = item.TubeTopDiameter;
                                dbrodtwo.TubeThickness = item.TubeThickness;
                                dbrodtwo.PrimaryBranchBottomDiameter = item.PrimaryBranchBottomDiameter;
                                dbrodtwo.PrimaryBranchTopDiameter = item.PrimaryBranchTopDiameter;
                                dbrodtwo.PrimaryBranchThickness = item.PrimaryBranchThickness;
                                dbrodtwo.ExtraBranchBottomDiameter = item.ExtraBranchBottomDiameter;
                                dbrodtwo.ExtraBranchTopDiameter = item.ExtraBranchTopDiameter;
                                dbrodtwo.ExtraBranchThickness = item.ExtraBranchThickness;
                                dbrodtwo.TubeSize = item.TubeSize;
                                dbrodtwo.AngleRod = item.AngleRod;
                                dbrodtwo.Code = item.Code;

                                var Id = int.Parse(dbrodtwo.Style);
                                dbrodtwo.StyleValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.TubeBottomDiameter);
                                dbrodtwo.TubeBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.TubeTopDiameter);
                                dbrodtwo.TubeTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.TubeThickness);
                                dbrodtwo.TubeThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.PrimaryBranchBottomDiameter);
                                dbrodtwo.PrimaryBranchBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.PrimaryBranchTopDiameter);
                                dbrodtwo.PrimaryBranchTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.PrimaryBranchThickness);
                                dbrodtwo.PrimaryBranchThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.ExtraBranchBottomDiameter);
                                dbrodtwo.ExtraBranchBottomDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.ExtraBranchTopDiameter);
                                dbrodtwo.ExtraBranchTopDiameterValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.ExtraBranchThickness);
                                dbrodtwo.ExtraBranchThicknessValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.TubeSize);
                                dbrodtwo.TubeSizeValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;

                                Id = int.Parse(dbrodtwo.AngleRod);
                                dbrodtwo.AngleRodValue = dbConn.Select<RodOne>(ro => ro.Id == Id).SingleOrDefault().Name;
                                dbConn.Insert<RodTwo>(dbrodtwo);
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
            var data = dbConn.Select<RodTwo>(whereCondition).ToList();
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
                    using (var dbTrans = dbConn.OpenTransaction(IsolationLevel.ReadCommitted))
                    {
                        foreach (var item in listdata)
                        {
                            dbConn.Delete<RodTwo>(s => s.Id == int.Parse(item));
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

            var RodHeaderHeaderList = dbConn.Select<RodHeader>(rh => rh.HaveCode == false && rh.HaveImage == false && rh.TypeId != "C");

            for (int i = 0; i < RodHeaderHeaderList.Count; i++)
            {
                var TypeId = RodHeaderHeaderList[i].TypeId;

                var List = dbConn.Query<Utilities_Parameters>("select Id as ID,Cast(Id as nvarchar(50)) as ParamID, Name as Value from RodOne where TypeId='" + TypeId + "' order by Id");

                DisplayName.Add(RodHeaderHeaderList[i].DisplayName);

                if (i == 0)
                {
                    ViewBag.Style = List;
                }
                if (i == 1)
                {
                    ViewBag.TubeBottomDiameter = List;
                }
                if (i == 2)
                {
                    ViewBag.TubeTopDiameter = List;
                }
                if (i == 3)
                {
                    ViewBag.TubeThickness = List;
                }
                if (i == 4)
                {
                    ViewBag.PrimaryBranchBottomDiameter = List;
                }
                if (i == 5)
                {
                    ViewBag.PrimaryBranchTopDiameter = List;
                }
                if (i == 6)
                {
                    ViewBag.PrimaryBranchThickness = List;
                }
                if (i == 7)
                {
                    ViewBag.ExtraBranchBottomDiameter = List;
                }
                if (i == 8)
                {
                    ViewBag.ExtraBranchTopDiameter = List;
                }
                if (i == 9)
                {
                    ViewBag.ExtraBranchThickness = List;
                }
                if (i == 10)
                {
                    ViewBag.TubeSize = List;
                }
                if (i == 11)
                {
                    ViewBag.AngleRod = List;
                }

            }
            ViewBag.DisplayName = DisplayName;
        }
    }
}