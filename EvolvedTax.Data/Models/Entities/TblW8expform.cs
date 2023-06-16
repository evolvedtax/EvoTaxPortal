using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblW8expform
{
    public int Id { get; set; }

    public string? NameOfOrganization { get; set; }

    public string? CountryOfIncorporation { get; set; }

    public string? TypeOfEntity { get; set; }

    public string? FatcaStatus { get; set; }

    public string? PermanentResidenceAddress { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? MailingAddress { get; set; }

    public string? MCity { get; set; }

    public string? MCountry { get; set; }

    public string? SsnOrItin { get; set; }

    public string? Gin { get; set; }

    public string? ForeignTaxIdentifyingNumber { get; set; }

    public bool? CheckIfFtinNotLegallyRequiredYN { get; set; }

    public string? ReferenceNumberS { get; set; }

    public bool? _10a { get; set; }

    public bool? _10b { get; set; }

    public string? _10bText { get; set; }

    public bool? _10c { get; set; }

    public string? _10cText { get; set; }

    public bool? _11 { get; set; }

    public bool? _12 { get; set; }

    public bool? _13a { get; set; }

    public string? _13aText { get; set; }

    public bool? _13b { get; set; }

    public bool? _13c { get; set; }

    public bool? _13d { get; set; }

    public bool? _14 { get; set; }

    public bool? _15 { get; set; }

    public string? _15Text1 { get; set; }

    public string? _15Text2 { get; set; }

    public string? _15Text3 { get; set; }

    public bool? _16 { get; set; }

    public bool? _17 { get; set; }

    public bool? _18 { get; set; }

    public bool? _19 { get; set; }

    public bool? _20a { get; set; }

    public bool? _20b { get; set; }

    public bool? _20c { get; set; }

    public string? _21Text { get; set; }

    public bool? _21 { get; set; }

    public string? PrintNameOfSigner { get; set; }

    public string? SignatureDateMmDdYyyy { get; set; }

    public string? UploadedFile { get; set; }

    public string? Status { get; set; }

    public string? EmailAddress { get; set; }

    public int? PrintSize { get; set; }

    public string? FontName { get; set; }
}
