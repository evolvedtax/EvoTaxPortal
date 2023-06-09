using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class ExemptPayeeCode
{
    public string? ExemptCode { get; set; }

    public string? ExemptValue { get; set; }
    public int ExemptId { get; set; }
}
