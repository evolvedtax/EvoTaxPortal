using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PasswordSecuredQ1 { get; set; }
        public string? PasswordSecuredA1 { get; set; }
        public string? PasswordSecuredQ2 { get; set; }
        public string? PasswordSecuredA2 { get; set; }
        public string? PasswordSecuredQ3 { get; set; }
        public string? PasswordSecuredA3 { get; set; }
        public string? Country { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Province { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpiryDate { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string? Position { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeZone { get; set; }
        public int InstituteId { get; set; } = 0;
    }
}
