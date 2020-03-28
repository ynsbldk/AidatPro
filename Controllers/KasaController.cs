using AidatPro.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System;

namespace AidatPro.Controllers
{
    [Authorize(Roles = "Uye")]
    [RoutePrefix("Kasa")]
    public class KasaController : Controller
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
        KasaViewModel kasaViewModel = new KasaViewModel();

        // GET: Kasa
        [Route("Kasa_Listesi")]
        public ActionResult Index()
        {         
                        
            var model = db.Kasa.Where(x => x.Acc == user).ToList();
            
            var bakiyes = db.KasaBakiye.Where(x => x.Acc == user).ToList();

            var toplam = db.KasaBakiye.Where(x => x.Acc == user).Where(x => x.GiderGelir).Sum(x=>x.Bakiye);

            kasaViewModel.Kasa = model;            
            kasaViewModel.KasaBakiye = bakiyes;
            
            return View(kasaViewModel);

        }

        [Route("Kasa_detayı/{id}")]
        public ActionResult KasaDetay(int id)
        {
            var tarihs = DateTime.Now.Month;
            var model = db.Kasa.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == id);
            var bakiye = db.KasaBakiye.Where(x => x.Acc == user).Where(x => x.GiderGelir).Where(x => x.KasaId == model.Id).Where(x=>x.Tarih.Value.Month==tarihs).Sum(x => x.Bakiye);
            var giderBakiye = db.KasaBakiye.Where(x => x.Acc == user).Where(x => x.GiderGelir == false).Where(x => x.KasaId == model.Id).Where(x=>x.Tarih.Value.Month==tarihs).Sum(x => x.Bakiye);

            var pers = db.Personels.Where(x => x.Acc == user).Where(x => x.Id == model.SorumluPersonelId).FirstOrDefault();
            model.SorumluPersonel = pers;

            ViewBag.perskDropdown = db.Personels.Where(x => x.Acc == user).Select(x => x);

            ViewBag.Bakiye = bakiye.ToString();
            ViewBag.GiderBakiye = giderBakiye.ToString();


            return View(model);
        }
  
        public ActionResult KasaIslem(int id)
        {
            var kmodel = db.KasaBakiye.Where(x => x.Acc == user).Where(x => x.KasaId == id).ToList();

            return PartialView("KasaIslem", kmodel);
        }

        [Route("yeni_kasa_oluştur")]
        public ActionResult CreateKasa()
        {
            ViewBag.SPersonel = db.Personels.Where(x => x.Acc == user).Select(x => x);
            return View();
        }

        [HttpPost]
        [Route("yeni_kasa_oluştur")]
        public ActionResult CreateKasa(KasaModel kasaModel)
        {
            ViewBag.SPersonel = db.Personels.Where(x => x.Acc == user).Select(x => x);

            db.Kasa.Add(kasaModel);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Route("kasayı_sil")]
        public ActionResult DeleteKasa(int kasaId)
        {
            var kasa = db.Kasa.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == kasaId);
            var kasaBakiye = db.KasaBakiye.Where(x => x.Acc == user).Where(x => x.KasaId == kasaId);

            foreach (var item in kasaBakiye)
            {
                db.KasaBakiye.Remove(item);
            }

            db.Kasa.Remove(kasa);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Route("bakiyeyi_sil")]
        public ActionResult DeleteBakiye(int bakiyeId)
        {
            var bakiye = db.KasaBakiye.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == bakiyeId);

            db.KasaBakiye.Remove(bakiye);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Route("personel_değiştir")]
        public ActionResult PersonelDegistir()
        {
            var model = db.Kasa.Where(x => x.Acc == user).ToList();
            return PartialView("PersonelDegistir",model);
        }

        [HttpPost]
        [Route("personel_değiştir")]
        public JsonResult PersonelDegistir(int id,int kasaid)
        {            
            var model = db.Kasa.Where(x=>x.Acc==user).Where(x => x.Id ==kasaid).FirstOrDefault();
            model.SorumluPersonelId =id;
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