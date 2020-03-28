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
    [RoutePrefix("Demirbaşlar")]
    public class DemirbasController : Controller
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
        // GET: Demirbas
        [Route("demirbaş_listesi")]
        public ActionResult Index()
        {
            var model = db.Demirbas.Where(x => x.Acc == user).ToList();
            return View(model);
        }

        [Route("demirbaş_oluştur")]
        public ActionResult CreateDemirbas()
        {
            return View();
        }

        [HttpPost]
        [Route("demirbaş_oluştur")]
        public ActionResult CreateDemirbas(DemirbasModel demirbasModel)
        {

            demirbasModel.TeslimEdildiMi = false;
            db.Demirbas.Add(demirbasModel);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("demirbaş_detay")]
        public ActionResult DemirbasDetay(int id)
        {
            var model = db.Demirbas.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        [Route("demirbaş_güncelle")]
        public ActionResult DemirbasUpdate(int id)
        {
            var model = db.Demirbas.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == id);
            return View(model);
        }

        [HttpPost]
        [Route("demirbaş_güncelle")]
        public ActionResult DemirbasUpdate(DemirbasModel demirbasModel)
        {
            var model = db.Demirbas.Where(x => x.Acc == user).FirstOrDefault(x => x.Id == demirbasModel.Id);
            model.Aciklama = demirbasModel.Aciklama;
            model.Adet = demirbasModel.Adet;
            model.AlisTarih = demirbasModel.AlisTarih;
            model.DemirbasAd = demirbasModel.DemirbasAd;
            model.Fiyat = demirbasModel.Fiyat;
            model.Kg = demirbasModel.Kg;
            model.TeslimEdildiMi = false;
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("demirbaş_sil")]
        public ActionResult DeleteDemirbas(int id)
        {
            var model = db.Demirbas.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            var dteslim = db.DemirbasTeslim.Where(x => x.Acc == user).Where(x => x.DemirbasId == id).Count();

            if (model.TeslimEdildiMi || dteslim > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            else
            {
                db.Demirbas.Remove(model);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("demirbaş_teslim")]
        public ActionResult DemirbasTeslim(int id)
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);
            ViewBag.daireDropdown = db.Daires.Where(x => x.Acc == user).Select(x => x);

            var demirSource = db.Demirbas.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();

            var blokSource = db.Bloks.Where(x => x.Acc == user).ToList();

            var daireSource = db.Daires.Where(x => x.Acc == user).ToList();

            var model = new DemirbasTeslimModel();
            model.Bloks = blokSource;
            model.Daires = daireSource;
            model.Demirs = demirSource;
            model.DemirbasId = demirSource.Id;
            

            return View(model);
        }

        [HttpPost]
        [Route("demirbaş_teslim")]
        public JsonResult DemirbasTeslim(DemirbasTeslimModel demirbasTeslimModel)
        {
            var demirbas = db.Demirbas.Where(x => x.Id == demirbasTeslimModel.DemirbasId).FirstOrDefault();
            var daire = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == demirbasTeslimModel.DaireId).FirstOrDefault().DaireId;
            

            if (demirbas.TeslimEdildiMi)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            else
            {                
                demirbas.TeslimEdildiMi = true;
                demirbasTeslimModel.DaireId = daire;
                db.DemirbasTeslim.Add(demirbasTeslimModel);
                db.SaveChanges();                

                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }


        [Route("teslim_edilen_demirbaşlar")]
        public ActionResult TeslimEdilenDemir()
        {

            var gModel = db.DemirbasTeslim.Where(x => x.Acc == user).Select(x => new TEDemirModel
            {
                Bloks = db.Bloks.Where(y => y.Acc == user).Where(y => y.Id == x.BlokId).FirstOrDefault(),
                Daires = db.Daires.Where(y => y.Acc == user).Where(a => a.DaireId == x.DaireId && a.BlokId == x.BlokId).FirstOrDefault(),
                Demirs = db.Demirbas.Where(y => y.Acc == user).Where(c => c.Id == x.DemirbasId).FirstOrDefault(),
                TeslimAlan = x.TeslimAlan,
                TeslimEden = x.TeslimEden,
                Id = x.Id,
                EklenmeTarihi = x.EklenmeTarih,
                Kullanici = x.Acc,
                DemirbasId = x.DemirbasId,
                DaireId = x.DaireId,
                BlokId = x.BlokId

            }).ToList();

            return View(gModel);
        }

        [HttpPost]
        [Route("demirbaş_teslimatları")]
        public JsonResult DaireList(int id)
        {
            var daireModel = db.Daires.Where(x => x.Acc == user).Where(x => x.BlokId == id).OrderBy(x => x.DaireNo).ToList();

            List<SelectListItem> itemList = daireModel.Select(x => new SelectListItem
            {
                Text = x.DaireAd,
                Value = x.Id.ToString()
            }).ToList();

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }

        [Route("demirbaş_teslimatı_iptal_et")]
        public ActionResult DeleteTeslim(int teslimId)
        {

            var demirbas = db.Demirbas.Where(x => x.Id == teslimId).FirstOrDefault();

            if (demirbas.TeslimEdildiMi)
            {
                demirbas.TeslimEdildiMi = false;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }

        [Route("demirbaş_teslimatı_sil")]
        public ActionResult DeleteTeslimat(int id)
        {
            var model = db.DemirbasTeslim.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            db.DemirbasTeslim.Remove(model);
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