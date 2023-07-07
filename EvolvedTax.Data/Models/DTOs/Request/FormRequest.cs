using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EvolvedTax.Data.Models.Entities;
public class FormRequest
{
    public string? _21Text;

    public string UserName { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string TypeofTaxNumber { get; set; } = string.Empty;
    public string USCitizen { get; set; } = string.Empty;
    public string US1 { get; set; } = string.Empty;
    public string IndividualOrEntityStatus { get; set; } = string.Empty;
    //-----------ENTITY STATUS---------//
    public string GQOrgName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string Ccountry { get; set; } = string.Empty;
    public string BackupWithHolding { get; set; } = string.Empty;
    public string Payeecode { get; set; } = string.Empty;
    public string W9Fatca { get; set; } = string.Empty;

    public bool? DE { get; set; } = false;

    public string? DEOwnerName { get; set; } = string.Empty;
    public bool DEW8ECI { get; set; } = false;

    public string? DEOwnerNameW8ECI { get; set; } = string.Empty;
    public string? TypeOfEntityForW8ECI { get; set; } = string.Empty;
    public bool? EnitityManagendOutSideUSA { get; set; } = false;
    public string? USPartner { get; set; } = string.Empty;
    public string? IdType { get; set; }
    public string? IdNumber { get; set; }
    public bool? RetirementPlan { get; set; } = false;
    public string AuthSignatoryName { get; set; } = string.Empty;


    //-----------W8EXPForm Fields Start-------------------//


    public bool _10a { get; set; } = false;
    public bool _10b { get; set; } = false;

    public string? _10b_Text { get; set; } = string.Empty;
    public bool _10c { get; set; } = false;
    public string? _10c_Text { get; set; } = string.Empty;
    public bool _11 { get; set; } = false;
    public bool _12 { get; set; } = false;
    public bool _13a { get; set; } = false;
    public string? _13a_Text { get; set; } = string.Empty;
    public bool _13b { get; set; } = false;
    public bool _13c { get; set; } = false;
    public bool _13d { get; set; } = false;
    public bool _14 { get; set; } = false;
    public bool _15 { get; set; } = false;
    public string? _15_Text1 { get; set; } = string.Empty;
    public string? _15_Text2 { get; set; } = string.Empty;
    public string? _15_Text3 { get; set; } = string.Empty;
    public bool _16 { get; set; } = false;
    public bool _17 { get; set; } = false;
    public bool _18 { get; set; } = false;
    public bool _19 { get; set; } = false;
    public bool _20a { get; set; } = false;
    public bool _20b { get; set; } = false;
    public bool _20c { get; set; } = false;
    public bool _21 { get; set; } = false;
    public string? _21_Text { get; set; } = string.Empty;
    public int W8ExpId { get; set; } = 0;
    public string W8EXPFatca { get; set; } = string.Empty;
    public string? GIN { get; set; } = string.Empty;
    public string? ForeigntaxIdentifyingNumber { get; set; } = string.Empty;
    public string? Referencenumber { get; set; } = string.Empty;


    //-----------W8EXPForm Fields End-------------------//



    //-----------W9-------------------//
    public int Id { get; set; }

    public string GQFirstName { get; set; } = string.Empty;
    public string GQLastName { get; set; } = string.Empty;
    public string GQCountry { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;

    public string MCountry { get; set; } = string.Empty;
    [Remote("ValidateAddress", "Entity", ErrorMessage = "P.O. box or in-care-of address is not allowed.")]
    [Display(Name = "Address Line 1")]
    public string MAddress1 { get; set; } = string.Empty;
    public string? MAddress2 { get; set; }
    public string MCity { get; set; } = string.Empty;
    public string? MState { get; set; }
    public string? MProvince { get; set; }
    public string MZipCode { get; set; } = string.Empty;

    public string PCountry { get; set; } = string.Empty;

    [Remote("ValidatePAddress", "Entity", ErrorMessage = "P.O. box or in-care-of address is not allowed.")]
    [Display(Name = "Address Line 1")]
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

    public bool? CheckIfFtinNotLegallyRequiredYN { get; set; } = false;
    public bool? LegallyRequired { get; set; } = false;

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

    public bool CheckIfFtinNotLegallyRequiredYNW8ECI { get; set; } = false;
    public bool CheckIfFtinNotLegallyRequiredYNW8BENE { get; set; } = false;

    public string? ReferenceNumberSW8ECI { get; set; }
    public string? ReferenceNumberSW8BENE { get; set; }
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
    public bool? W8ECIOnBehalfName { get; set; }

    #region W8IMYForm
    public string FatcaStatus { get; set; } = string.Empty;

    public string? US_TIN_CB { get; set; } = string.Empty;
    public string US_TIN { get; set; } = string.Empty;

    public string? _11FATCA_CB { get; set; } = string.Empty;
    public string? _12_City { get; set; } = string.Empty;
    public string? _12_Country { get; set; } = string.Empty;
    public string? _12Mailing_address { get; set; } = string.Empty;
    public string? _12_State { get; set; } = string.Empty;
    public string? _12_Province { get; set; } = string.Empty;
    public string? _13GIN { get; set; } = string.Empty;

    public bool _14_CB { get; set; } = false;
    public bool _15a { get; set; } = false;
    public bool _15b { get; set; } = false;
    public bool _15c { get; set; } = false;
    public bool _15d { get; set; } = false;
    public bool _15e { get; set; } = false;
    public bool _15f { get; set; } = false;
    public bool _15g { get; set; } = false;
    public bool _15h { get; set; } = false;
    public bool _15i { get; set; } = false;
    public bool _16a { get; set; } = false;
    public string? _16b { get; set; } = string.Empty;
    public bool _17a { get; set; } = false;
    public bool _17b { get; set; } = false;
    public bool _17c { get; set; } = false;
    public bool _17d { get; set; } = false;
    public bool _17e { get; set; } = false;
    public bool _18a { get; set; } = false;
    public bool _18b { get; set; } = false;
    public bool _18c { get; set; } = false;
    public bool _18d { get; set; } = false;
    public bool _18e { get; set; } = false;
    public bool _18f { get; set; } = false;
    public bool _19a { get; set; } = false;
    public bool _19b { get; set; } = false;
    public bool _19c { get; set; } = false;
    public bool _19d { get; set; } = false;
    public bool _19e { get; set; } = false;
    public bool _19f { get; set; } = false;
    public bool _20 { get; set; } = false;

    public bool _21a { get; set; } = false;
    public bool _21b { get; set; } = false;
    public bool _21c { get; set; } = false;
    public bool _21d { get; set; } = false;
    public bool _21e { get; set; } = false;
    public bool _21f { get; set; } = false;
    public bool _22 { get; set; } = false;
    public string _23a_text { get; set; } = string.Empty;
    public bool _23b { get; set; } = false;
    public bool _23c { get; set; } = false;
    public bool _24a { get; set; } = false;
    public bool _24b { get; set; } = false;
    public bool _24c { get; set; } = false;
    public bool _25 { get; set; } = false;
    public bool _26 { get; set; } = false;
  
    public string _27a_text { get; set; } = string.Empty;
    public bool _27b { get; set; } = false;
    public bool _28 { get; set; } = false;
    public bool _29 { get; set; } = false;
    public bool _30a { get; set; } = false;
    public bool _30b { get; set; } = false;
    public bool _30c { get; set; } = false;
    public bool _31 { get; set; } = false;
    public string _32_Text1 { get; set; } = string.Empty;
    public bool _32_CB1 { get; set; } = false;
    public string _32_Text2 { get; set; } = string.Empty;
    public string _32_Text3 { get; set; } = string.Empty;
    public string _32_CB2 { get; set; } = string.Empty;
    public string _32_CB3 { get; set; } = string.Empty;
 
    public bool _33a { get; set; } = false;
    public bool _33b { get; set; } = false;
    public bool _33c { get; set; } = false;
    public bool _33d { get; set; } = false;
    public bool _33e { get; set; } = false;
    public bool _33f { get; set; } = false;
    public bool _34 { get; set; } = false;
    public bool _35 { get; set; } = false;
    public DateTime? _35_Text { get; set; }
    public bool _36 { get; set; } = false;
    public DateTime? _36_Text { get; set; }
    public bool _37a { get; set; } = false;
    public string _37a_Text { get; set; } = string.Empty;
    public bool _37b { get; set; } = false;
    public string _37b_Text1 { get; set; } = string.Empty;
    public string _37b_Text2 { get; set; } = string.Empty;
    public bool _38 { get; set; } = false;
    public bool _39 { get; set; } = false;
    public bool _40 { get; set; } = false;
    public string _41_Text { get; set; } = string.Empty;
    public bool _42 { get; set; } = false;
    public bool IsPartialSave { get; set; } = false;
    public string? PrintNameOfSignerW8IMY { get; set; } = string.Empty;
    public string? W8IMYOnBehalfName { get; set; } = string.Empty;
    #endregion
}