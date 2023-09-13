using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs
{
    public class IpInfo
    {
        public string City { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string Isp { get; set; } = string.Empty;
    }
}
