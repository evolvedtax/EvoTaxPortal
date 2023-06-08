using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblUser
{
    public short UserId { get; set; }

    public short? GroupId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int? StatusId { get; set; }

    public virtual TblUserGroup? Group { get; set; }

    public virtual ICollection<TblLoginHistory> TblLoginHistories { get; set; } = new List<TblLoginHistory>();
}
