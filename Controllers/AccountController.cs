using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AidatPro.Models;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;
using System.Net;

namespace AidatPro.Controllers
{
    [Authorize(Roles = "Uye")]
    [RoutePrefix("Uyelik")]
    public class AccountController : Controller
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

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]    
        [Route("giris_yap")]
        public ActionResult Login()
        {

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("giris_yap")]
        public async Task<ActionResult> Login(LoginViewModel model, bool rembr = false)
        {            

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = UserManager.Find(model.Email, model.Password);
            if (user != null)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var us = db.Users.SingleOrDefault(x => x.Email == model.Email);
                    us.LastLoginTime = DateTime.Now;
                    db.Entry(us).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, rembr, shouldLockout: false);

                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    return RedirectToAction("ConfirmError", "Home");
                }

                var use = db.Users.SingleOrDefault(x => x.Email == model.Email);

                if (result==SignInStatus.Success)
                {
                    if (await UserManager.IsInRoleAsync(use.Id,"Uye"))
                    {
                        return RedirectToAction("Panel", "Panel");

                    }
                    if (await UserManager.IsInRoleAsync(use.Id, "Daire"))
                    {
                        return RedirectToAction("Index", "DaireInfo", new { id = use.Id });
                    }
                }             
              
              

                return RedirectToAction("Anasayfa", "Home");
            }
            else
            {
                TempData["UserMessage"] = "Kullanıcı Bulunamadı veya Parola Hatalı..";

                return View();
            }           



        }

    
        [AllowAnonymous]
        [Route("çıkış_yap")]
        public ActionResult LogOf()
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]   
        [Route("Üye_OL")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Üye_OL")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            string bodys = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/MailHtml/OnayMail.html")))
            {
                bodys = reader.ReadToEnd();
            }

            var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    if (!roleManager.RoleExists("Uye"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                        role.Name = "Uye";
                        roleManager.Create(role);
                    }
                    await UserManager.AddToRoleAsync(user.Id, "Uye");

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    bodys = bodys.Replace("{callbackUrl}", callbackUrl);
                    var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
                    var mailMessage = new System.Net.Mail.MailMessage();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Subject = "Üyelik Onayı";
                    mailMessage.Body = bodys;
                    mailMessage.Priority = MailPriority.Normal;
                    mailMessage.From = adrese;
                    mailMessage.To.Add(new MailAddress(user.Email));

                    var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
                    smtpClient.Send(mailMessage);

                    return RedirectToAction("RegisForm", "Home");
                }

                TempData["UserMessage"] = result;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

    
        [Route("Daireye_Hesap_Oluştur")]
        public ActionResult DaireAccount()
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);
            ViewBag.daireDropdown = db.Daires.Where(x => x.Acc == user).Select(x => x);

            return View();
        }


        [HttpPost]
        [Route("Daireye_Hesap_Oluştur")]
        public async Task<ActionResult> DaireAccount(DaireAccountModel model)
        {
            ViewBag.blokDropdown = db.Bloks.Where(x => x.Acc == user).Select(x => x);
            ViewBag.daireDropdown = db.Daires.Where(x => x.Acc == user).Select(x => x);

            var varmi = db.DaireAccount.Where(x => x.Acc == user && x.Email == model.Email).FirstOrDefault();
            if (varmi != null)
            {
                db.DaireAccount.Remove(varmi);
                db.SaveChanges();
            }

            var daire = db.Daires.Where(x => x.Acc == user).Where(x => x.Id == model.DaireId).FirstOrDefault().DaireId;
            model.DaireId = daire;
            db.DaireAccount.Add(model);
            db.SaveChanges();

            string bodys = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/MailHtml/OnayMail.html")))
            {
                bodys = reader.ReadToEnd();
            }

            var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Email);

                if (result.Succeeded)
                {
                    if (!roleManager.RoleExists("Daire"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                        role.Name = "Daire";
                        roleManager.Create(role);
                    }
                    var users = await UserManager.FindByNameAsync(model.Email);
                    await UserManager.AddToRoleAsync(users.Id, "Daire");

                    string code = await UserManager.GeneratePasswordResetTokenAsync(users.Id);
                    var callbackUrl = Url.Action("ConfirmDaire", "Account", new { userId = users.Id, code = code }, protocol: Request.Url.Scheme);
                    bodys = bodys.Replace("{callbackUrl}", callbackUrl);
                    var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
                    var mailMessage = new System.Net.Mail.MailMessage();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Subject = "Üyelik Onayı";
                    mailMessage.Body = bodys;
                    mailMessage.Priority = MailPriority.Normal;
                    mailMessage.From = adrese;
                    mailMessage.To.Add(new MailAddress(user.Email));

                    var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
                    smtpClient.Send(mailMessage);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    await UserManager.UpdateAsync(user);

                    if (!roleManager.RoleExists("Daire"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                        role.Name = "Daire";
                        roleManager.Create(role);
                    }
                    var users = await UserManager.FindByNameAsync(model.Email);
                    await UserManager.AddToRoleAsync(users.Id, "Daire");

                    string code = await UserManager.GeneratePasswordResetTokenAsync(users.Id);
                    var callbackUrl = Url.Action("ConfirmDaire", "Account", new { userId = users.Id, code = code }, protocol: Request.Url.Scheme);
                    bodys = bodys.Replace("{callbackUrl}", callbackUrl);
                    var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
                    var mailMessage = new System.Net.Mail.MailMessage();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Subject = "Üyelik Onayı";
                    mailMessage.Body = bodys;
                    mailMessage.Priority = MailPriority.Normal;
                    mailMessage.From = adrese;
                    mailMessage.To.Add(new MailAddress(user.Email));

                    var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
                    smtpClient.Send(mailMessage);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        [Route("doğrulama")]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]   
        [Route("daire_doğrulama")]
        public ActionResult ConfirmDaire(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var us = db.Users.SingleOrDefault(x => x.Id == userId);
            us.EmailConfirmed = true;
            db.Entry(us).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            Session["code"] = code;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("daire_doğrulama")]
        public ActionResult ConfirmDaire(string Email, string Password, string das)
        {

            var code = Session["code"];

            if (!ModelState.IsValid)
            {
                return View();
            }

            var us = db.Users.SingleOrDefault(x => x.Email == Email);

            if (us == null)
            {
                TempData["UserMessage"] = "Bu kullanıcı adı size ait değil";
                return View();
            }
            var result = UserManager.ResetPassword(us.Id, code.ToString(), Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "DaireInfo", new { id = us.Id });
            }

            TempData["UserMessage"] = "Bilinmeyen bir hata oluştu.";

            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        [Route("parolamı_unuttum")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("parolamı_unuttum")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    TempData["UserMessage"] = "asd";
                    return View();
                }

                string bodys = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/MailHtml/ParolaReset.html")))
                {
                    bodys = reader.ReadToEnd();
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                bodys = bodys.Replace("{callbackUrl}", callbackUrl);
                var adrese = new MailAddress("MailJet Kullanıcı Mailiniz", "Site Aidat Takip");
                var mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = "Parola Hatırlatma";
                mailMessage.Body = bodys;
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.From = adrese;
                mailMessage.To.Add(new MailAddress(user.Email));

                var smtpClient = new SmtpClient("in-v3.mailjet.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("Kendinize ait App Key", "Kendinize ait App Key");
                smtpClient.Send(mailMessage);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        [Route("parolamı_unuttum_bilgilendirme")]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [Route("parolamı_sıfırla")]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("parolamı_sıfırla")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                TempData["UserMessage"] = "Bu kullanıcı adı size ait değil";
                return View();
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            TempData["UserMessage"] = "Bilinmeyen bir hata oluştu.";

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        [Route("parolamı_sıfırla_bilgilendirme")]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}