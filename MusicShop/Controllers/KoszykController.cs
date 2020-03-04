using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MusicShop.App_Start;
using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.Models;
using MusicShop.ViewModels;
using NLog;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Impl.Neighborhood;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Impl.Similarity;
using NReco.CF.Taste.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Controllers
{
    public class KoszykController : Controller
    {
        private KoszykMenager koszykMenager;
        private ISessionMenager sessionMenager { get; set; }
        private AlbumContext db;
        private IMailService mailService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        public KoszykController(IMailService mailService)
        {
            this.mailService = mailService;
            db = new AlbumContext();
            sessionMenager = new SessionMenager();
            koszykMenager = new KoszykMenager(sessionMenager, db);
        }

        // GET: Koszyk
        public ActionResult Index()
        {
            var pozycjeKoszyka = koszykMenager.PobierzKoszyk();
            var cenaCalkowita = koszykMenager.PobierzWartoscKoszyka();

            KoszykViewModel kvm = new KoszykViewModel {
                PozycjaKoszyka =pozycjeKoszyka,
                CenaCalkowita =cenaCalkowita};

            return View(kvm);
        }
        public ActionResult DodajDoKoszyka(int id)
        {
            koszykMenager.DodajDoKoszyka(id);

            return RedirectToAction("Index");
        }

        public int PobierzIloscElementowKoszyka()
        {
            return koszykMenager.PobierzIloscPozycjiKoszyka();
        }

        public ActionResult UsunZKoszyka(int albumId)
        {
            int iloscPozycji = koszykMenager.UsunZKoszyka(albumId);
            int iloscPozycjiKoszyka = koszykMenager.PobierzIloscPozycjiKoszyka();
            decimal wartoscKoszyka = koszykMenager.PobierzWartoscKoszyka();

            var wynik = new KoszykUsuwanieViewModel
            {
                IdPozycjiUsuwanej = albumId,
                IloscPozycjiUsuwanej = iloscPozycji,
                KoszykCenaCalkowita = wartoscKoszyka,
                KoszykIloscPozycji = iloscPozycjiKoszyka,
            };
            return Json(wynik);
        }

        public async Task<ActionResult> Zaplac()
        {
            if (Request.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var zamowienie = new Zamowienie
                {
                    Imie = user.DaneUzytkownika.Imie,
                    Nazwisko = user.DaneUzytkownika.Nazwisko,
                    Adres = user.DaneUzytkownika.Adres,
                    Miasto = user.DaneUzytkownika.Miasto,
                    KodPocztowy = user.DaneUzytkownika.KodPocztowy,
                    Email = user.DaneUzytkownika.Email,
                    Phone = user.DaneUzytkownika.Telefon
                };
                return View(zamowienie);
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Zaplac", "Koszyk") });
        }
        
        [HttpPost]
        public async Task<ActionResult> Zaplac(Zamowienie zamowienieSzczegoly)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var newOrder = koszykMenager.UtworzZamowienie(zamowienieSzczegoly, userId);
                var user = await UserManager.FindByIdAsync(userId);
                TryUpdateModel(user.DaneUzytkownika);
                await UserManager.UpdateAsync(user);
                koszykMenager.PustyKoszyk();
                mailService.WyslaniePotwierdzenieZamowieniaEmail(newOrder);

                return RedirectToAction("PotwierdzenieZamowienia");
            }
            else
                return View(zamowienieSzczegoly);
        }
        public void Call(string url)
        {
            var req = HttpWebRequest.Create(url);
            req.GetResponseAsync();
        }

        public ActionResult PotwierdzenieZamowieniaEmail(int zamowienieId, string nazwisko)
        {
            var zamowienie = db.Zamowienia.Include("PozycjeZamowienia").
                   Include("PozycjeZamowienia.Album").SingleOrDefault(o => o.ZamowienieID == zamowienieId && o.Nazwisko == nazwisko);
            PotwierdzenieZamowieniaEmail email = new PotwierdzenieZamowieniaEmail();

            if(zamowienie == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            email.To = zamowienie.Email;
            email.From = "adrian94wilk@gmail.com";
            email.Wartosc = zamowienie.CenaZamowienia;
            email.NumerZamowienia = zamowienie.ZamowienieID;
            email.PozycjeZamowienia = zamowienie.PozycjeZamowienia;
            email.Send();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public ActionResult PotwierdzenieZamowienia()
        {
            var name = User.Identity.Name;
            logger.Info("Strona koszyk | potwierdzenie | " + name);
            return View();
        }


        public IDataModel LoadDataModel()
        {
            var query = (from a in db.Zamowienia
                         join b in db.PozycjeZamowienia
                         on a.ZamowienieID equals b.ZamowienieID
                         select new RecommendViewModel { UserID = a.UserID, AlbumID = b.AlbumID }).ToList();
            var dataModelLoader = new DataModelLoader(query, "UserID", "AlbumID");
            IDataModel dataModel = null;
            dataModel = dataModelLoader.Load();
            return dataModel;
        }
        public IDataModel GetDataModelForNewUser(IDataModel baseModel, params long[] preferredItems)
        {
            var plusAnonymModel = new PlusAnonymousUserDataModel(baseModel);
            var prefArr = new BooleanUserPreferenceArray(preferredItems.Length);
            prefArr.SetUserID(0, PlusAnonymousUserDataModel.TEMP_USER_ID);
            for (int i = 0; i < preferredItems.Length; i++)
            {
                prefArr.SetItemID(i, preferredItems[i]);
            }
            plusAnonymModel.SetTempPrefs(prefArr);
            return plusAnonymModel;
        }
        public ActionResult WyswietlRekomendacje()
        {

            int CurrentUserId = User.Identity.GetUserId().GetHashCode();
            if (User.Identity.GetUserId() == null)
            {
                CurrentUserId = 0;
            }

            var query = (from a in db.Zamowienia
                         join b in db.PozycjeZamowienia
                         on a.ZamowienieID equals b.ZamowienieID
                         select new RecommendViewModel { UserID = a.UserID, AlbumID = b.AlbumID }).ToList();

            var dataModelLoader = new DataModelLoader(query, "UserID", "AlbumID");
            IDataModel dataModel = null;
            dataModel = dataModelLoader.Load();

            //var ordersDataModel = LoadDataModel();

            var currentProductID = 57;
            var modelWithCurrentUser = GetDataModelForNewUser(dataModel, currentProductID);
            var similarity = new LogLikelihoodSimilarity(modelWithCurrentUser);
            var neighborhood = new NearestNUserNeighborhood(3, similarity, modelWithCurrentUser);
            var recommender = new GenericBooleanPrefUserBasedRecommender(modelWithCurrentUser, neighborhood, similarity);

            var recommendedItems = recommender.Recommend(CurrentUserId, 3);
            recommendedItems.ToList();
            //var rvm = new List<RecommendViewModel>();
           var rvm= recommendedItems.Select(x => new RecommendViewModel() {AlbumID=(int) x.GetItemID(),Album=db.Albumy.ToList().FirstOrDefault(r=>r.AlbumID== x.GetItemID()) }).ToList();
            

            return PartialView("_WyswietlRekomendacje", rvm);
        }
    }
}