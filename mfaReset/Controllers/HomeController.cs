using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using mfaReset.Models;
using mfaReset.Service;
using mfaReset.ViewModel;

namespace mfaReset.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IFunctionService _functionService;
        private IGraphService _graphService;
        private ILogService _logService;

        public HomeController(IFunctionService functionService, IGraphService graphService, ILogService logService)
        {
            _functionService = functionService;
            _graphService = graphService;
            _logService = logService;

        }
        public ActionResult Index()
        {
            return View("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            HomeViewModel model = new HomeViewModel();

            if (query != null)
            {
                try
                {
                    model.User = await _graphService.GetUser(query);
                    return PartialView("_SearchResult", model);
                }

                catch (Exception e)
                {
                    //DBlog error
                    Session["Error"] = "Error: Can't find user with UPN " + query ;
                    return PartialView("_StatusMessagePartial");
                }


            }
            return View(model);

        }
        [HttpPost]
        public async Task<ActionResult> Reset(string upn)
        {
            if (upn != null)
            {
                ClaimsPrincipal cp = ClaimsPrincipal.Current;
                bool result = await _functionService.ResetUserAsync(upn, cp.Identity.Name);
                if (result)
                {
                    _logService.LogRequest(cp.Identity.Name, upn, DateTime.Now);
                    Session["Success"] = "User has been reset";
                }
                else
                {
                    Session["Error"] = "Reset failed. Please check function logs";
                }
            }
            else
            {
                Session["Error"] = "No input received";

            }
            return RedirectToAction("Index");
        }

        public ActionResult Feedback()
        {
            FeedbackViewModel model = new FeedbackViewModel();
            return View("Feedback", model);
        }
        [HttpPost]
        public async Task<ActionResult> SendFeedback(FeedbackViewModel Feedback)
        {
            if (Feedback.FeedbackTextArea != null)
            {
                bool result = await _graphService.SendMail(Feedback.FeedbackTextArea);
                if (result)
                {
                    Session["Success"] = "Feedback has been sent";
                }
                else
                {
                    Session["Error"] = "Send Feedback failed. Please check function logs";
                }
            }
            else
            {
                Session["Error"] = "No input received";

            }
            return RedirectToAction("Index");
        }
    }
}