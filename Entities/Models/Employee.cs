using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public enum GeslachtType
    { 
        Man,
        Vrouw,
        X
    }
    public class Employee
    {
        [Column("EmployeeId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "De naam van de werknemer is verplicht.")]
        [MaxLength(30, ErrorMessage = "De maximale lengte van de naam is 30.")]
        [Display(Name = "Naam")]
        public string Name { get; set; }
        [Required]
        [Range(18,67,ErrorMessage ="Leeftijd moet tussen 18 en 67 zijn")]
        [Display(Name = "Leeftijd")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Positie is een verplicht veld.")]
        [MaxLength(20, ErrorMessage = "De maximale lengte van positie is 20.")]
        [Display(Name = "Positie")]
        public string Position { get; set; }
        [MinLength(10,ErrorMessage="De minimale lengte van beschrijving is 10 karakters")]
        [MaxLength(500, ErrorMessage = "De maximale lengte van positie is 500 karakters")]
        [Display(Name = "Beschrijving")]
        public string Description { get; set; }

        [Display(Name="Geslacht")]
        public GeslachtType Gender { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
