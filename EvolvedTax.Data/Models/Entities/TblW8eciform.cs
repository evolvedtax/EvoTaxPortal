using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblW8eciform
{
    public int Id { get; set; }

    public string? NameOfIndividual { get; set; }

    public string? CountryOfIncorporation { get; set; }

    public string? DisregardedEntity { get; set; }

    public string? TypeOfEntity { get; set; }

    public string? PermanentResidenceAddress { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? MailingAddress { get; set; }

    public string? MCity { get; set; }

    public string? Ssnitnein { get; set; }

    public string? SsnOrItin { get; set; }

    public string? ForeignTaxIdentifyingNumber { get; set; }

    public bool? CheckIfFtinNotLegallyRequiredYN { get; set; }

    public string? ReferenceNumberS { get; set; }

    public string? DateOfBirthMmDdYyyy { get; set; }

    public string? Items { get; set; }

    public bool? DealerCertification { get; set; }

    public string? PrintNameOfSigner { get; set; }

    public string? SignatureDateMmDdYyyy { get; set; }

    public string? UploadedFile { get; set; }

    public string? Status { get; set; }

    public string? W8eciprintName { get; set; }

    public int? W8eciprintSize { get; set; }

    public DateTime? W8ecientryDate { get; set; }

    public string? W8ecifontName { get; set; }

    public string? W8eciemailAddress { get; set; }

    public string? W8ecionBehalfName { get; set; }
}
