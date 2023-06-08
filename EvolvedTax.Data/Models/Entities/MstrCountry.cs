using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class MstrCountry
{
    public int Id { get; set; }

    public string CountryId { get; set; } = null!;

    public string? Country { get; set; }

    public string? Favorite { get; set; }
}
