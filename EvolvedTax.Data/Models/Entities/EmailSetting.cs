using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class EmailSetting
    {
        public byte Id { get; set; }
        public string EmailDoamin { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int SMTPPort { get; set; }
        public string SMTPServer { get; set; } = string.Empty;
        public int? POPPort { get; set; }
        public string? POPServer { get; set; }
    }
}
