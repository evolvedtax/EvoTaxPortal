using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class PasswordSecurityQuestion
{
    public short PasswordSecurityQuestionId { get; set; }

    public string SecurityQuestion { get; set; } = null!;
}
