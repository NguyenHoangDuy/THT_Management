using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace THT.Controllers
{    
    public class ErrorController : Controller
    {        
        public ActionResult NoAccess()
        {
            return View("NoAccess");
        }

        public ActionResult NotFound()
        {
            return View("NotFound");
        }

        public ActionResult ErrorPage()
        {
            return View("ErrorPage");
        }

        public ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/ExcelImport"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
	}
}