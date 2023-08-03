using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class UserRequest
    {
        public short UserId { get; set; }
        public int InstId { get; set; }
        public short TypeId { get; set; }
        public string UserName { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public string InstituteName { get; set; } = string.Empty!;
        public int StatusId { get; set; }
        public bool IsLoggedIn { get; set; }
        public string? OTP { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsAdmin { get; set; }
        public string? InstituteLogo { get; set; }
        #region Signup Form Fields

        public int SUInstID { get; set; }
        [StringLength(40)]
        public string SUFirstName { get; set; } = string.Empty;
        [StringLength(40)]
        public string SULastName { get; set; } = string.Empty;
        public string SUCountry { get; set; } = string.Empty;
        [StringLength(40)]
        public string SUInstitutionName { get; set; } = string.Empty;
        [StringLength(40)]
        public string SUPosition { get; set; } = string.Empty;
        public DateTime SURegistrationDate { get; set; }
        public DateTime? SURegistrationExpiryDate { get; set; }

        [Remote("ValidateEmailDomainAddress", "Account", "Email address is not allowed")]
        [Display(Name = "Official Email")]
        public string SUEmailAddress { get; set; } = string.Empty;

        public string? SupportEmailAddress { get; set; } = null;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password at least one capital letter, one small letter, one number, one special character, and be at least 8 characters long")]
        [Display(Name = "password")]
        public string SUPassword { get; set; } = string.Empty;


        [Compare("SUPassword", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "confirm password")]
        public string SUConfirmPassword { get; set; }
        public string SUPasswordSecuredQ1 { get; set; } = string.Empty;
        [StringLength(100)]
        public string SUPasswordSecuredA1 { get; set; } = string.Empty;
        public string SUPasswordSecuredQ2 { get; set; } = string.Empty;
        [StringLength(100)]
        public string SUPasswordSecuredA2 { get; set; } = string.Empty;
        public string SUPasswordSecuredQ3 { get; set; } = string.Empty;
        [StringLength(100)]
        public string SUPasswordSecuredA3 { get; set; } = string.Empty;
        public string SUTypeofEntity { get; set; } = string.Empty;
        public string SUIDType { get; set; } = string.Empty;
        public string SUIDNumber { get; set; } = string.Empty;
        public string SUMCountry { get; set; } = string.Empty;
        //[Remote("ValidateAddress", "Account", ErrorMessage = "Do not use a P.O. box or in-care-of address")]
        [Display(Name = "Address Line 1")]
        [StringLength(35)]
        public string SUMMAdd1 { get; set; } = string.Empty;
        [StringLength(35)]
        public string? SUMMAdd2 { get; set; }
        [StringLength(22)]
        public string SUMCity { get; set; } = string.Empty;
        [StringLength(50)]
        public string SUMProvince { get; set; } = string.Empty;
        public string? SUMState { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode should contain only numbers.")]
        [StringLength(9)]
        public string SUMZip { get; set; } = string.Empty;

        public string SUPCountry { get; set; } = string.Empty;
        //[Remote("ValidatePAddress", "Account", ErrorMessage = "Do not use a P.O. box or in-care-of address")]
        //[Display(Name = "Address Line 1")]
        [StringLength(35)]
        public string SUMPAdd1 { get; set; } = string.Empty;
        [StringLength(35)]
        public string? SUPPAdd2 { get; set; }
        [StringLength(22)]
        public string SUPCity { get; set; } = string.Empty;
        [StringLength(50)]
        public string? SUPProvince { get; set; } = string.Empty;
        public string? SUPState { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessage = "ZipCode should contain only numbers.")]
        [StringLength(9)]
        public string SUPZip { get; set; } = string.Empty;

        public string? SUFTIN { get; set; } = string.Empty;
        [StringLength(9)]
        public string? SUGIN { get; set; } = string.Empty;
        public string? SUCountryOfIncorporation { get; set; } = string.Empty;
        [StringLength(15)]
        public string? Phone { get; set; } = string.Empty;
        #endregion
    }
}
