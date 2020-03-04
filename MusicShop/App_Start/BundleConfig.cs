using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace MusicShop.App_Start
{
    //klasa do wczytywania skryptow, pobiera tylko fragmenty potrzebne do załadowania z danego skryptu
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css")
                .Include("~/Content/themes/base/core.css", "~/Content/themes/base/autocomplete.css",
                "~/Content/themes/base/theme.css", "~/Content/themes/base/menu.css"));
        }
    }
}