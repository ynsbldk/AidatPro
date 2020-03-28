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
    [RoutePrefix("Personel")]
    public class PersonelController : Controller
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
        // GET: Personel
        [Route("Personel_Listesi")]
        public ActionResult Index()
        {
            var model = db.Personels.Where(x => x.Acc == user).ToList();
            return View(model);
        }

        [Route("Yeni_Personel")]
        [HttpGet]
        public ActionResult CreatePersonel()
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);

            return View();
        }

        
        [Route("Yeni_Personel")]
        [HttpPost]
        public ActionResult CreatePersonel(PersonelModel personelModel)
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);

            db.Personels.Add(personelModel);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("Personel_detay")]
        public ActionResult SinglePersonel(int id)
        {
            var personel = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            ViewBag.Blok = db.Bloks.Where(x => x.Acc == user).Where(x => x.Id == personel.SorumluBlokId).Select(x => x.BlokAd).FirstOrDefault();
            return View(personel);
        }

        [HttpGet]
        public ActionResult UpdatePersonel(int id)
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);

            var model = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            return PartialView("UpdatePersonel", model);
        }

        [HttpPost]    
        public ActionResult UpdatePersonel(PersonelModel personelModel)
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);

            var model = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == personelModel.Id).FirstOrDefault();            
            model.PersonelAd = personelModel.PersonelAd;
            model.PersonelGorev = personelModel.PersonelGorev;
            model.PersonelMail = personelModel.PersonelMail;
            model.SorumluBlokId = personelModel.SorumluBlokId;        
            model.Telefon = personelModel.Telefon;
            model.IsBaslangicTarih = personelModel.IsBaslangicTarih;

            int a = db.SaveChanges();
            if (a > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        [Route("Personel_Sil")]
        public ActionResult DeletePersonel(int id)
        {
            var model = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            var persKasa = db.Kasa.Where(x => x.SorumluPersonelId == id).Count();
            if (persKasa > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            else
            {
                db.Personels.Remove(model);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        private void ShowMessage(string title, string body, string type)
        {
            ViewBag.Titles = title;
            ViewBag.Mesaj = body;
            ViewBag.Types = type;
        }


    }
}