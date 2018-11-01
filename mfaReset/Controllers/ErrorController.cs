using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mfaReset.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            ViewBag.Message = "The Page you were looking for was not found";

            return View();
        }
        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "You are not authorized for this page";
            return View();
        }
    }
}