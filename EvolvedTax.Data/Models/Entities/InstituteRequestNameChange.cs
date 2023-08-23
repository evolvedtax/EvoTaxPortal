using EvolvedTax.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class InstituteRequestNameChange
    {
        public int Id { get; set; }
        public int InstituteId { get; set; }
        public string OldName { get; set; } = string.Empty;
        public string NewName { get; set; } = string.Empty;
        public string RequesterUserId { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public DateTime RequestedOn { get; set; }
        public RequestChangeNameStatusEnum IsApproved { get; set; } = RequestChangeNameStatusEnum.Approved;
    }
}
