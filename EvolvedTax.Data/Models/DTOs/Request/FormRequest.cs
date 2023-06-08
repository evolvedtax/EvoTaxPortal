using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities;
public class FormRequest
{
    public string UserName { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string TypeofTaxNumber { get; set; } = string.Empty;
    public string USCitizen { get; set; } = string.Empty;
    public string US1 { get; set; } = string.Empty;
    //-----------W9-------------------//
    public int Id { get; set; }

    public string GQFirstName { get; set; } = string.Empty;
    public string GQLastName { get; set; } = string.Empty;
    public string GQCountry { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;

    public string MCountry { get; set; } = string.Empty;
    public string MAddress1 { get; set; } = string.Empty;
    public string? MAddress2 { get; set; }
    public string MCity { get; set; } = string.Empty;
    public string? MState { get; set; }
    public string? MProvince { get; set; }
    public string MZipCode { get; set; } = string.Empty;

    public string PCountry { get; set; } = string.Empty;
    public string PAddress1 { get; set; } = string.Empty;
    public string? PAddress2 { get; set; }
    public string PCity { get; set; } = string.Empty;
    public string? PState { get; set; }
    public string? PProvince { get; set; }
    public string PZipCode { get; set; } = string.Empty;
    public string EmailId { get; set; } = string.Empty;

    public string FormType { get; set; } = string.Empty;
    public string W8FormType { get; set; } = string.Empty;

    public string TemplateFilePath { get; set; } = string.Empty;

    //------------W8Ben----------------------//
    public string? NameOfIndividual { get; set; }

    public string? CountryOfCitizenship { get; set; }

    public string? PermanentResidenceAddress { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? MailingAddress { get; set; }

    public string? SsnOrItin { get; set; }

    public string? ForeignTaxIdentifyingNumber { get; set; }

    public bool? CheckIfFtinNotLegallyRequiredYN { get; set; }

    public string? ReferenceNumberS { get; set; }
    [BindProperty, DataType(DataType.Date)]
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
    public bool W8BENOnBehalfName { get; set; }
    //------------W8ECI------------------//
    public string? NameOfIndividualW8ECI { get; set; }

    public string? CountryOfCitizenshipW8ECI { get; set; }

    public string? PermanentResidenceAddressW8ECI { get; set; }

    public string? CityW8ECI { get; set; }

    public string? CountryW8ECI { get; set; }

    public string? MailingAddressW8ECI { get; set; }

    public string? SsnOrItinW8ECI { get; set; }

    public string? ForeignTaxIdentifyingNumberW8ECI { get; set; }

    public bool? CheckIfFtinNotLegallyRequiredYNW8ECI { get; set; }

    public string? ReferenceNumberSW8ECI { get; set; }
    [BindProperty, DataType(DataType.Date)]
    public string? DateOfBirthMmDdYyyyW8ECI { get; set; }

    public string? ResidentCertificationW8ECI { get; set; }

    public string? ArticleAndParagraphW8ECI { get; set; }


    public string? SpecifyTypeOfIncomeW8ECI { get; set; }

    public string? EligibleForTheRateOfWithholdingW8ECI { get; set; }

    public string? SignatureDateMmDdYyyyW8ECI { get; set; }

    public string? PrintNameOfSignerW8ECI { get; set; }

    public string? UploadedFileW8ECI { get; set; }

    public string? CountryOfIncorporation { get; set; }

    public string? DisregardedEntity { get; set; }

    public string? TypeOfEntity { get; set; }

    public string? Ssnitnein { get; set; }

    public string? Items { get; set; }

    public bool DealerCertification { get; set; }
    public bool IsYouCertifiedW8ECI { get; set; }
    public string? W9PrintName { get; set; }
    public bool W8ECIOnBehalfName { get; set; }
}