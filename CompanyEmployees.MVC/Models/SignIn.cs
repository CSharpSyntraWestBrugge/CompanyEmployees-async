using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Models
{
    public class SignIn
    {
        [Required]
        [Display(Name = "Gebruikersnaam")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Onthoud mij")]
        public bool RememberMe { get; set; }

    }
}
