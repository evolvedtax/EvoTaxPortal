using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblCountry
{
    public int CountryId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? CountryName { get; set; }

    public string? SortOrder { get; set; }
}
