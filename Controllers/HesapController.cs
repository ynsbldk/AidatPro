using AidatPro.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AidatPro.Controllers
{
    [Authorize(Roles = "Uye")]
    [RoutePrefix("Hesap")]
    public class HesapController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

            filterContext.Result = new ViewResult()
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary(model)
            };

        }

        private ApplicationDbContext db = new ApplicationDbContext();
        private string user = System.Web.HttpContext.Current.User.Identity.GetUserId();
        // GET: Hesap   
        [Route("hesap_oluştur")]
        public ActionResult CreateHesap()
        {
            ViewBag.KasaListe = db.Kasa.Where(x => x.Acc == user).Select(x => x);
            return View();
        }

        [HttpPost]
        [Route("hesap_oluştur")]
        public ActionResult CreateHesap(KasaBakiyeModel kasaBakiyeModel,string selector)
        {         
            if (selector=="true")
            {
                kasaBakiyeModel.GiderGelir = true;
            }
            else
            {
                kasaBakiyeModel.GiderGelir = false;
            }
            var value = kasaBakiyeModel.Bakiye;

            kasaBakiyeModel.Bakiye = Math.Abs(value.Value);

            db.KasaBakiye.Add(kasaBakiyeModel);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }     

        private void ShowMessage(string title, string body, string type)
        {
            ViewBag.Titles = title;
            ViewBag.Mesaj = body;
            ViewBag.Types = type;
        }
    }
}