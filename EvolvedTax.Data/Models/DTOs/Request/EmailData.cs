using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs.Request
{
    public class EmailData
    {
        public string Email { get; set; }
        public int EntityId { get; set; }
        public string Role { get; set; }
        public string EntityName { get; set; }
    }
}
