using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class Announcement
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedDate { get; set; }
}
