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
    public class RequestProductController : CustomController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /RequestProduct/
        public ActionResult Index()
        {
            if (userAsset.ContainsKey("View") && userAsset["View"])
            {
                IDbConnection dbConn = new OrmliteConnection().openConn();
                var dict = new Dictionary<string, object>();
                dict["asset"] = userAsset;
                dict["activestatus"] = new CommonLib().GetActiveStatus();
                ViewBag.listProductType = dbConn.Select<Utilities_Parameters>(p => p.Type == AllConstant.ProductTYPE);

                //columns.ForeignKey(p => p.TypeId, (System.Collections.IEnumerable)ViewBag.listTypeID, "Value", "Text").EditorTemplateName("GridForeignKey_Dropdownlist").Title("Loại xe").Width(110).HtmlAttributes(new { style = "background-color:rgb(198, 239, 206);" });
                ViewBag.listTypeId = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from LightHeadHeader union all
                                                                    select id,DisplayName as Text, TypeId as Value from FoundationBoltHeader union all
                                                                    select id,DisplayName as Text, TypeId as Value from RodHeader
                                                                 ");
                dbConn.Close();
                return View("_RequestProduct", dict);
            }
            else
                return RedirectToAction("NoAccess", "Error");
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var dbConn = new OrmliteConnection().openConn();
            string str = "SELECT * FROM LightHeadOne WHERE 1=0";
            var data = dbConn.Select<LightHeadOne>(str).ToList();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getTypeID(string listproducttype)
        {
            var dbConn = new OrmliteConnection().openConn();
            try
            {
                var data = new List<DropListDown>();
                if (listproducttype == AllConstant.ProductTYPE_TruDen)
                    data = dbConn.Select<DropListDown>("select id,DisplayName as Text, TypeId as Value from LightHeadHeader");
                else
                    if (listproducttype == AllConstant.ProductTYPE_BuLong)
                        data = dbConn.Select<DropListDown>("select id,DisplayName as Text, TypeId as Value from FoundationBoltHeader");
                    else
                        if (listproducttype == AllConstant.ProductTYPE_CanDen)
                            data = dbConn.Select<DropListDown>("select id,DisplayName as Text, TypeId as Value from RodHeader");
                        else
                            data = dbConn.Select<DropListDown>(@"select id,DisplayName as Text, TypeId as Value from LightHeadHeader union all
                                                                    select id,DisplayName as Text, TypeId as Value from FoundationBoltHeader union all
                                                                    select id,DisplayName as Text, TypeId as Value from RodHeader
                                                                 ");
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<RequestModel> list, string ProductType)
        {
            var db = new OrmliteConnection().openConn();
            try
            {
                //var ProductType = Request["ProductType"];
                var lst = new List<RequestModel>();
                TempData["RequestModel"] = lst;

                if (ProductType == AllConstant.ProductTYPE_CanDen)
                {
                    foreach (var item in list)
                    {

                        if (string.IsNullOrEmpty(item.TypeId))
                        {
                            ModelState.AddModelError("", "Vui lòng chọn loại");
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }

                        if (string.IsNullOrEmpty(item.Value))
                        {
                            ModelState.AddModelError("", "Vui lòng nhập thông tin");
                            return Json(list.ToDataSourceResult(request, ModelState));
                        }

                        var model = db.Select<RodOne>(ro => (ro.Code == item.Value || ro.Name == item.Value) && ro.TypeId == item.TypeId);

                        if (model != null)
                        {
                            if (model.Count() > 0)
                            {
                                ModelState.AddModelError("", "Giá trị " + item.Value + " đã tồn tại");
                                return Json(list.ToDataSourceResult(request, ModelState));
                            }
                        }

                        item.TypeValue = db.Select<RodHeader>(rh => rh.TypeId == item.TypeId).Single().DisplayName;
                        lst.Add(item);
                        TempData["RequestModel"] = lst;
                    }
                    SenaMailRod();
                }
                else
                    if (ProductType == AllConstant.ProductTYPE_TruDen)
                    {
                        foreach (var item in list)
                        {

                            if (string.IsNullOrEmpty(item.TypeId))
                            {
                                ModelState.AddModelError("", "Vui lòng chọn loại");
                                return Json(list.ToDataSourceResult(request, ModelState));
                            }

                            if (string.IsNullOrEmpty(item.Value))
                            {
                                ModelState.AddModelError("", "Vui lòng nhập thông tin");
                                return Json(list.ToDataSourceResult(request, ModelState));
                            }

                            var model = db.Select<LightHeadOne>(ro => (ro.Code == item.Value || ro.Name == item.Value) && ro.TypeId == item.TypeId);

                            if (model != null)
                            {
                                if (model.Count() > 0)
                                {
                                    ModelState.AddModelError("", "Giá trị " + item.Value + " đã tồn tại");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }
                            }

                            item.TypeValue = db.Select<LightHeadHeader>(rh => rh.TypeId == item.TypeId).Single().DisplayName;
                            lst.Add(item);
                            TempData["RequestModel"] = lst;
                        }
                        SenaMailLightHead();
                    }
                    else
                        if (ProductType == AllConstant.ProductTYPE_BuLong)
                        {
                            foreach (var item in list)
                            {

                                if (string.IsNullOrEmpty(item.TypeId))
                                {
                                    ModelState.AddModelError("", "Vui lòng chọn loại");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                if (string.IsNullOrEmpty(item.Value))
                                {
                                    ModelState.AddModelError("", "Vui lòng nhập thông tin");
                                    return Json(list.ToDataSourceResult(request, ModelState));
                                }

                                var model = db.Select<FoundationBoltOne>(ro => (ro.Code == item.Value || ro.Name == item.Value) && ro.TypeId == item.TypeId);

                                if (model != null)
                                {
                                    if (model.Count() > 0)
                                    {
                                        ModelState.AddModelError("", "Giá trị " + item.Value + " đã tồn tại");
                                        return Json(list.ToDataSourceResult(request, ModelState));
                                    }
                                }

                                item.TypeValue = db.Select<FoundationBoltHeader>(rh => rh.TypeId == item.TypeId).SingleOrDefault().DisplayName;
                                lst.Add(item);
                                TempData["RequestModel"] = lst;
                            }
                            SenaMailLightHead();
                        }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(list.ToDataSourceResult(request, ModelState));
            }
            return Json(new { sussess = true });
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<RequestModel> list)
        {
            var db = new OrmliteConnection().openConn();
            if (list != null)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("error", ex.Message);
                    return Json(list.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new { sussess = true });
        }

        public ActionResult SenaMailRod()
        {
            var lst = TempData["RequestModel"] as List<RequestModel> ?? new List<RequestModel>();

            //var lst = TempData["Rod_RequestModel"] as List<RequestModel> ?? new List<RequestModel>();
            string body = "";
            List<string> eMailTos = new List<string>();
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();
            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }
            dbConn.Close();
            string Subject = "Thông số kỹ thuật yêu cầu được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail_Request.txt"));
            string bodytable = "";
            foreach (var item in lst)
                bodytable += "<tr><td>" + item.TypeValue + "</td><td>" + item.Value + "</td></tr>";

            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "cần đèn");
            if (eMailTos.Count > 0)
                try
                {
                    Mail.SendMail(eMailTos, Subject, body);
                    lst = new List<RequestModel>();
                    TempData["RequestModel"] = lst;
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 1, Message = ex.Message });
                }
            return Json(new { sussess = true });
        }

        public ActionResult SenaMailFoundationBolt()
        {
            var lst = TempData["RequestModel"] as List<RequestModel> ?? new List<RequestModel>();
            string body = "";
            List<string> eMailTos = new List<string>();
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();
            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }
            dbConn.Close();
            string Subject = "Thông số kỹ thuật yêu cầu được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail_Request.txt"));
            string bodytable = "";
            foreach (var item in lst)
                bodytable += "<tr><td>" + item.TypeValue + "</td><td>" + item.Value + "</td></tr>";

            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "Bulong-móng");
            if (eMailTos.Count > 0)
                try
                {
                    Mail.SendMail(eMailTos, Subject, body);
                    lst = new List<RequestModel>();
                    TempData["RequestModel"] = lst;
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 1, Message = ex.Message });
                }
            return Json(new { sussess = true });
        }

        public ActionResult SenaMailLightHead()
        {

            var lst = TempData["RequestModel"] as List<RequestModel> ?? new List<RequestModel>();
            string body = "";
            List<string> eMailTos = new List<string>();
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var data = dbConn.Query<Auth_User>("[p_get_User_eMailTos]", commandType: System.Data.CommandType.StoredProcedure).ToList();
            foreach (var u in data)
            {
                eMailTos.Add(u.Email);
            }
            dbConn.Close();
            string Subject = "Thông số kỹ thuật yêu cầu được định nghĩa";
            body = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/TemplateMail_Request.txt"));
            string bodytable = "";
            foreach (var item in lst)
                bodytable += "<tr><td>" + item.TypeValue + "</td><td>" + item.Value + "</td></tr>";

            body = body.Replace("[@BodyTable]", bodytable);
            body = body.Replace("[@Material]", "Trụ đèn");
            if (eMailTos.Count > 0)
                try
                {
                    Mail.SendMail(eMailTos, Subject, body);
                    lst = new List<RequestModel>();
                    TempData["RequestModel"] = lst;
                }
                catch (Exception ex)
                {
                    return Json(new { Status = 1, Message = ex.Message });
                }
            return Json(new { sussess = true });
        }
    }
}