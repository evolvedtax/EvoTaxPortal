using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class Tbl1099_ReminderDays
{
    public int Id { get; set; }
    public int? InstId { get; set; }
    public int? ReminderDays { get; set; }
}
