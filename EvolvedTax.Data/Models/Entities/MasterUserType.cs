using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class MasterUserType
{
    public short TypeId { get; set; }

    public string Type { get; set; } = null!;
}
