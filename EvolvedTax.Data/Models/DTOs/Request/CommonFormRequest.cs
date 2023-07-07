using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class CommonFormRequest
    {
        public string MCountry { get; set; } = string.Empty;
        public string MAddress1 { get; set; } = string.Empty;
        public string? MAddress2 { get; set; }
        public string MCity { get; set; } = string.Empty;
        public string? MState { get; set; }
        public string? MProvince { get; set; }
        public string MZipCode { get; set; } = string.Empty;

        public string PCountry { get; set; } = string.Empty;
        public string PAddress1 { get; set; } = string.Empty;
        public string? PAddress2 { get; set; }
        public string PCity { get; set; } = string.Empty;
        public string? PState { get; set; }
        public string? PProvince { get; set; }
        public string PZipCode { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;

        public string FormType { get; set; } = string.Empty;
        public string W8FormType { get; set; } = string.Empty;
        public string IndividualOrEntityStatus { get; set; } = string.Empty;
        public string PrintNameOfSigner { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;
        public string BasePath { get; set; } = string.Empty;
        public string TemplateFilePath { get; set; } = string.Empty;
    }
}
