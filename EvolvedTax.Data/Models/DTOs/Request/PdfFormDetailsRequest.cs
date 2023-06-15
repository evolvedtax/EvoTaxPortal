using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class PdfFormDetailsRequest
    {
        public string FontFamily { get; set; } = string.Empty;
        public string FontSize { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? BaseUrl { get; set; }
        public string FormName { get; set; } = string.Empty;
        public string EntityStatus { get; set; } = string.Empty;
        public DateTime? EntryDate { get; set; }
        public string PrintName { get; set; } = string.Empty;
        public string NameOfIndividual { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsSignaturePasted { get; set; }
        public bool Agreement1 { get; set; } = false;
        public bool Agreement2 { get; set; } = false;
        public List<PdfFormDetailsRequest> ButtonRequests { get; set; } = new List<PdfFormDetailsRequest>();
    }
}
