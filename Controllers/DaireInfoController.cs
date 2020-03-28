using AidatPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace AidatPro.Controllers
{
    [Authorize(Roles = "Daire")]
    [RoutePrefix("Daireniz")]
    public class DaireInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

        // GET: DaireInfo   
        [Route("istatistikler")]
        public ActionResult Index(string id)
        {
            TempData["Id"] = id;

            var us = db.Users.SingleOrDefault(x => x.Id == id);

            var user = db.DaireAccount.Where(x => x.Email == us.Email).FirstOrDefault();

            var model = db.Aidats.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).ToList();

            ViewBag.Daires = db.Daires.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).FirstOrDefault().DaireAd;

            ViewBag.Bloks = db.Bloks.Where(x => x.Id == user.BlokId).FirstOrDefault().BlokAd;

            ViewBag.Email = user.Email;

            return View(model);

        }

      
        public ActionResult DaireEtkinlik()
        {
            var id = TempData["Id"];

            var us = db.Users.SingleOrDefault(x => x.Id == id.ToString());

            var user = db.DaireAccount.Where(x => x.Email == us.Email).FirstOrDefault();

            var model = db.Etkinliks.Where(x => x.Acc == user.Acc).ToList();

            return PartialView("DaireEtkinlik", model);
        }

       
        public ActionResult DaireDemirbas()
        {
            var id = TempData["Id"];

            var us = db.Users.SingleOrDefault(x => x.Id == id.ToString());

            var user = db.DaireAccount.Where(x => x.Email == us.Email).FirstOrDefault();

            var teslimModel = db.DemirbasTeslim.Where(x => x.Acc == user.Acc).ToList();
            var demirSource = db.Demirbas.Where(x => x.Acc == user.Acc).ToList();

            var model = teslimModel.Where(x => x.DaireId == user.DaireId).Select(x => new DemirbasTeslimModel
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

            return PartialView("DaireDemirbas", model);
        }

     
        public ActionResult SorumluPers()
        {
            var id = TempData["Id"];

            var us = db.Users.SingleOrDefault(x => x.Id == id.ToString());

            var user = db.DaireAccount.Where(x => x.Email == us.Email).FirstOrDefault();

            var model = db.Personels.Where(x => x.Acc == user.Acc).Where(x => x.SorumluBlokId == user.BlokId).ToList();


            return PartialView("SorumluPers", model);
        }

  
        public ActionResult Iletisim()
        {
            var id = TempData["Id"];

            var us = db.Users.SingleOrDefault(x => x.Id == id.ToString());

            var user = db.DaireAccount.Where(x => x.Email == us.Email).FirstOrDefault();

            var yoneticiMail = db.Users.FirstOrDefault(x => x.Id == user.Acc).Email;

            ViewBag.Email = db.Daires.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).FirstOrDefault().EMail;

            ViewBag.Telefon = db.Daires.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).FirstOrDefault().Telefon;

            ViewBag.Sahip = db.Daires.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).FirstOrDefault().Sahip;

            ViewBag.Yonetim = db.Daires.Where(x => x.BlokId == user.BlokId && x.DaireId == user.DaireId).FirstOrDefault().Acc;


            return PartialView("Iletisim");
        }

        [HttpPost]  
        public ActionResult Iletisim(DaireMailModel daireMailModel,string Acc)
        {          

            var yoneticiMail = db.Users.SingleOrDefault(x => x.Id == Acc).Email;            

            var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.Subject = daireMailModel.Konu;
            mailMessage.Body = daireMailModel.Mesaj + " Gonderen İsim : " + daireMailModel.GonderenAd + " Gonderen Telefon : " + daireMailModel.Telefon;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.From = adrese;
            mailMessage.To.Add(new MailAddress(yoneticiMail));

            var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
            smtpClient.Send(mailMessage);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}