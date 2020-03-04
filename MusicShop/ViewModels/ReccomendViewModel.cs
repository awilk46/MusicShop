using MusicShop.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class ReccomendViewModel
    {
        //ID użytkownika, dla którego będzie rekomendował album
        public int UzytkownikID { get; set; }
        //Lista rekomendowanych albumów 
        public IEnumerable<Album> Album { get; set; }     
    }
}