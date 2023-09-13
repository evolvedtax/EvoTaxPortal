using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public class AuditTrail1099
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime OTPExpiryTime { get; set; } = DateTime.Now;
        public string RecipientEmail { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0;
    }
}
