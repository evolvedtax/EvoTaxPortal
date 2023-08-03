using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class InstituteMasterRequest
    {
        public int InstId { get; set; }
        [StringLength(40)]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(40)]
        public string LastName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        [StringLength(40)]
        public string InstitutionName { get; set; } = string.Empty;

        [Remote("ValidateEmailDomainAddress", "TaxInformation", "Email address is not allowed")]
        [Display(Name = "Official Email")]
        public string EmailAddress { get; set; } = string.Empty;

        public string? InstituteLogo { get; set; } = null;


        public string Mcountry { get; set; } = string.Empty;
        //[Remote("ValidateAddress", "TaxInformation", ErrorMessage = "Do not use a P.O. box or in-care-of address")]
        [Display(Name = "Address Line 1")]
        [StringLength(35)]
        public string Madd1 { get; set; } = string.Empty;
        [StringLength(35)]
        public string? Madd2 { get; set; }
        [StringLength(22)]
        public string Mcity { get; set; } = string.Empty;
        [StringLength(50)]
        public string Mprovince { get; set; } = string.Empty;
        public string? Mstate { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode should contain only numbers.")]
        [StringLength(9)]
        public string Mzip { get; set; } = string.Empty;

        [StringLength(15)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Phone number only contain digits.")]
        public string? Phone { get; set; } = string.Empty;
        public IFormFile? ProfileImage { get; set; }
        public string? DateFormat { get; set; } = string.Empty; 
        public string? Position { get; set; } = string.Empty; 
        public string? Timezone { get; set; } = string.Empty;

        public int EmailFrequency { get; set; }
        public bool IsEmailFrequency { get; set; }
    }
}
