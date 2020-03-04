using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AlbumContext db = new AlbumContext();
        private IMailService mailService;

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        public ManageController(/*AlbumContext context,*/ IMailService mailService)
        {
            //this.db = context;
            this.mailService = mailService;
        }

        //public ManageController(ApplicationUserManager userManager)
        //{
        //    UserManager = userManager;
        //}

        private ApplicationUserManager _userManager;
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Manage
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            var name = User.Identity.Name;
            logger.Info("Admin główna | " + name);

            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (User.IsInRole("Admin"))
                ViewBag.UserIsAdmin = true;
            else
                ViewBag.UserIsAdmin = false;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }

            var model = new ManageCredentialsViewModel
            {
                Message = message,
                DaneUzytkownika = user.DaneUzytkownika
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile([Bind(Prefix = "DaneUzytkownika")]DaneUzytkownika daneUzytkownika)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.DaneUzytkownika = daneUzytkownika;
                var result = await UserManager.UpdateAsync(user);

                AddErrors(result);
            }

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);

            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var message = ManageMessageId.ChangePasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("password-error", error);
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        public ActionResult ListaZamowien()
        {
            var name = User.Identity.Name;
            logger.Info("Admin zamowienia | " + name);

            bool isAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = isAdmin;

            IEnumerable<Zamowienie> zamowieniaUzytkownika;

            // Dla administratora zwracamy wszystkie zamowienia
            if (isAdmin)
            {
                zamowieniaUzytkownika = db.Zamowienia.Include("PozycjeZamowienia").OrderByDescending(o => o.DataZamowienia).ToArray();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                zamowieniaUzytkownika = db.Zamowienia.Where(o => o.UserID == userId).Include("PozycjeZamowienia").OrderByDescending(o => o.DataZamowienia).ToArray();
            }

            return View(zamowieniaUzytkownika);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public StanZamowienia ZmianaStanuZamowienia(Zamowienie zamowienie)
        {
            Zamowienie zamowienieDoModyfikacji = db.Zamowienia.Find(zamowienie.ZamowienieID);
            zamowienieDoModyfikacji.StanZamowienia = zamowienie.StanZamowienia;
            db.SaveChanges();

            if (zamowienieDoModyfikacji.StanZamowienia == StanZamowienia.Zrealizowane)
            {
                this.mailService.WyslanieZamowienieZrealizowaneEmail(zamowienieDoModyfikacji);
            }

            return zamowienie.StanZamowienia;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DodajAlbum(int? albumId, bool? potwierdzenie)
        {
            Album album;
            if (albumId.HasValue)
            {
                //tryb edycji albumu
                ViewBag.EditMode = true;
                album = db.Albumy.Find(albumId);
            }
            else
            {
                //tryb dodania albumu
                ViewBag.EditMode = false;
                album = new Album();
            }

            var result = new EditAlbumViewModel();
            result.Kategorie = db.Kategorie.ToList();
            result.Album = album;
            result.Potwierdzenie = potwierdzenie;

            return View(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DodajAlbum(EditAlbumViewModel model, HttpPostedFileBase file)
        {
            if (model.Album.AlbumID > 0)
            {               
                db.Entry(model.Album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DodajAlbum", new { potwierdzenie = true });
            }
            else
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        var fileExt = Path.GetExtension(file.FileName);
                        var filename = Guid.NewGuid() + fileExt;
                        var path = Path.Combine(Server.MapPath(AppConfig.IkonyKategoriiFolderWzgledny), filename);
                        file.SaveAs(path);
                        model.Album.Okladkalbumu = filename;
                        model.Album.DataDodania = DateTime.Now;
                        db.Entry(model.Album).State = EntityState.Added;
                        db.SaveChanges();
                        return RedirectToAction("DodajAlbum", new { potwierdzenie = true });
                    }
                    else
                    {
                        var kategorie = db.Kategorie.ToList();
                        model.Kategorie = kategorie;
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nie wskazano pliku");
                    var kategorie = db.Kategorie.ToList();
                    model.Kategorie = kategorie;
                    return View(model);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UkryjAlbum(int albumId)
        {
            var album = db.Albumy.Find(albumId);
            album.Ukryty = true;
            db.SaveChanges();

            return RedirectToAction("DodajAlbum", new { potwierdzenie = true });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult PokazAlbum(int albumId)
        {
            var album = db.Albumy.Find(albumId);
            album.Ukryty = false;
            db.SaveChanges();

            return RedirectToAction("DodajAlbum", new { potwierdzenie = true });
        }

        [AllowAnonymous]
        public ActionResult WyslaniePotwierdzenieZamowieniaEmail(int zamowienieId, string nazwisko)
        {
            var zamowienie = db.Zamowienia.Include("PozycjeZamowienia").Include("PozycjeZamowienia.Album")
                               .SingleOrDefault(o => o.ZamowienieID == zamowienieId && o.Nazwisko == nazwisko);

            if (zamowienie == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            PotwierdzenieZamowieniaEmail email = new PotwierdzenieZamowieniaEmail();
            email.To = zamowienie.Email;
            email.From = "adrian94wilk@gmail.com";
            email.Wartosc = zamowienie.CenaZamowienia;
            email.NumerZamowienia = zamowienie.ZamowienieID;
            email.PozycjeZamowienia = zamowienie.PozycjeZamowienia;
            email.sciezkaObrazka = AppConfig.IkonyKategoriiFolderWzgledny;
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public ActionResult WyslanieZamowienieZrealizowaneEmail(int zamowienieId, string nazwisko)
        {
            var zamowienie = db.Zamowienia.Include("PozycjeZamowienia").Include("PozycjeZamowienia.Album")
                                  .SingleOrDefault(o => o.ZamowienieID == zamowienieId && o.Nazwisko == nazwisko);

            if (zamowienie == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            ZamowienieZrealizowaneEmail email = new ZamowienieZrealizowaneEmail();
            email.To = zamowienie.Email;
            email.From = "adrian94wilk@gmail.com";
            email.NumerZamowienia = zamowienie.ZamowienieID;
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ListaUzytkownikow(List<RoleViewModel> rolesWithUsers)
        {

            rolesWithUsers = new List<RoleViewModel>();

            var applicationRoles = db.Roles.Include(r => r.Users).ToList();
            var list = new List<SelectListItem>();

            foreach (var applicationRole in applicationRoles)
            {

                list.Add(new SelectListItem() { Value = applicationRole.Name, Text = applicationRole.Name, Selected = true });
                List<UserViewModel> usersByRole = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == applicationRole.Id))
                        .Select(u => new UserViewModel
                        {
                            UserId = u.Id,
                            UserName = u.UserName
                        }).ToList();
                RoleViewModel roleViewModel = new RoleViewModel()
                {
                    RoleId = applicationRole.Id,
                    CurrentRole = applicationRole.Name,
                    Name = list,
                    Users = usersByRole
                };
                ViewBag.Roles = list;
                rolesWithUsers.Add(roleViewModel);
            }

            return View(rolesWithUsers);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UsunUzytkownika(string email)
        {
          
            var user = UserManager.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) return null;
            var zamowienia = db.Zamowienia.Where(x => x.User.Email == email).ToList();
            foreach(var zamowienie in zamowienia)
            {
               user.Zamowienia.Remove(zamowienie);
            }
            var result = await UserManager.DeleteAsync(user);
            
            return RedirectToAction("ListaUzytkownikow");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EdytujRole2(string rolaDoModyfikacji, string idUzytkownika)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == idUzytkownika);
            if (ModelState.IsValid)
            {            
                var oldUser = db.Users.Find(user.Id);
                var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
                var oldRoleName = db.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;

                if (oldRoleName != rolaDoModyfikacji && user.Id==idUzytkownika)
                {
                     await UserManager.RemoveFromRoleAsync(user.Id, oldRoleName);
                     await UserManager.AddToRoleAsync(user.Id, rolaDoModyfikacji);
                }
                db.Entry(user).State = EntityState.Modified;              
                db.SaveChanges();
            }
            return  RedirectToAction("ListaUzytkownikow");
        }
    }
}