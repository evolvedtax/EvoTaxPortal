using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class InstituteSignUpFormRequest
    {
        #region Signup Form Fields

        public int SUInstID { get; set; }
        public string SUFirstName { get; set; } = string.Empty;
        public string SULastName { get; set; } = string.Empty;
        public string SUCountry { get; set; } = string.Empty;
        public string SUInstitutionName { get; set; } = string.Empty;
        public DateTime SURegistrationDate { get; set; }
        public DateTime? SURegistrationExpiryDate { get; set; }



        [Remote("ValidateEmailDomainAddress", "TaxInformation", "Email address is not allow")]
        [Display(Name = "official email")]
        public string SUEmailAddress { get; set; } = string.Empty;

        public string SupportEmailAddress { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password at least one capital letter, one small letter, one number, one special character, and be at least 8 characters long")]
        [Display(Name = "password")]
        public string SUPassword { get; set; } = string.Empty;


        [Compare("SUPassword", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "confirm password")]
        public string SUConfirmPassword { get; set; }
        public string SUPasswordSecuredQ1 { get; set; } = string.Empty;
        public string SUPasswordSecuredA1 { get; set; } = string.Empty;
        public string SUPasswordSecuredQ2 { get; set; } = string.Empty;
        public string SUPasswordSecuredA2 { get; set; } = string.Empty;
        public string SUPasswordSecuredQ3 { get; set; } = string.Empty;
        public string SUPasswordSecuredA3 { get; set; } = string.Empty;
        public string SUTypeofEntity { get; set; } = string.Empty;
        public string SUIDType { get; set; } = string.Empty;
        public string SUIDNumber { get; set; } = string.Empty;

        public string SUMCountry { get; set; } = string.Empty;
        [Remote("ValidateAddress", "TaxInformation", ErrorMessage = "Do not use a P.O. box or in-care-of address")]
        [Display(Name = "Address Line 1")]
        public string SUMMAdd1 { get; set; } = string.Empty;
        public string? SUMMAdd2 { get; set; }
        public string SUMCity { get; set; } = string.Empty;
        public string SUMProvince { get; set; } = string.Empty;
        public string? SUMState { get; set; }

        public string SUMZip { get; set; } = string.Empty;

        public string SUPCountry { get; set; } = string.Empty;
        [Remote("ValidatePAddress", "TaxInformation", ErrorMessage = "Do not use a P.O. box or in-care-of address")]
        [Display(Name = "Address Line 1")]
        public string SUMPAdd1 { get; set; } = string.Empty;
        public string? SUPPAdd2 { get; set; }
        public string SUPCity { get; set; } = string.Empty;
        public string? SUPProvince { get; set; } = string.Empty;
        public string? SUPState { get; set; }
        public string SUPZip { get; set; } = string.Empty;

        public string? SUFTIN { get; set; } = string.Empty;
        public string? SUGIN { get; set; } = string.Empty;
        public string? SUCountryOfIncorporation { get; set; } = string.Empty;



        #endregion
    }
}
