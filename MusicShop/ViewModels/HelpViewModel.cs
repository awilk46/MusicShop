using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicShop.ViewModels
{
    public class HelpViewModel
    {
        [Required(ErrorMessage = "Wprowadź nazwę użytkownika")]
        [StringLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Wprowadź E-mail")]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Wprowadź wiadomość")]
        [StringLength(500)]
        [MinLength(20)]
        public string Message { get; set; }
        [Required(ErrorMessage = "Wprowadź temat wiadomości")]
        [StringLength(50)]
        public string Subject { get; set; }
    }
}