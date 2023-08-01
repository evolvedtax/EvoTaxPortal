using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Response
{
    public class InstituteClientResponse : InstitutesClient
    {
        public string InstituteUserName { get; set; } = string.Empty;
        public string InstituteName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string LastUpdatedByName { get; set; } = string.Empty;
    }
}
