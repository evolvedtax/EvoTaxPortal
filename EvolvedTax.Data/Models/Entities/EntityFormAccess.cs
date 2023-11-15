using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class EntityFormAccess
{
    public int Id { get; set; }
    public int? InstituteID { get; set; }
    public int? EntityId { get; set; }
    public int? FormNameId { get; set; }
    public DateTime? EntryDateTime { get; set; }
}
