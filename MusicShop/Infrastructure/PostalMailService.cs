using MusicShop.Models;
using MusicShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class PostalMailService : IMailService
    {
        public void WyslaniePotwierdzenieZamowieniaEmail(Zamowienie zamowienie)
        {
            PotwierdzenieZamowieniaEmail email = new PotwierdzenieZamowieniaEmail();
            email.To = zamowienie.Email;
            email.From = "adrian94wilk@gmail.com";
            email.Wartosc = zamowienie.CenaZamowienia;
            email.NumerZamowienia = zamowienie.ZamowienieID;
            email.PozycjeZamowienia = zamowienie.PozycjeZamowienia;
            email.sciezkaObrazka = AppConfig.IkonyKategoriiFolderWzgledny;
            email.Send();
        }

        public void WyslanieZamowienieZrealizowaneEmail(Zamowienie zamowienie)
        {
            ZamowienieZrealizowaneEmail email = new ZamowienieZrealizowaneEmail();
            email.To = zamowienie.Email;
            email.From = "adrian94wilk@gmail.com";
            email.NumerZamowienia = zamowienie.ZamowienieID;
            email.Send();
        }
    }
}