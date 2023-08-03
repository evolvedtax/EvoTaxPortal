using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class EvoClientsMaster
{
    public string ClientId { get; set; } = null!;

    public string? ClientName { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactPersonDesignation { get; set; }

    public string? ClientContactNumber { get; set; }

    public string? ClientEmailAddress { get; set; }

    public string? ClientAddress { get; set; }

    public string? ClientCityStateZip { get; set; }

    public string? ClientNotes { get; set; }

    public int Id { get; set; }
}
