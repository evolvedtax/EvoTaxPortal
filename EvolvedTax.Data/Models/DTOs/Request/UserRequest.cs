using System;
using System.Collections.Generic;
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
    }
}
