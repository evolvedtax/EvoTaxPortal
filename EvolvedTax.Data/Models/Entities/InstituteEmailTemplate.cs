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
        public string CustomTemplate { get; set; } = string.Empty;
        public string DefaultTemplate { get; set; } = string.Empty;
        public int FormNameId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? UpdatedDatetime { get; set; }
        public string? UpdateBy { get; set; }
    }
}
