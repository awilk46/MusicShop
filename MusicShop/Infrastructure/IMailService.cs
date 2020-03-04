using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicShop.Infrastructure
{
   public interface IMailService
    {
        void WyslaniePotwierdzenieZamowieniaEmail(Zamowienie zamowienie);
        void WyslanieZamowienieZrealizowaneEmail(Zamowienie zamowienie);
    }
}
