using EvolvedTax.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Response
{
    public class InstituteEntitiesResponse : InstituteEntity
    {
        public string LastUpdatedByName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Subscription { get; set; } = string.Empty;
    }
}
