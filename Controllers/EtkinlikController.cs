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
    [RoutePrefix("Etkinlikler")]
    public class EtkinlikController : Controller
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
        // GET: Etkinlik
        [Route("Etkinlik_Listesi")]
        public ActionResult Index()
        {
            var model = db.Etkinliks.Where(x => x.Acc == user).ToList();
            return View(model);
        }
        [Route("Etkinlik_Oluştur")]
        public ActionResult CreateEtkinlik()
        {
            return View();
        }

        [HttpPost]
        [Route("Etkinlik_Oluştur")]
        public ActionResult CreateEtkinlik(EtkinlikModel etkinlikModel)
        {
            db.Etkinliks.Add(etkinlikModel);            

            var deger = db.SaveChanges();
            if (deger > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("Etkinlik_Sil")]
        public ActionResult DeleteEtkinlik(int id)
        {
            var model = db.Etkinliks.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            db.Etkinliks.Remove(model);
            var deger = db.SaveChanges();
            if (deger > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}