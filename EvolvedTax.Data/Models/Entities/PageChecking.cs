using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class PageChecking
{
    public string? CurrentPage { get; set; }

    public string? PreviousPage { get; set; }

    public int? UserType { get; set; }
}
