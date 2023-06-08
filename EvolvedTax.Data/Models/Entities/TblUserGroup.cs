using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblUserGroup
{
    public string? GroupName { get; set; }

    public short GroupId { get; set; }

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();
}
