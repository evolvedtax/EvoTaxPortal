using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EvolvedTax.Data.Models.Entities;
public class W8IMY
{
  

    public string UserName { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;




    public string GQOrgName { get; set; } = string.Empty;
    public string? CountryOfIncorporation { get; set; }

    public string EntityType { get; set; } = string.Empty;
    public bool? DE { get; set; } = false;
    public string? DEOwnerName { get; set; } = string.Empty;
    public string FatcaStatus { get; set; } = string.Empty;

    public bool? US_TIN_CB { get; set; } = false;
    public string US_TIN { get; set; } = string.Empty;
    public string? GIN { get; set; } = string.Empty;



    public string TypeofTaxNumber { get; set; } = string.Empty;
    public string? ForeignTaxIdentifyingNumber { get; set; } = string.Empty;
    public bool? LegallyRequired { get; set; } = false;
    public string? Referencenumber { get; set; } = string.Empty;
    public string? _11FATCA_CB { get; set; } = string.Empty;
    public string? _12_City { get; set; } = string.Empty;
    public string? _12_Country { get; set; } = string.Empty;
    public string? _12Mailing_address { get; set; } = string.Empty;
    public string? _12_State { get; set; } = string.Empty;
    public string? _12_Province { get; set; } = string.Empty;
    public string? _13GIN { get; set; } = string.Empty;


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






    public string AuthSignatoryName { get; set; } = string.Empty;




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
    public  bool _20b { get; set; } = false;
    public bool _20c { get; set; } = false;
    public bool _21 { get; set; } = false;
    public string? _21_Text { get; set; } = string.Empty;
    public int W8ExpId { get; set; } = 0;
    public string W8EXPFatca { get; set; } = string.Empty;



}