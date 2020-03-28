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
    [RoutePrefix("Blok")]
    public class BlokController : Controller
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
        private DaireModel daire = new DaireModel();
        private AidatModel aidat = new AidatModel();
        private string user = System.Web.HttpContext.Current.User.Identity.GetUserId();

        // GET: Blok
        [HttpGet]
        [Route("Blok_Listesi")]
        public ActionResult Index()
        {
            var model = db.Bloks.Where(x => x.Acc == user).ToList();

            return View(model);
        }

     
        [Route("Yeni_Blok_Oluştur")]
        public ActionResult CreateBlok()
        {
            return View();
        }

        [HttpPost]
        [Route("Yeni_Blok_Oluştur")]
        public ActionResult CreateBlok(BlokModel blokModel)
        {

            if (blokModel.DaireSayi < 81 && blokModel.DaireSayi > 0)
            {
                db.Bloks.Add(blokModel);
                db.SaveChanges();

                for (int i = 1; i <= blokModel.DaireSayi; i++)
                {
                    daire.DaireId = i;
                    daire.BlokId = blokModel.Id;
                    daire.DaireNo = i.ToString();
                    daire.Sahip = "Belirsiz";
                    daire.EvSahibi = "Ev Sahibi";
                    daire.EMail = "Belirsiz";
                    daire.Telefon = "Belirsiz";
                    daire.DaireAd = blokModel.BlokAd + " " + i.ToString();
                    db.Daires.Add(daire);
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }



        }

        [Route("bloğunuza_ait_daire")]
        public ActionResult BlokAitDaire(int id)
        {
            var daire = db.Daires.Where(x => x.BlokId == id).Where(y => y.Acc == user).ToList();
            return View(daire);

        }

        [HttpPost]
        [Route("bloğunuza_ait_daire")]
        public ActionResult BlokAitDaire(string Borc, string blokId, IEnumerable<string> grade)
        {
            var deger = Convert.ToDecimal(Borc);

            foreach (var item in grade)
            {
                if (item != null)
                {
                    aidat.Borc = Math.Abs(deger);
                    aidat.DaireId = Convert.ToInt32(item);
                    aidat.BlokId = Convert.ToInt32(blokId);
                    db.Aidats.Add(aidat);
                    db.SaveChanges();
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("bloğu_sil")]
        public ActionResult DeleteBlok(int blokId)
        {
            var count = db.Aidats.Where(x => x.Acc == user).Where(x => x.BlokId == blokId).Count();

            var gelenBlok = db.Bloks.Find(blokId);

            var gelenDaire = db.Daires.Where(x => x.BlokId == blokId);

            if (count <= 0)
            {
                foreach (var item in gelenDaire)
                {
                    db.Daires.Remove(item);
                }

                db.Bloks.Remove(gelenBlok);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        [Route("bloğunuza_ait_sorumlular")]
        public ActionResult Sorumlu(int id)
        {
            var model = db.Personels.Where(x => x.Acc == user).Where(x => x.SorumluBlokId == id).ToList();
            return View(model);
        }

        [Route("bloğunuza_ait_sorumlu_iptal")]
        public ActionResult SorumluIptal(int id)
        {
            var model = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            model.SorumluBlokId = 0;
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