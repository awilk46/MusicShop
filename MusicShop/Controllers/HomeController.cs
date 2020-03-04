using MusicShop.DAL;
using MusicShop.Infrastructure;
using MusicShop.Models;
using MusicShop.ViewModels;
using NLog;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Controllers
{
    public class HomeController : Controller
    {
        private AlbumContext db = new AlbumContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Home
        public ActionResult Index()
        {
            logger.Info("Jesteś na stronie głównej");

            ICacheProvider cache = new DefaultCacheProvider();
      
            List<Kategoria> kategorie;
            if (cache.IsSet(Const.KategorieCacheKey))
            {
                kategorie = cache.Get(Const.KategorieCacheKey) as List<Kategoria>;
            }
            else
            {
                kategorie = db.Kategorie.ToList();
                cache.Set(Const.KategorieCacheKey, kategorie, 60);
            }

            List<Album> nowosci;
            if (cache.IsSet(Const.NowosciCacheKey))
            {
                nowosci = cache.Get(Const.NowosciCacheKey) as List<Album>;
            }
            else
            {
                nowosci = db.Albumy.Where(a => !a.Ukryty).OrderByDescending(a => a.DataDodania).Take(3).ToList();
                cache.Set(Const.NowosciCacheKey,nowosci,60);
            }
            //Guid tworzy unikatowe wartości dla parametrów, za każdym razem będą inaczej wyświetlane przedmioty
            List<Album> bestsellery;
            if (cache.IsSet(Const.BestselleryCacheKey))
            {
                bestsellery = cache.Get(Const.BestselleryCacheKey) as List<Album>;
            }
            else
            {
                bestsellery = db.Albumy.Where(k => !k.Ukryty && k.Bestseller).OrderBy(a => Guid.NewGuid()).Take(3).ToList();
                cache.Set(Const.BestselleryCacheKey, bestsellery, 60);
            }
            

            var hvm = new HomeViewModel()
            {
                Kategorie = kategorie,
                Nowosci = nowosci,
                Bestsellery = bestsellery

            };
            return View(hvm);
        }

        public ActionResult StronyStatyczne(string nazwa) {

            return View(nazwa);
        }
  
        public async Task<ActionResult> Help(HelpViewModel e)
        {
            if (ModelState.IsValid)
            { 
                // var ApiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var ApiKey = "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";
            var client = new SendGridClient(ApiKey);
            var from = new EmailAddress("adrian94wilk@gmail.com", "Any Name");
            var subject = e.Subject;
                var to = new EmailAddress(e.Email, e.Name);
                var plainTextContent = e.Message;
            var htmlContent = e.Message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                if (client != null)
                {
                    await client.SendEmailAsync(msg);
                }
                else
                {
                    Trace.TraceError("Failed to create Web transport.");
                    await Task.FromResult(0);
                }
            }
            return View();
        }
    }
}