using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class Album
    {
        public int AlbumID { get; set; }
        public int KategoriaID { get; set; }
        [Required(ErrorMessage = "Wprowadź nazwę Albumu")]
        [StringLength(50)]
        public string NazwaAlbumu { get; set; }
        [Required(ErrorMessage = "Wprowadź nazwę Zespołu")]
        [StringLength(50)]
        public string NazwaZespolu { get; set; }
        public int RokWydania { get; set; }
        public RodzajKrazka RodzajKrazka { get; set; }
        public string Okladkalbumu { get; set; }
        public bool Bestseller { get; set; }
        public bool Nowosc { get; set; }
        public bool Ukryty { get; set; }
        public decimal CenaAlbumu { get; set; }
        public string Opis { get; set; }
        public DateTime DataDodania { get; set; }

        public virtual Kategoria Kategoria { get; set; }
    }
    public enum RodzajKrazka
    {
        Vinyl,
        CD,
        DVD,
        Kaseta
    }
  
}