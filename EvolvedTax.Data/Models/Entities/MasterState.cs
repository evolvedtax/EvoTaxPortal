using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class MasterState
{
    public int Id { get; set; }

    public string StateId { get; set; } = null!;

    public string? State { get; set; }
}
