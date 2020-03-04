using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MusicShop.App_Start;
using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.Models;
using MusicShop.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private KoszykMenager koszykMenager;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ISessionMenager sessionMenager { get; set; }
        private AlbumContext db;

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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            logger.Info("Logowanie start");

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.Find(model.Email,model.Password);
            if (user != null)
            {             
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Potwierdź swoje konto");

                    ViewBag.errorMessage = "Musisz mieć potwierdzony adres e-mail, aby się zalogować.";
                    return View("Error");
                }
            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, 
                model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    logger.Info("Logowanie udane | " + model.Email);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("loginerror", "Nieudana próba logowania.");
                    logger.Info("Logowanie nieudane | " + model.Email);
                    return View(model);
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

        public ActionResult Register()
        {
            logger.Info("Rejestracja start");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, DaneUzytkownika = new DaneUzytkownika()};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   
                    logger.Info("Rejestracja udana");
                    await UserManager.AddToRoleAsync(user.Id, "Member");
                   // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Potwierdź swoje konto.");

                    ViewBag.Message = "sprawdź pocztę e-mail. Na podany adres został wysłany link aktywujący konto w serwisie muzycznym." +
                                      "Konto musi być potwierdzone, aby się zalogować.";

                    return View("Info");
                }
                logger.Info("Rejestracja nieudana");
                AddErrors(result);
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            db = new AlbumContext();
            sessionMenager = new SessionMenager();
            koszykMenager = new KoszykMenager(sessionMenager, db);
            var name = User.Identity.Name;
            logger.Info("Wylogowanie | " + name);
            koszykMenager.PustyKoszyk();
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null/* || !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
                {
                    // Nie ujawniaj informacji o tym, że użytkownik nie istnieje lub nie został potwierdzony
                    return View("ResetPasswordConfirmation");
                }

                // Aby uzyskać więcej informacji o sposobie włączania potwierdzania konta i resetowaniu hasła, odwiedź stronę https://go.microsoft.com/fwlink/?LinkID=320771
                // Wyślij wiadomość e-mail z tym łączem
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Resetuj hasło", "<h1>Drogi użytkowniku,</h1>" +
               "<h2>zresetuj swoje hasło klikając <a href=\"" + callbackUrl + "\">tutaj</a>.</h2>" +
               "<h2>Pozdrawiamy,</h2><h2>Zespół serwisu muzycznego</h2>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Dotarcie do tego miejsca wskazuje, że wystąpił błąd, wyświetl ponownie formularz
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Nie ujawniaj informacji o tym, że użytkownik nie istnieje
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
                "<h1>Drogi użytkowniku,</h1>"+
               "<h2>potwierdź swoje konto klikając <a href=\"" + callbackUrl + "\">tutaj</a>.</h2>" +
               "<h2>Pozdrawiamy,</h2><h2>Zespół serwisu muzycznego</h2>");

            return callbackUrl;
        }
    }
}