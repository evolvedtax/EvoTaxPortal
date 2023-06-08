using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class MasterUser
{
    public short UserId { get; set; }

    public short TypeId { get; set; }

    public string? NameofIndividual1 { get; set; }

    public string? NameofIndividual2 { get; set; }

    public string? NameofEntity { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public int StatusId { get; set; }

    public string Justification { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? State { get; set; }

    public string? Province { get; set; }

    public string City { get; set; } = null!;

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string ZipPostalCode { get; set; } = null!;

    public DateTime? ExpiryDate { get; set; }

    public DateTime RequestDate { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string RequestIp { get; set; } = null!;
}
