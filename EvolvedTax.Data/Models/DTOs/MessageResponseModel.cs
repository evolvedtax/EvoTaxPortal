using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.DTOs
{
    public class MessageResponseModel
    {
        public bool Status { get; set; }
        public object? Message { get; set; }
        public string? Param { get; set; }
    }
}
