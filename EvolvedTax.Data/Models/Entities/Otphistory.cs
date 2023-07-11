using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class Otphistory
{
    public long Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpexpiryDate { get; set; }

    public string? UserType { get; set; }

    public DateTime? EntryDateTime { get; set; }
}
