using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicShop.Models
{
    public class Kategoria
    {
        public int KategoriaID { get; set; }
        [Required(ErrorMessage = "Wprowadź nazwę Kategorii")]
        [StringLength(50)]
        public string NazwaKategorii { get; set; }
        public string ObrazKategorii { get; set; }
        public string OpisKategorii { get; set; }
        public virtual ICollection<Album> Albumy { get; set; }
    }
}