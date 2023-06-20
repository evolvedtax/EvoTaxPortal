using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class InstitutesClient
{
    public int ClientId { get; set; }

    public short InstituteId { get; set; }

    public int EntityId { get; set; }

    public string EntityName { get; set; } = null!;

    /// <summary>
    /// Partner Name 1
    /// </summary>
    public string PartnerName1 { get; set; } = null!;

    public string? PartnerName2 { get; set; }

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string? Province { get; set; }

    public string Zip { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ClientEmailId { get; set; } = null!;

    public short? ClientStatus { get; set; }

    public DateTime? ClientStatusDate { get; set; }

    public string? FileName { get; set; }

    public string? FormName { get; set; }

    public bool? IsLocked { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? InActiveDate { get; set; }
}
