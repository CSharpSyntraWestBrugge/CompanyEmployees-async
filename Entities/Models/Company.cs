using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public enum CompanySize
    { 
        Small,
        Medium,
        Big
    }
    [Table("Company")]
    public class Company
    {
        [Column("CompanyId")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Naam is een verplicht veld.")]
        [MaxLength(60, ErrorMessage = "Maximum lengte voor de naam is 60.")]
        [Display(Name = "Naam")]
        public string Name { get; set; }
        [Required(ErrorMessage = "adres is een verplicht veld.")]
        [MaxLength(60, ErrorMessage = "Maximum lengte voor het adres is 60")]
        [Display(Name = "Adres")]
        public string Address { get; set; }
        [Display(Name = "Land")]
        public string Country { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}", ApplyFormatInEditMode =true)]
        [Display(Name="LanceerDatum")]
        public DateTime? LaunchDate { get; set;}
        [Display(Name="Grootte")]
        public CompanySize Size { get; set; }

        [MinLength(5, ErrorMessage = "De minimale lengte van beschrijving is 5 karakters")]
        [MaxLength(250, ErrorMessage = "De maximale lengte van positie is 250 karakters")]
        [Display(Name = "Beschrijving")]
        public string Description { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
