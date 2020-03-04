using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class Zamowienie
    {
        public int ZamowienieID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }


        [Required(ErrorMessage = "Wprowadź imię")]
        [StringLength(50)]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Wprowadź nazwisko")]
        [StringLength(50)]
        public string Nazwisko { get; set; }

        [Required(ErrorMessage = "Wprowadź Adres")]
        [StringLength(50)]
        public string Adres { get; set; }
        [Required(ErrorMessage = "Wprowadź miasto")]
        [StringLength(50)]
        public string Miasto { get; set; }
        [Required(ErrorMessage = "Wprowadź kod pocztowy")]
        [StringLength(50)]
        public string KodPocztowy { get; set; }

        [Required(ErrorMessage = "Wprowadź numer telefonu")]
        [StringLength(50)]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Wprowadź e-mail")]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime DataZamowienia { get; set; }
        public decimal CenaZamowienia { get; set; }
        public StanZamowienia StanZamowienia { get; set; }

        public List<PozycjaZamowienia> PozycjeZamowienia { get; set; }
    }

    public enum StanZamowienia
    {
        Nowe,
        Zrealizowane
    }
    
}