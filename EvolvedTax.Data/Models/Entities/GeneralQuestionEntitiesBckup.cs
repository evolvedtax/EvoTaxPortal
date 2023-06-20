using System;
using System.Collections.Generic;

namespace EvolvedTax.Data.Models.Entities;

public partial class GeneralQuestionEntitiesBckup
{
    public int Id { get; set; }

    public string? OrgName { get; set; }

    public string? EntityType { get; set; }

    public string? Ccountry { get; set; }

    public string? TypeofTaxNumber { get; set; }

    public string? Number { get; set; }

    public string? Payeecode { get; set; }

    public string? Fatca { get; set; }

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

    public string? De { get; set; }

    public string? DeownerName { get; set; }

    public string? EnitityManagendOutSideUsa { get; set; }

    public string? BackupWithHolding { get; set; }

    public string? Uspartner { get; set; }

    public string? Idtype { get; set; }

    public string? Idnumber { get; set; }

    public string? W8formType { get; set; }

    public int? W8expId { get; set; }
}
