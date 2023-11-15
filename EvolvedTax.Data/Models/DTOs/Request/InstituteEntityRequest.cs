using Microsoft.AspNetCore.Mvc;
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
        [Remote("IsEntityNameExist", "Institute", AdditionalFields = "EntityId", ErrorMessage = "Entity name is already exist.")]
        public string EntityName { get; set; } = string.Empty;
        public string? InstituteName { get; set; }

        [Remote("IsEINExist", "Institute", AdditionalFields = "EntityId", ErrorMessage = "EIN is already exist.")]
        public string Ein { get; set; } = string.Empty;

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
        public short? LastUpdatedBy { get; set; }


    }
}
