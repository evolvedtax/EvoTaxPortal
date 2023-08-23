using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class EntitiesUsers
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public string? AssignedBy {  get; set;}
        public DateTime? EntryDatetime { get; set; }
        public DateTime? ExpirySignupDatetime { get; set; }

    }
}
