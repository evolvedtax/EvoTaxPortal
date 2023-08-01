using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class EvoTeam
{
    public string TeamMemberId { get; set; } = null!;

    public string? TeamMember { get; set; }

    public string? Role { get; set; }

    public string? Location { get; set; }

    public int Id { get; set; }
}
