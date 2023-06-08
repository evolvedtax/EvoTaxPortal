using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class GeneralQuestionIndividual
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CountryofCitizenship { get; set; }

    public string? TypeofTaxNumber { get; set; }

    public string? SocialSecurityNumber { get; set; }

    public string? Uscitizen { get; set; }

    public string? Us1 { get; set; }

    public string? RetirementPlan { get; set; }

    public string? PermanentCountry { get; set; }

    public string? PermanentState { get; set; }

    public string? PermanentProvince { get; set; }

    public string? PermanentCity { get; set; }

    public string? PermanentAddress1 { get; set; }

    public string? PermanentAddress2 { get; set; }

    public string? PermanentZip { get; set; }

    public string? MailingCountry { get; set; }

    public string? MailingState { get; set; }

    public string? MailingProvince { get; set; }

    public string? MailingCity { get; set; }

    public string? MailingAddress1 { get; set; }

    public string? MailingAddress2 { get; set; }

    public string? MailingZip { get; set; }

    public string UserName { get; set; } = null!;
}
