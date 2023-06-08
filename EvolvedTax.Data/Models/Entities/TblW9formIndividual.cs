using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblW9formIndividual
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BusinessEntity { get; set; }

    public string? FederalTaxClassification { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? ListofAccounts { get; set; }

    public string? Exemptions { get; set; }

    public string? Fatca { get; set; }

    public string? SsnTin { get; set; }

    public long? Trid { get; set; }

    public bool? Printed { get; set; }

    public string? UploadedFile { get; set; }

    public string? UserName { get; set; }

    public DateTime? LastUpdatedon { get; set; }
}
