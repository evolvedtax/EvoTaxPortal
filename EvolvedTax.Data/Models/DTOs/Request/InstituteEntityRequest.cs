using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class InstituteEntityRequest
    {
        public int EntityId { get; set; }
        public short InstituteId { get; set; }

        public string? EntityName { get; set; }
        public string? InstituteName { get; set; }

        public string? Ein { get; set; }

        public DateTime? EntityRegistrationDate { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Province { get; set; }

        public string? Zip { get; set; }

        public string? Country { get; set; }

        public DateTime? LastUpdatedDate { get; set; }
        public int? EmailFrequency { get; set; }

    }
}
