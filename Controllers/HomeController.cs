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
    [RoutePrefix("Anasayfa")]
    public class HomeController : Controller
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
    
        [Route("Aidat-Takip")]
        public ActionResult Anasayfa()
        {
            return View();
        }

        [Route("mail_bilgilendirme")]
        public ActionResult MailConfirm()
        {
            return View();
        }

        [Route("parola_resetle")]
        public ActionResult ResetPass()
        {
            return View();
        }

        [Route("kayıt_form_bilgilendirme")]
        public ActionResult RegisForm()
        {
            return View();
        }

        [Route("hata_bilgilendirme")]
        public ActionResult ConfirmError()
        {
            return View();
        }

        [Route("iletişim")]
        public ActionResult Iletisim()
        {
            return PartialView("Iletisim");
        }

        [HttpPost]
        [Route("iletişim")]
        public ActionResult Iletisim(DaireMailModel daireMailModel)
        {

            var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.Subject = daireMailModel.Konu;
            mailMessage.Body = daireMailModel.Mesaj + " Gonderen İsim : " + daireMailModel.GonderenAd + " Gonderen Telefon : " + daireMailModel.Telefon;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.From = adrese;
            mailMessage.To.Add(new MailAddress("siteaidattakip@hotmail.com"));

            var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
            smtpClient.Send(mailMessage);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Route("Dokumasyon")]
        public ActionResult Dokumasyon()
        {
            return View();
        }
    }
}