using AidatPro.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AidatPro.Controllers
{
    [Authorize(Roles = "Uye")]
    [RoutePrefix("Panel")]
    public class PanelController : Controller
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
        // GET: Panel     

        [Route("site_istatistikleri")]
        public ActionResult Panel()
        {
            ViewBag.ToplamBlok = db.Bloks.Where(x => x.Acc == user).Count();
            ViewBag.ToplamDaire = db.Daires.Where(x => x.Acc == user).Count();
            ViewBag.ToplamPersonel = db.Personels.Where(x => x.Acc == user).Count();
            ViewBag.ToplamDemirbas = db.Demirbas.Where(x => x.Acc == user).Count();

            return View();
        }

   
        public ActionResult KasaPartial()
        {
            var gModel = db.Kasa.Where(x => x.Acc == user).ToList();

            var bakiyes = db.KasaBakiye.Where(x => x.Acc == user).ToList();

            var gg = new KasaViewModel();
            gg.Kasa = gModel;
            gg.KasaBakiye = bakiyes;

            return PartialView("KasaPartial", gg);

        }

     
        public ActionResult AidatPartial()
        {
            var model = db.Aidats.Where(x => x.Acc == user).ToList();
            var bloklar = db.Bloks.Where(x => x.Acc == user).ToList();

            var dd = new PanelAidatModel();
            dd.Aidats = model;
            dd.Bloks = bloklar;

            return PartialView("AidatPartial", dd);
        }

   
        public ActionResult ChartPartial()
        {
            var gModel = db.Kasa.Where(x => x.Acc == user).ToList();
            var bakiyes = db.KasaBakiye.Where(x => x.Acc == user).ToList();

            var gg = new KasaViewModel();
            gg.Kasa = gModel;
            gg.KasaBakiye = bakiyes;

            return PartialView("ChartPartial", gg);
        }

    
        public ActionResult ChartGiderPartial()
        {
            var gModel = db.Kasa.Where(x => x.Acc == user).ToList();
            var bakiyes = db.KasaBakiye.Where(x => x.Acc == user).ToList();

            var gg = new KasaViewModel();
            gg.Kasa = gModel;
            gg.KasaBakiye = bakiyes;

            return PartialView("ChartGiderPartial", gg);
        }

      
        public JsonResult AjaxMethod()
        {
            var model = db.Kasa.Where(x => x.Acc == user).ToList();

            var query =
                 (from post in db.KasaBakiye
                  join meta in db.Kasa on post.KasaId equals meta.Id
                  where post.Acc == user
                  select new { Bakiye = post, Kasa = meta }).ToList();


            List<object> chartData = new List<object>();
            chartData.Add(new object[]
                            {
                            "Kasa", "Bakiye"
                            });


            foreach (var item in model)
            {
                chartData.Add(new object[]
                        {
                            item.KasaAd,query.Where(x=>x.Bakiye.GiderGelir).Where(x=>x.Bakiye.KasaId==item.Id).Sum(x=>x.Bakiye.Bakiye)
                        });
            }

            List<object> bos = new List<object>();

            if (chartData == null)
            {
                return Json(bos);
            }
            else
            {
                return Json(chartData);
            }

        }

        [HttpPost]
   
        public JsonResult AjaxsMethod()
        {
            var model = db.Kasa.Where(x => x.Acc == user).ToList();

            var query =
                 (from post in db.KasaBakiye
                  join meta in db.Kasa on post.KasaId equals meta.Id
                  where post.Acc == user
                  select new { Bakiye = post, Kasa = meta }).ToList();


            List<object> chartData = new List<object>();
            chartData.Add(new object[]
                            {
                            "Kasa", "Bakiye"
                            });


            foreach (var item in model)
            {
                chartData.Add(new object[]
                        {
                            item.KasaAd,query.Where(x=>!x.Bakiye.GiderGelir).Where(x=>x.Bakiye.KasaId==item.Id).Sum(x=>x.Bakiye.Bakiye)
                        });
            }

            List<object> bos = new List<object>();

            if (chartData == null)
            {
                return Json(bos);
            }
            else
            {
                return Json(chartData);
            }
        }
    }
}