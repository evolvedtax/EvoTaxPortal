using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblW9form
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BusinessEntity { get; set; }

    public string? FederalTaxClassification { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Address1 { get; set; }

    public string? City1 { get; set; }

    public string? Country1 { get; set; }

    public string? ListofAccounts { get; set; }

    public string? Exemptions { get; set; }

    public string? Fatca { get; set; }

    public string? SsnTin { get; set; }

    public long? Trid { get; set; }

    public bool? Printed { get; set; }

    public string? UploadedFile { get; set; }

    public string? Status { get; set; }

    public string? W9printName { get; set; }

    public int? W9printSize { get; set; }

    public DateTime? W9entryDate { get; set; }

    public string? W9fontName { get; set; }

    public string? W9emailAddress { get; set; }
}
