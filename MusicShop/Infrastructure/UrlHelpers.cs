using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.Infrastructure
{
    public static class UrlHelpers
    {
        public static string IkonyKategoriiSciezka(this UrlHelper helper, string nazwaKategorii)
        {
            var IkonyKateforiiFolder = AppConfig.IkonyKategoriiFolderWzgledny;
            var sciezka = Path.Combine(IkonyKateforiiFolder, nazwaKategorii);
            var sciezkaBezwzgledna = helper.Content(sciezka);
            return sciezkaBezwzgledna;
        }
    }
}