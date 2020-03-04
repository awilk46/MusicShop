using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class AppConfig
    {
        private static string _ikonyKategoriiFolderWzgledny = ConfigurationManager.AppSettings["IkonyKategoriiFolder"];

        public static string IkonyKategoriiFolderWzgledny
        {
            get
            {
                return _ikonyKategoriiFolderWzgledny;
            }
        }
    }
}