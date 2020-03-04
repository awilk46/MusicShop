using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class EditAlbumViewModel
    {
        public Album Album { get; set; }
        public IEnumerable<Kategoria> Kategorie { get; set; }
        public bool? Potwierdzenie { get; set; }
    }
}