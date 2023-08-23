using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities
{
    public class AuditLog
    {
 
        public int AuditLogID { get; set; }

        [StringLength(255)]
        public string TableName { get; set; }

        [StringLength(255)]
        public string ColumnName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Action { get; set; }
    }
}
