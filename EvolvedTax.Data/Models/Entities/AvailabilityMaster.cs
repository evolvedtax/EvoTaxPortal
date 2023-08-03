using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class AvailabilityMaster
{
    public int AvailabilityId { get; set; }

    public int? EmpId { get; set; }

    public TimeSpan? TimeStart { get; set; }

    public TimeSpan? TimeEnd { get; set; }

    public DateTime? AvailabilityDate { get; set; }
}
