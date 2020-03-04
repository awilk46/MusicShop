using MusicShop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MusicShop.Controllers
{
    public class AlbumyController : Controller
    {
        private AlbumContext db = new AlbumContext();
        // GET: Albumy
        public ActionResult Index()
        {
            return View();
        }
   
public ActionResult Lista(string nazwaKategorii,/* int? page,*/ string searchQuery = null)
        {

            var kategorieMenu = db.Kategorie.Include("Albumy").Where(a => a.NazwaKategorii.ToUpper() == nazwaKategorii.ToUpper()).Single();
            var albumy = kategorieMenu.Albumy.Where(a => (searchQuery == null ||
                                           a.NazwaAlbumu.ToLower().Contains(searchQuery.ToLower()) ||
                                           a.NazwaZespolu.ToLower().Contains(searchQuery.ToLower())) &&
                                           !a.Ukryty);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlbumyList", albumy);
            }

            //int pageSize = 12;
            //int pageNumber = (page ?? 1);
            //return View(albumy.ToPagedList(pageNumber, pageSize));
            return View(albumy);
        }


        public ActionResult Szczegoly(int id)
        {
            var album = db.Albumy.Find(id);
            return View(album);
        }
        //child action- wywołana z poziomu innej akcji
        //outputcache- przetrzymujemy kategorie w cache na 1 dzien
        [ChildActionOnly]
        [OutputCache(Duration =60000)]
        public ActionResult KategorieMenu()
        {
            var kategorieMenu = db.Kategorie.ToList();
            return PartialView("_KategorieMenu",kategorieMenu);
        }
        public ActionResult AlbumyPodpowiedzi(string term)
        {
            var albumy = db.Albumy.Where(a => !a.Ukryty && a.NazwaAlbumu.ToLower().
            Contains(term.ToLower())).Take(5).Select(a=> new { label=a.NazwaAlbumu});
            return Json(albumy,JsonRequestBehavior.AllowGet);
        }
    }
}