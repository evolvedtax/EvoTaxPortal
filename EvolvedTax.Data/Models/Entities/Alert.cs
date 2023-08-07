using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class Alert
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int InstituteID { get; set; }

    public string? AlertText { get; set; }

    public DateTime? CreatedDate { get; set; }
    public string? Title { get; set; }
    public int EntityID { get; set; }
    public bool IsRead { get; set; }
}
