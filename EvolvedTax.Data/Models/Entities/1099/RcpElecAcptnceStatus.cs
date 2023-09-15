using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities._1099
{
    public class RcpElecAcptnceStatus
    {
        [Key]
        public int Id { get; set; }
        public int Status { get; set; } = 0;
        public string Rcp_Email { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
