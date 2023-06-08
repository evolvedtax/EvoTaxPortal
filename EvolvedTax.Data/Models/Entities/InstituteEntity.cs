using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class InstituteEntity
{
    /// <summary>
    /// EntityID
    /// </summary>
    public int EntityId { get; set; }

    public string? EntityName { get; set; }

    public string? Ein { get; set; }

    public DateTime? EntityRegistrationDate { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Province { get; set; }

    public string? Zip { get; set; }

    public string? Country { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public int InstituteId { get; set; }

    public string? InstituteName { get; set; }
}
