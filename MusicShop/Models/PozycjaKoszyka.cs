using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class PozycjaKoszyka
    {
        public Album Album { get; set; }
        public int Ilosc { get; set; }
        public decimal Wartosc { get; set; }
    }
}