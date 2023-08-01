using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Name { get; set; }

    public string? Link { get; set; }

    public string? IconClass { get; set; }

    public int? ParentId { get; set; }
}
