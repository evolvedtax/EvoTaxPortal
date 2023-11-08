using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class InstituteEmailTemplate
    {
        public int Id { get; set; }
        public int InstituteID { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public DateTime EntryDatetime { get; set; }
        public DateTime? UpdatedDatetime { get; set; }
        public string? UpdateBy { get; set; }
    }
}
