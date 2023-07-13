using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class ForgetPasswordRequest
    {

        [Remote("ValidateEmailAddress", "Account", ErrorMessage = "Email address does not exist.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password at least one capital letter, one small letter, one number, one special character, and be at least 8 characters long")]
        [Display(Name = "password")]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PasswordSecuredQ1 { get; set; } = string.Empty;
        public string PasswordSecuredA1 { get; set; } = string.Empty;
        public string PasswordSecuredQ2 { get; set; } = string.Empty;
        public string PasswordSecuredA2 { get; set; } = string.Empty;
        public string PasswordSecuredQ3 { get; set; } = string.Empty;
        public string PasswordSecuredA3 { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
    }
}
