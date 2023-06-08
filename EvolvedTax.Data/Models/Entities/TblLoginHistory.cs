using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblLoginHistory
{
    public long TrNo { get; set; }

    public DateTime? LoginDateTime { get; set; }

    public short? LoginUserId { get; set; }

    public string? LoginUserIp { get; set; }

    public string? LoginUserLocation { get; set; }

    public string? LoginUserQuery { get; set; }

    public virtual TblUser? LoginUser { get; set; }
}
