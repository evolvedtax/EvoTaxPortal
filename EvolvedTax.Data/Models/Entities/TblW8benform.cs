using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class TblW8benform
{
    public int Id { get; set; }

    public string? NameOfIndividual { get; set; }

    public string? CountryOfCitizenship { get; set; }

    public string? PermanentResidenceAddress { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? MailingAddress { get; set; }

    public string? MCity { get; set; }

    public string? MCountry { get; set; }

    public string? SsnOrItin { get; set; }

    public string? ForeignTaxIdentifyingNumber { get; set; }

    public bool? CheckIfFtinNotLegallyRequiredYN { get; set; }

    public string? ReferenceNumberS { get; set; }

    public string? DateOfBirthMmDdYyyy { get; set; }

    public string? ResidentCertification { get; set; }

    public string? ArticleAndParagraph { get; set; }

    public string? Rate { get; set; }

    public string? SpecifyTypeOfIncome { get; set; }

    public string? EligibleForTheRateOfWithholding { get; set; }

    public string? SignatureDateMmDdYyyy { get; set; }

    public string? PrintNameOfSigner { get; set; }

    public string? UploadedFile { get; set; }

    public string? Status { get; set; }

    public string? W8benprintName { get; set; }

    public int? W8benprintSize { get; set; }

    public DateTime? W8benentryDate { get; set; }

    public string? W8benfontName { get; set; }

    public string? W8benemailAddress { get; set; }

    public string? W8benonBehalfName { get; set; }
}
