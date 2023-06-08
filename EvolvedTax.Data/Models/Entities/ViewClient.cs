using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class ViewClient
{
    public string EntityName { get; set; } = null!;

    public string? PartnerName { get; set; }

    public string? Address { get; set; }

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string? Province { get; set; }

    public string Zip { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ClientEmailId { get; set; } = null!;

    public short InstituteId { get; set; }

    public int ClientId { get; set; }

    public int EntityId { get; set; }

    public string? ClientStatus { get; set; }

    public DateTime? ClientStatusDate { get; set; }

    public string? FileName { get; set; }
}
