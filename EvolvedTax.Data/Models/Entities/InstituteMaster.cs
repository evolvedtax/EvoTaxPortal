using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class InstituteMaster
{
    public int InstId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? InstitutionName { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? InstituteLogo { get; set; }

    public string? TypeofEntity { get; set; }

    public string? Idtype { get; set; }

    public string? Idnumber { get; set; }

    public string? Mcountry { get; set; }

    public string? Madd1 { get; set; }

    public string? Madd2 { get; set; }

    public string? Mcity { get; set; }

    public string? Mstate { get; set; }

    public string? Mprovince { get; set; }

    public string? Mzip { get; set; }

    public string? Pcountry { get; set; }

    public string? Padd1 { get; set; }

    public string? Padd2 { get; set; }

    public string? Pcity { get; set; }

    public string? Pstate { get; set; }

    public string? Pprovince { get; set; }

    public string? Pzip { get; set; }

    public string? Ftin { get; set; }

    public string? Gin { get; set; }

    public string? CountryOfIncorporation { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public DateTime? RegistrationExpiryDate { get; set; }

    public string? Status { get; set; }

    public DateTime? StatusDate { get; set; }

    public string? PasswordSecuredQ1 { get; set; }

    public string? PasswordSecuredA1 { get; set; }

    public string? PasswordSecuredQ2 { get; set; }

    public string? PasswordSecuredA2 { get; set; }

    public string? PasswordSecuredQ3 { get; set; }

    public string? PasswordSecuredA3 { get; set; }
}
