using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Models
{
    public class Register
    {
        [Required]
        [Display(Name = "GebruikersNaam")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirmatie wachtwoord")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

    }
}
