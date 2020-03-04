using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class KoszykViewModel
    {
        public List<PozycjaKoszyka> PozycjaKoszyka { get; set; }
        public decimal CenaCalkowita { get; set; }
    }
}