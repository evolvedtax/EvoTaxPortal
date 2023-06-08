using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblLicense
{
    public short LicenseId { get; set; }

    public DateTime EntryDate { get; set; }

    public string? ClientName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNo { get; set; }

    public string? Address { get; set; }

    public string? AddressOther { get; set; }

    public string? Version { get; set; }

    public string? ProductId { get; set; }

    public string ActivationId { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string Description { get; set; } = null!;

    public bool? IsLicensed { get; set; }

    public string? CountryName { get; set; }

    public string? StateId { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? ZipCode { get; set; }
}
