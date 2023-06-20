using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Response
{
    public class TaxPayerInfo
    {
        public string? TaxpayerName { get; set; }
        public string? TaxpayerSSN { get; set; }
        public string? Address { get; set; }
        public string? Year { get; set; }
        public string? State { get; set; }
    }

}
