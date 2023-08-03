using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class MasterTimezone
{
    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? TimeZone { get; set; }

    public string? GmtOffset { get; set; }

    public int Id { get; set; }
}
