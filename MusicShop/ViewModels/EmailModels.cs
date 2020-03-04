using MusicShop.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class PotwierdzenieZamowieniaEmail : Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public decimal Wartosc { get; set; }
        public int NumerZamowienia { get; set; }
        public string sciezkaObrazka { get; set; }
        public List<PozycjaZamowienia> PozycjeZamowienia { get; set; }
    }

    public class ZamowienieZrealizowaneEmail : Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public int NumerZamowienia { get; set; }
    }
}