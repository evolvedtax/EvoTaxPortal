using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvolvedTax.Data.Models.Entities._1042
{
    public class Tbl_1042s
    {
        [Key]
        public int Id { get; set; }

        public string? ProRataBasisChkbx { get; set; }

        public int? UniqueFormID { get; set; }

        public string? WHACh3Status { get; set; }

        public string? WHACh4Status { get; set; }

        public string? RecipientTIN { get; set; }

        public int? RcpTINType { get; set; }

        public string? RcpAcctNo { get; set; }

        public string? RcpFirstAndMI { get; set; }

        public string? RcpLastNameCompany { get; set; }

        public string? RcpNameLine2 { get; set; }

        public string? RcpAddressType { get; set; }

        public string? RcpAddressLine1 { get; set; }

        public string? RcpAddressLine2 { get; set; }

        public string? RcpCityForeign3 { get; set; }

        public string? RcpStateUSCanada { get; set; }

        public string? RcpZipUSCanada { get; set; }

        public string? RcpCountryName { get; set; }

        public int? RcpCh3Status { get; set; }

        public int? RcpCh4Status { get; set; }

        public DateTime? RcpDOB { get; set; }

        public string? RcpForeignTIN { get; set; }

        public string? RcpGIIN { get; set; }

        public string? RcpLOBCode { get; set; }

        public string? RcpEmail { get; set; }

        public int? Box1Code { get; set; }

        public decimal? Box2Amount { get; set; }

        public string? Box3Chap3Chk { get; set; }

        public string? Box3aExemptCode { get; set; }

        public decimal? Box3bTaxRate { get; set; }

        public string? Box4Chap4Chk { get; set; }

        public string? Box4aExemptCode { get; set; }

        public decimal? Box4bTaxRate { get; set; }

        public decimal? Box5Amount { get; set; }

        public decimal? Box6Amount { get; set; }

        public decimal? Box7aAmount { get; set; }

        public string? Box7bCheck { get; set; }

        public string? Box7cCheck { get; set; }

        public decimal? Box8Amount { get; set; }

        public decimal? Box9Amount { get; set; }

        public decimal? Box11Amount { get; set; }

        public string? RcpTaxCountryName { get; set; }

        public string? Box14aPrimaryName { get; set; }

        public string? Box14bPrimaryEIN { get; set; }

        public string? Box14bPrimaryType { get; set; }

        public string? Box15aIFTEEIN { get; set; }

        public string? Box15bIFTECh3 { get; set; }

        public string? Box15cIFTECh4 { get; set; }

        public string? Box15dIFTENAME1 { get; set; }

        public string? Box15eIFTEGIIN { get; set; }

        public string? Box15fCountryName { get; set; }

        public string? Box15gForeignTIN { get; set; }

        public string? Box15hIFTEAddr1 { get; set; }

        public string? Box15hIFTEAddr2 { get; set; }

        public string? Box15dIFTENAME2 { get; set; }

        public string? Box15dIFTENAME3 { get; set; }

        public string? Box15iIFTECity { get; set; }

        public string? Box15iStateProv { get; set; }

        public string? Box15iZipPostal { get; set; }

        public string? Box16aPayersName { get; set; }

        public string? Box16bPayersTIN { get; set; }

        public string? Box16cPayersGIIN { get; set; }

        public string? Box16dPayersCh3 { get; set; }

        public string? Box16ePayersCh4 { get; set; }

        public decimal? Box17aAmount { get; set; }

        public string? Box17bStateTaxNo { get; set; }

        public string? Box17cState { get; set; }

        public string? FormSource { get; set; }

        public string? FormCategory { get; set; }

        public string? BatchID { get; set; }

        public string? Uploaded_File { get; set; }

        public int? Status { get; set; }

        public string? Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string? UserId { get; set; }

        public int? InstID { get; set; }

        public int? EntityId { get; set; }

        public bool IsDuplicated { get; set; }

        public string? Province { get; set; }

        public string? PostalCode { get; set; }

    }



}
