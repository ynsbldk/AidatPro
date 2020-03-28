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
    [RoutePrefix("Daire")]
    public class DaireController : Controller
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
        // GET: Daire
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Daires()
        {

            return View();
        }

        [Route("tekil_daire")]
        public ActionResult SingleDaire(int id)
        {
            var daire = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();

            return View(daire);
        }

        [HttpPost]
        [Route("tekil_daire_broç_ekle")]
        public ActionResult TekilBorcEkle(int id, int borc, int blokId)
        {
            var borcs = Convert.ToDecimal(Math.Abs(borc));

            var gDaire = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault().DaireId;
            var model = new AidatModel();
            model.Borc = borcs;
            model.BlokId = blokId;
            model.DaireId = gDaire;
            model.Odeme = false;
            model.EklenmeTarih = DateTime.Now;

            if (model != null)
            {
                db.Aidats.Add(model);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("tekil_daire_duzenle")]
        public ActionResult UpdateDaire(int id)
        {
            var model = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            return PartialView("UpdateDaire", model);
        }

        [HttpPost]
        [Route("tekil_daire_duzenle")]
        public ActionResult UpdateDaire(DaireModel model)
        {
            var dmodel = db.Daires.Where(x => x.Id == model.Id).FirstOrDefault();
            dmodel.DaireAd = model.DaireAd;
            dmodel.DaireNo = model.DaireNo;
            dmodel.EMail = model.EMail;
            dmodel.EvSahibi = model.EvSahibi;
            dmodel.HaneUye = model.HaneUye;
            dmodel.Notlar = model.Notlar;
            dmodel.Telefon = model.Telefon;
            dmodel.Sahip = model.Sahip;
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("tekil_daireye_ait_demirbaş")]
        public ActionResult DaireAitDemirbas(int daireId)
        {
            var teslimModel = db.DemirbasTeslim.Where(x => x.Acc == user).ToList();
            var demirSource = db.Demirbas.Where(x => x.Acc == user).ToList();

            var model = teslimModel.Where(x => x.DaireId == daireId).Select(x => new DemirbasTeslimModel
            {
                Id = x.Id,
                Demirs = demirSource.Where(y => y.Id == x.DemirbasId).FirstOrDefault(),
                BlokId = x.BlokId,
                DaireId = x.DaireId,
                DemirbasId = x.DemirbasId,
                EklenmeTarih = x.EklenmeTarih,
                TeslimAlan = x.TeslimAlan,
                TeslimEden = x.TeslimEden,
                Acc = x.Acc
            }).ToList();


            return PartialView("DaireAitDemirbas", model);
        }

        [Route("tekil_daire_demirbas_temizle")]
        public ActionResult DaireDemirbasTemizle(int gelenid)
        {

            var gelen = db.DemirbasTeslim.Find(gelenid);
            gelen.Demirs = null;
            gelen.Bloks = null;
            gelen.Daires = null;

            db.DemirbasTeslim.Remove(gelen);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [Route("tekil_daire_borçlar")]
        public ActionResult DaireBorc(int? page, int id)
        {
            var kasaview = new KasaViewModel();

            int pageNumber = (page ?? 1);

            var daires = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();
            var bloks = db.Bloks.Where(x => x.Acc == user).Where(x => x.Id == daires.BlokId).FirstOrDefault().Id;


            var model = db.Aidats.Where(x => x.Acc == user).Where(x => x.DaireId == daires.DaireId).Where(x => x.BlokId == bloks).ToList().ToPagedList(pageNumber, 5);

            ViewBag.KasaListe = db.Kasa.Where(x => x.Acc == user).Select(x => x);

            ViewBag.GunlukOdenen = db.Aidats.Where(x => x.Acc == user).Where(x => x.DaireId == daires.DaireId).Where(x => x.BlokId == bloks).Where(x => x.Odeme).Where(x => x.TahsilatTarih.Value.Day == DateTime.Now.Day).Sum(x => x.Borc);
            ViewBag.GunlukBorc = db.Aidats.Where(x => x.Acc == user).Where(x => x.DaireId == daires.DaireId).Where(x => x.BlokId == bloks).Where(x => x.Odeme == false).Where(x => x.EklenmeTarih.Day == DateTime.Now.Day).Sum(x => x.Borc);

            ViewBag.AylikOdenen = db.Aidats.Where(x => x.Acc == user).Where(x => x.DaireId == daires.DaireId).Where(x => x.BlokId == bloks).Where(x => x.Odeme).Where(x => x.TahsilatTarih.Value.Month == DateTime.Now.Month).Sum(x => x.Borc);
            ViewBag.AylikBorc = db.Aidats.Where(x => x.Acc == user).Where(x => x.DaireId == daires.DaireId).Where(x => x.BlokId == bloks).Where(x => x.Odeme == false).Where(x => x.EklenmeTarih.Month == DateTime.Now.Month).Sum(x => x.Borc);

            kasaview.AidatModel = model;



            return PartialView("DaireBorc", kasaview);

        }


        [HttpPost]
        [Route("tekil_daire_borçlar")]
        public ActionResult DaireBorc(int id, string tahsil)
        {
            try
            {
                var gid = Convert.ToInt32(tahsil);
                var model = db.Aidats.Where(x => x.Acc == user).Where(x => x.Id == gid).FirstOrDefault();
                model.KasaId = id;
                model.Odeme = true;
                model.TahsilatTarih = DateTime.Now;
                db.SaveChanges();

                var kasabak = new KasaBakiyeModel();
                kasabak.Bakiye = model.Borc;
                kasabak.GiderGelir = true;
                kasabak.KasaId = id;
                kasabak.Tarih = DateTime.Now;
                db.KasaBakiye.Add(kasabak);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }

        }

        [Route("tekil_daire_hesap_listesi")]
        public ActionResult HesapListe()
        {
            var model = db.DaireAccount.Where(x => x.Acc == user).Select(x => new HesapViewModel
            {
                Blok = db.Bloks.Where(y => y.Acc == user).Where(y => y.Id == x.BlokId).FirstOrDefault(),
                Daire = db.Daires.Where(y => y.Acc == user).Where(a => a.DaireId == x.DaireId && a.BlokId == x.BlokId).FirstOrDefault(),
                DaireAcc = db.DaireAccount.Where(y => y.Acc == user).Where(b => b.BlokId == x.BlokId && b.DaireId == x.DaireId).FirstOrDefault(),
                Use = db.Users.FirstOrDefault(y => y.Email == x.Email)
            }).ToList();


            return View(model);

        }

        [Route("tekil_daire_hesap_sil")]
        public ActionResult DeleteHesap(int id)
        {
            var model = db.DaireAccount.Where(x => x.Acc == user).Where(x => x.Id == id).FirstOrDefault();

            var Use = db.Users.FirstOrDefault(y => y.Email == model.Email);

            Use.EmailConfirmed = false;

            db.DaireAccount.Remove(model);

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