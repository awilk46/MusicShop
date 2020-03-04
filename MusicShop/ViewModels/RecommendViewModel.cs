using MusicShop.Models;
using NReco.CF.Taste.Recommender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class RecommendViewModel 
    {
        public int AlbumID { get; set; }
        public string UserID { get; set; }
        public Album Album { get; set; }

    }
}