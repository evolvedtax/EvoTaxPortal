using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class EvoProjectsMaster
{
    public string ProjectId { get; set; } = null!;

    public string? ProjectName { get; set; }

    public DateTime? ProjectStartDate { get; set; }

    public DateTime? ProjectEndDate { get; set; }

    public string? ProjectStatus { get; set; }

    public int Id { get; set; }
}
