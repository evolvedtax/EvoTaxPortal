using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class PasswordChangeLog
{
    public short PassId { get; set; }

    public string Password { get; set; } = null!;

    public DateTime PasswordChangedDate { get; set; }

    public string EmailId { get; set; } = null!;
}
