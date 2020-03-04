using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class PozycjaZamowienia
    {
        public int PozycjaZamowieniaID { get; set; }
        public int ZamowienieID { get; set; }
        public int AlbumID { get; set; }
        public int Ilosc { get; set; }
        public decimal CenaZakupu { get; set; }

        public virtual Album Album { get; set; }
        public virtual Zamowienie Zamowienie { get; set; }
    }
}